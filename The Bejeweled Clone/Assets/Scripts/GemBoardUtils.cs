public static class GemBoardUtils
{
    public static int GetNumberOfMovesAvailable(GemBoard gemBoard)
    {
        int numMatchesFound = 0;

        // scans a 3x2 area on the gem board
        for (int currentRow = 0; currentRow <= gemBoard.Rows - 3; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.Columns - 2; currentCol++)
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
        for (int currentRow = 0; currentRow <= gemBoard.Rows - 2; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.Columns - 3; currentCol++)
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
        for (int currentRow = 0; currentRow <= gemBoard.Rows - 4; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.Rows - 1; currentCol++)
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
        for (int currentRow = 0; currentRow <= gemBoard.Rows - 1; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.Columns - 4; currentCol++)
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
}
