public class Gem
{
    public GemTypes GemType { get; set; }

    public GemBehaviour GemBehaviour { get; set; }

    // TODO To be removed once all scripts have explicitly accessed the GemType of the Gem
    public GemTypes gemType
    {
        get => GemBehaviour.gemType;
        set => GemBehaviour.gemType = value;
    }

    // TODO To be removed once all scripts have explicit access to hasBeenMatched on this Gem
    public bool hasBeenMatched
    {
        get => GemBehaviour.hasBeenMatched;
        set => GemBehaviour.hasBeenMatched = value;
    }

    // TODO To be removed once all scripts have explicit access to this
    public int rowOnBoard
    {
        get => GemBehaviour.rowOnBoard;
        set => GemBehaviour.rowOnBoard = value;
    }

    // TODO To be removed once all scripts have explicit access to this
    public int colOnBoard
    {
        get => GemBehaviour.colOnBoard;
        set => GemBehaviour.colOnBoard = value;
    }

    // TODO To be removed once all scripts have explicitly accessed the GemBehaviour via its property
    public static implicit operator GemBehaviour(Gem gem) => gem.GemBehaviour;
}
