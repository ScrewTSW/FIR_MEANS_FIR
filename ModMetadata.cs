using SPTarkov.Server.Core.Models.Spt.Mod;

namespace FIR_MEANS_FIR;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "eu.thescrewcollab.firmeansifir";
    public string Name { get; init; } = "FIR_MEANS_FIR";
    public string Author { get; init; } = "ScrewTSW";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.1.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public bool HasPrepatcher { get; init; } = false;
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
}
