using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using HarmonyLib;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Services;

namespace FIR_MEANS_FIR.Patches;

[Injectable]
public class SnapshotFirOnRaidStartPatch : AbstractPatch
{
    private static ISptLogger<SnapshotFirOnRaidStartPatch> _logger = default!;
    private static ProfileHelper _profileHelper = default!;

    internal static readonly ConcurrentDictionary<string, HashSet<string>> FirSnapshots = new();

    public SnapshotFirOnRaidStartPatch(ISptLogger<SnapshotFirOnRaidStartPatch> logger, ProfileHelper profileHelper)
    {
        _logger = logger;
        _profileHelper = profileHelper;
    }

    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(
            "StartLocalRaid",
            BindingFlags.Public | BindingFlags.Instance)!;
    }

    [PatchPrefix]
    public static void Prefix(MongoId sessionId, StartLocalRaidRequestData request)
    {
        var profile = _profileHelper.GetFullProfile(sessionId);
        var pmcData = profile?.CharacterData?.PmcData;
        if (pmcData?.Inventory?.Items == null) return;

        var firItemIds = pmcData.Inventory.Items
            .Where(i => i.Upd?.SpawnedInSession == true && i.Id != null)
            .Select(i => i.Id!.ToString()!)
            .ToHashSet();

        FirSnapshots[sessionId.ToString()!] = firItemIds;
        _logger.Info($"FIR_MEANS_FIR: Snapshotted {firItemIds.Count} FIR items for session {sessionId}");
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId)
    {
        if (!FirSnapshots.TryGetValue(sessionId.ToString()!, out var firItemIds)) return;

        var profile = _profileHelper.GetFullProfile(sessionId);
        var pmcData = profile?.CharacterData?.PmcData;
        if (pmcData?.Inventory?.Items == null) return;

        var restoredCount = 0;
        foreach (var item in pmcData.Inventory.Items)
        {
            if (item.Id == null) continue;
            if (!firItemIds.Contains(item.Id.ToString()!)) continue;
            if (item.Upd?.SpawnedInSession == true) continue;

            item.Upd ??= new Upd();
            item.Upd.SpawnedInSession = true;
            restoredCount++;
        }

        if (restoredCount > 0)
            _logger.Info($"FIR_MEANS_FIR: Re-applied FIR on {restoredCount} items at raid start for session {sessionId}");
    }
}

[Injectable]
public class RestoreFirOnRaidEndPatch : AbstractPatch
{
    private static ISptLogger<RestoreFirOnRaidEndPatch> _logger = default!;
    private static ModConfig _config = default!;

    public RestoreFirOnRaidEndPatch(ISptLogger<RestoreFirOnRaidEndPatch> logger)
    {
        _logger = logger;
    }

    public static void SetConfig(ModConfig config) => _config = config;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(LocationLifecycleService).GetMethod(
            "HandlePostRaidPmc",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    [PatchPostfix]
    public static void Postfix(MongoId sessionId, SPTarkov.Server.Core.Models.Eft.Profile.SptProfile fullServerProfile)
    {
        if (!_config.KeepFirOnBroughtItems) return;

        if (!SnapshotFirOnRaidStartPatch.FirSnapshots.TryRemove(sessionId.ToString()!, out var firItemIds))
            return;

        var pmcData = fullServerProfile.CharacterData?.PmcData;
        if (pmcData?.Inventory?.Items == null) return;

        var restoredCount = 0;
        foreach (var item in pmcData.Inventory.Items)
        {
            if (item.Id == null) continue;
            if (!firItemIds.Contains(item.Id.ToString()!)) continue;
            if (item.Upd?.SpawnedInSession == true) continue;

            item.Upd ??= new Upd();
            item.Upd.SpawnedInSession = true;
            restoredCount++;
        }

        if (restoredCount > 0)
            _logger.Info($"FIR_MEANS_FIR: Restored FIR on {restoredCount} brought items for session {sessionId}");
    }
}
