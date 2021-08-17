using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemMovesAvailableChecker : MonoBehaviour
{
    public TextMeshProUGUI movesAvailableCounter;

    public int GetNumberOfMatchesAvailable(Gem[,] gemBoard)
    {
        int numMatchesFound = 0;

        // scans a 3x2 area on the gem board
        for (int currentRow = 0; currentRow <= gemBoard.GetLength(0) - 3; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.GetLength(1) - 2; currentCol++)
            {
                // pattern legend (? denotes gem of any type, X denotes gem of a specific type)
                // ? X
                // ? X
                // X ?
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 2, currentCol].gemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {

                    numMatchesFound++;
                }

                // ? X
                // X ?
                // X ?
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol].gemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // X ?
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 2, currentCol].gemType)
                {
                    numMatchesFound++;
                }

                // ? X
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {
                    numMatchesFound++;
                }
            }
        }

        // scans a 2x3 area on the gem board
        for (int currentRow = 0; currentRow <= gemBoard.GetLength(0) - 2; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.GetLength(1) - 3; currentCol++)
            {
                // pattern legend (? denotes gem of any type, X denotes gem of a specific type)

                // ? ? X
                // X X ?
                if (gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType)
                {
                    numMatchesFound++;
                }

                // X X ?
                // ? ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 1].gemType
                    && gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    numMatchesFound++;
                }

                // X ? ?
                // ? X X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    numMatchesFound++;
                }

                // ? X X
                // X ? ?
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow, currentCol + 2].gemType
                    && gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow + 1, currentCol].gemType)
                {
                    numMatchesFound++;
                }

                // X ? X
                // ? X ?
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 2].gemType
                    && gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType)
                {
                    numMatchesFound++;
                }

                // ? X ?
                // X ? X
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    numMatchesFound++;
                }
            }
        }

        // scans a 4x1 area on the gem board
        for (int currentRow = 0; currentRow <= gemBoard.GetLength(0) - 4; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.GetLength(0) - 1; currentCol++)
            {
                // X
                // X
                // ?
                // X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 3, currentCol].gemType)
                {
                    numMatchesFound++;
                }

                // X
                // ?
                // X
                // X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 2, currentCol].gemType
                    && gemBoard[currentRow + 2, currentCol].gemType == gemBoard[currentRow + 3, currentCol].gemType)
                {
                    numMatchesFound++;
                }
            }
        }

        // scans a 1x4 area on the board
        for (int currentRow = 0; currentRow <= gemBoard.GetLength(0) - 1; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.GetLength(1) - 4; currentCol++)
            {
                // X ? X X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 2].gemType
                    && gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow, currentCol + 3].gemType)
                {
                    numMatchesFound++;
                }

                // X X ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 1].gemType
                    && gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow, currentCol + 3].gemType)
                {
                    numMatchesFound++;
                }
            }
        }

        return numMatchesFound;
    }

    public void UpdateMovesAvailableCounter(Gem[,] gemBoard)
    {
        movesAvailableCounter.text = GetNumberOfMatchesAvailable(gemBoard).ToString();
    }
}