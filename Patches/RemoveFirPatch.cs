using System.Reflection;
using HarmonyLib;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Helpers;

namespace FIR_MEANS_FIR.Patches;

[Injectable]
public class RemoveFirPatch : AbstractPatch
{
    private static ISptLogger<RemoveFirPatch> _logger = default!;
    private static ModConfig _config = default!;

    public RemoveFirPatch(ISptLogger<RemoveFirPatch> logger)
    {
        _logger = logger;
    }

    public static void SetConfig(ModConfig config) => _config = config;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(InRaidHelper).GetMethod(
            "RemoveFiRStatusFromItems",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        if (_config.KeepFirOnDeath || _config.KeepFirOnBroughtItems)
        {
            _logger.Info("FIR_MEANS_FIR: Skipping FIR removal on death");
            return false;
        }
        return true;
    }
}
