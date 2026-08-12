using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using EFT;
using SPT.Reflection.Patching;

namespace FIR_MEANS_FIR.Client;

[BepInPlugin("eu.thescrewcollab.firmeansifir.client", "FIR_MEANS_FIR Client", "1.0.0")]
public class FirMeansFirPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        new SetSpawnedInSessionPatch().Enable();
        Logger.LogInfo("FIR_MEANS_FIR Client: Patched SetSpawnedInSession - FIR items stay FIR in raid");
    }
}

public class SetSpawnedInSessionPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Profile).GetMethod("SetSpawnedInSession", BindingFlags.Public | BindingFlags.Instance);
    }

    [PatchPrefix]
    public static bool Prefix(bool value)
    {
        if (!value)
        {
            return false;
        }
        return true;
    }
}
