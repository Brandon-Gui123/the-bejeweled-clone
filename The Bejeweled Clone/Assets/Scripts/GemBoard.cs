public class GemBoard
{
    private Gem[,] gems;

    public GemBoard(int rows, int columns)
    {
        gems = new Gem[rows, columns];
    }

    public Gem this[int row, int column]
    {
        get => gems[row, column];
        set => gems[row, column] = value;
    }
}
