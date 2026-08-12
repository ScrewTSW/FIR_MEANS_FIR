using System.Reflection;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using FIR_MEANS_FIR.Patches;

namespace FIR_MEANS_FIR;

[Injectable]
public class FirMeansFir(
    ISptLogger<FirMeansFir> logger,
    ModHelper modHelper,
    RemoveFirPatch removeFirPatch,
    SecureContainerFirPatch secureContainerFirPatch,
    SnapshotFirOnRaidStartPatch snapshotPatch,
    RestoreFirOnRaidEndPatch restorePatch,
    BotFirPatch botFirPatch) : IOnLoad
{
    public Task OnLoad()
    {
        var modPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var config = modHelper.GetJsonDataFromFile<ModConfig>(modPath, "config.jsonc");

        RemoveFirPatch.SetConfig(config);
        SecureContainerFirPatch.SetConfig(config);
        RestoreFirOnRaidEndPatch.SetConfig(config);
        BotFirPatch.SetConfig(config);

        removeFirPatch.Enable();
        secureContainerFirPatch.Enable();
        snapshotPatch.Enable();
        restorePatch.Enable();
        botFirPatch.Enable();

        logger.Success("FIR_MEANS_FIR loaded");
        logger.Info($"  KeepFirOnDeath: {config.KeepFirOnDeath}");
        logger.Info($"  KeepFirOnBroughtItems: {config.KeepFirOnBroughtItems}");
        logger.Info($"  KeepFirInProtectedSlots: {config.KeepFirInProtectedSlots}");
        logger.Info($"  BotItemsAreFir: {config.BotItemsAreFir}");

        return Task.CompletedTask;
    }
}
