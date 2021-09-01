using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GemBoard : IEnumerable<Gem>
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

    public int Rows => gems.GetLength(0);
    public int Columns => gems.GetLength(1);

    public IEnumerator<Gem> GetEnumerator() => gems.Cast<Gem>().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => gems.GetEnumerator();
}
