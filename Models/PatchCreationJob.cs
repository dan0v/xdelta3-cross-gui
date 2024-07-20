namespace xdelta3_cross_gui.Models
{
    public record struct PatchCreationJob(string Options, string Source, string Goal, string PatchDestination);
}
