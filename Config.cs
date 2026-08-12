namespace FIR_MEANS_FIR;

public record ModConfig
{
    public bool KeepFirOnDeath { get; init; } = true;
    public bool KeepFirOnBroughtItems { get; init; } = true;
    public bool KeepFirInProtectedSlots { get; init; } = true;
    public bool BotItemsAreFir { get; init; } = true;
}
