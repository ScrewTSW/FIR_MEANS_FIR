using System.Reflection;
using HarmonyLib;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Extensions;

namespace FIR_MEANS_FIR.Patches;

[Injectable]
public class SecureContainerFirPatch : AbstractPatch
{
    private static ISptLogger<SecureContainerFirPatch> _logger = default!;
    private static ModConfig _config = default!;

    public SecureContainerFirPatch(ISptLogger<SecureContainerFirPatch> logger)
    {
        _logger = logger;
    }

    public static void SetConfig(ModConfig config) => _config = config;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(ItemExtensions).GetMethod(
            "RemoveFiRStatusFromItemsInContainer",
            BindingFlags.Public | BindingFlags.Static)!;
    }

    [PatchPrefix]
    public static bool Prefix()
    {
        if (_config.KeepFirInProtectedSlots)
        {
            _logger.Info("FIR_MEANS_FIR: Skipping FIR removal from secure container");
            return false;
        }
        return true;
    }
}
