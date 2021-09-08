public class Gem
{
    public GemTypes GemType { get; set; }
    public int RowOnBoard { get; set; }
    public int ColOnBoard { get; set; }
    public bool HasBeenMatched { get; set; }

    public GemBehaviour GemBehaviour { get; set; }

    // TODO To be removed once all scripts have explicitly accessed the GemBehaviour via its property
    public static implicit operator GemBehaviour(Gem gem) => gem.GemBehaviour;
}
