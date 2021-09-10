public class Gem
{
    public GemTypes GemType { get; set; }
    public int RowOnBoard { get; set; }
    public int ColOnBoard { get; set; }
    public bool HasBeenMatched { get; set; }
    public bool IsEmpty { get; set; }
}
