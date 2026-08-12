using System.Reflection;
using HarmonyLib;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Generators;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace FIR_MEANS_FIR.Patches;

[Injectable]
public class BotFirPatch : AbstractPatch
{
    private static ISptLogger<BotFirPatch> _logger = default!;
    private static ModConfig _config = default!;

    public BotFirPatch(ISptLogger<BotFirPatch> logger)
    {
        _logger = logger;
    }

    public static void SetConfig(ModConfig config) => _config = config;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(BotGenerator).GetMethod(
            "PrepareAndGenerateBot",
            BindingFlags.Public | BindingFlags.Instance)!;
    }

    [PatchPostfix]
    public static void Postfix(BotBase __result)
    {
        if (!_config.BotItemsAreFir)
            return;

        if (__result?.Inventory?.Items == null)
            return;

        int marked = 0;
        foreach (var item in __result.Inventory.Items)
        {
            if (item.Upd == null)
                item.Upd = new Upd { SpawnedInSession = true };
            else if (item.Upd.SpawnedInSession != true)
                item.Upd.SpawnedInSession = true;

            marked++;
        }

        _logger.Info($"FIR_MEANS_FIR: Marked {marked} items as FIR on bot {__result.Info?.Nickname}");
    }
}
