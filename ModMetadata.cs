using SPTarkov.Server.Core.Models.Spt.Mod;

namespace FIR_MEANS_FIR;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "eu.thescrewcollab.firmeansifir";
    public override string Name { get; init; } = "FIR_MEANS_FIR";
    public override string Author { get; init; } = "ScrewTSW";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.2.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override bool? IsBundleMod { get; init; } = false;
    public override string? Url { get; init; }
    public override string License { get; init; } = "MIT";
}
