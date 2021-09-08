using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                if (gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType
                    && gemBoard[currentRow + 1, currentCol + 1].GemType == gemBoard[currentRow + 2, currentCol].GemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 2, currentCol + 1].GemType)
                {

                    numMatchesFound++;
                }

                // ? X
                // X ?
                // X ?
                if (gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 2, currentCol].GemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // ? X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType
                    && gemBoard[currentRow + 1, currentCol + 1].GemType == gemBoard[currentRow + 2, currentCol + 1].GemType)
                {
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // X ?
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType
                    && gemBoard[currentRow + 1, currentCol + 1].GemType == gemBoard[currentRow + 2, currentCol].GemType)
                {
                    numMatchesFound++;
                }

                // ? X
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 2, currentCol + 1].GemType)
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
                if (gemBoard[currentRow, currentCol + 2].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType)
                {
                    numMatchesFound++;
                }

                // X X ?
                // ? ? X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow, currentCol + 1].GemType
                    && gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol + 2].GemType)
                {
                    numMatchesFound++;
                }

                // X ? ?
                // ? X X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType
                    && gemBoard[currentRow + 1, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol + 2].GemType)
                {
                    numMatchesFound++;
                }

                // ? X X
                // X ? ?
                if (gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow, currentCol + 2].GemType
                    && gemBoard[currentRow, currentCol + 2].GemType == gemBoard[currentRow + 1, currentCol].GemType)
                {
                    numMatchesFound++;
                }

                // X ? X
                // ? X ?
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow, currentCol + 2].GemType
                    && gemBoard[currentRow, currentCol + 2].GemType == gemBoard[currentRow + 1, currentCol + 1].GemType)
                {
                    numMatchesFound++;
                }

                // ? X ?
                // X ? X
                if (gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 1, currentCol + 2].GemType)
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
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 1, currentCol].GemType
                    && gemBoard[currentRow + 1, currentCol].GemType == gemBoard[currentRow + 3, currentCol].GemType)
                {
                    numMatchesFound++;
                }

                // X
                // ?
                // X
                // X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow + 2, currentCol].GemType
                    && gemBoard[currentRow + 2, currentCol].GemType == gemBoard[currentRow + 3, currentCol].GemType)
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
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow, currentCol + 2].GemType
                    && gemBoard[currentRow, currentCol + 2].GemType == gemBoard[currentRow, currentCol + 3].GemType)
                {
                    numMatchesFound++;
                }

                // X X ? X
                if (gemBoard[currentRow, currentCol].GemType == gemBoard[currentRow, currentCol + 1].GemType
                    && gemBoard[currentRow, currentCol + 1].GemType == gemBoard[currentRow, currentCol + 3].GemType)
                {
                    numMatchesFound++;
                }
            }
        }

        return numMatchesFound;
    }

    // Ensures that the board will not have 3 or more adjacent gems
    public static void EnsureNoMatches(GemBoard gemBoard, GemTypes[] gemTypesToUse)
    {
        // we'll start off with finding matches horizontally
        for (int currentRow = 0; currentRow < gemBoard.Rows; currentRow++)
        {
            for (int currentCol = 0; currentCol < gemBoard.Columns - 2; currentCol++)
            {
                Gem currentGem = gemBoard[currentRow, currentCol];
                Gem rightOfCurrentGem = gemBoard[currentRow, currentCol + 1];
                Gem rightOfRightGem = gemBoard[currentRow, currentCol + 2];

                if (currentGem.GemType == rightOfCurrentGem.GemType && rightOfCurrentGem.GemType == rightOfRightGem.GemType)
                {
                    // since we're starting off with no randomness,
                    // we'll choose on the gem to the right of the current
                    // that will be removed
                    Gem targetGem = rightOfCurrentGem;

                    // a list of gem types that the gem can become
                    // this will be changed as we look around
                    List<GemTypes> applicableGemTypes = gemTypesToUse.ToList();

                    // the current gem type shall be removed from the list
                    // since this has been the type we've been avoiding all the while
                    // we also make use of the return value to log us additional messages
                    // if things go wrong
                    if (!applicableGemTypes.Remove(targetGem.GemType))
                    {
                        ($"Horizontal matching: Unable to remove the gem type of the target gem itself ({targetGem.GemType})!\n"
                            + $"This may happen if the list of applicable gem types differ from the gem types available during gem generation.")
                            .Log(targetGem);
                    }

                    // the gems detected in horizontal matches shall only check upwards and downwards

                    if (rightOfCurrentGem.RowOnBoard >= 2)
                    {
                        Gem aboveTargeted = gemBoard[targetGem.RowOnBoard - 1, targetGem.ColOnBoard];
                        Gem aboveAboveTargeted = gemBoard[targetGem.RowOnBoard - 2, targetGem.ColOnBoard];

                        if (aboveTargeted.GemType == aboveAboveTargeted.GemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(aboveTargeted.GemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {aboveTargeted.GemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = aboveTargeted.GemType.ToString().Color(GemUtils.GetColorBasedOnGemType(aboveTargeted.GemType));

                                $"Horizontal matching: Found out that the targeted gem cannot be {colouredGemTypeString} because the two gems above are {colouredGemTypeString}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (rightOfCurrentGem.RowOnBoard < gemBoard.Rows - 2)
                    {
                        Gem belowTargeted = gemBoard[targetGem.RowOnBoard + 1, targetGem.ColOnBoard];
                        Gem belowBelowTargeted = gemBoard[targetGem.RowOnBoard + 2, targetGem.ColOnBoard];

                        if (belowTargeted.GemType == belowBelowTargeted.GemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(belowTargeted.GemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {belowTargeted.GemType} from the list of applicable gem types!"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = belowTargeted.GemType.ToString().Color(GemUtils.GetColorBasedOnGemType(belowTargeted.GemType));

                                $"Horizontal matching: Found out that the targeted gem cannot be {colouredGemTypeString} because the two gems below are {colouredGemTypeString}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (rightOfCurrentGem.RowOnBoard >= 1 && rightOfCurrentGem.RowOnBoard < gemBoard.Rows - 1)
                    {
                        // can look up and down by 1 space
                        // if these 2 gems are the same type, a match will occur, so we also need to
                        // consider this situation

                        Gem aboveTargeted = gemBoard[targetGem.RowOnBoard - 1, targetGem.ColOnBoard];
                        Gem belowTargeted = gemBoard[targetGem.RowOnBoard + 1, targetGem.ColOnBoard];

                        if (aboveTargeted.GemType == belowTargeted.GemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(belowTargeted.GemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {aboveTargeted.GemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = aboveTargeted.GemType.ToString().Color(GemUtils.GetColorBasedOnGemType(aboveTargeted.GemType));

                                $"Horizontal matching: Found out that the targeted gem cannot be {colouredGemTypeString} because the gem above and below are {colouredGemTypeString}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (applicableGemTypes.Count >= 1)
                    {
                        // randomly choose a gem type for the gem to use
                        GemTypes chosenGemType = applicableGemTypes[Random.Range(0, applicableGemTypes.Count)];

                        // log to console so we know what changed
                        string originalColourString = $"{targetGem.GemType}".Color(GemUtils.GetColorBasedOnGemType(targetGem.GemType));
                        string newColourString = $"{chosenGemType}".Color(GemUtils.GetColorBasedOnGemType(chosenGemType));

                        $"Horizontal matching: The gem at ({targetGem.RowOnBoard}, {targetGem.ColOnBoard}) will be changed from {originalColourString} to {newColourString}"
                            .Log(targetGem);

                        targetGem.GemType = chosenGemType;
                        //targetGem.UpdateGemColor();
                    }
                    else
                    {
                        // we don't have anything we can choose to stop the match from occurring
                        // for simplicity's sake, we don't alter the gem and just log this
                        ($"Horizontal matching: Unable to find a suitable gem type to change to for the gem at ({targetGem.RowOnBoard}, {targetGem.ColOnBoard})!\n"
                            + $"This can happen if we exhaust the list of applicable gem types for the gem to change to.")
                            .Log(targetGem);
                    }
                }
            }
        }

        // for finding matches vertically
        for (int currentRow = 0; currentRow < gemBoard.Rows - 2; currentRow++)
        {
            for (int currentCol = 0; currentCol < gemBoard.Columns; currentCol++)
            {
                Gem currentGem = gemBoard[currentRow, currentCol];
                Gem bottomOfCurrentGem = gemBoard[currentRow + 1, currentCol];
                Gem bottomOfBottomGem = gemBoard[currentRow + 2, currentCol];

                if (currentGem.GemType == bottomOfCurrentGem.GemType && currentGem.GemType == bottomOfBottomGem.GemType)
                {
                    // same as above, we'll chose the gem just 1 space below the current
                    Gem targetGem = bottomOfCurrentGem;

                    // this is a list of gem types that the gem can become
                    // note that this list will be changed as we progress
                    List<GemTypes> applicableGemTypes = gemTypesToUse.ToList();

                    // remove the current gem type from the list, since that
                    // type is what we want to switch away from
                    // we put this in a conditional so we will get alerted if we're
                    // unable to remove the gem type from the list
                    if (!applicableGemTypes.Remove(targetGem.GemType))
                    {
                        ($"Vertical matching: Unable to remove the gem type of the target gem itself ({targetGem.GemType})!\n"
                            + $"This may happen if the list of applicable gem types differ from the gem types available during gem generation.")
                            .LogAsWarning(targetGem);
                    }

                    // gems detected in vertical matches shall be checked for other possible
                    // matches horizontal to them

                    // the 2 gems to the left of the target gem, where applicable
                    if (targetGem.ColOnBoard >= 2)
                    {
                        Gem leftOfTargetGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard - 1];
                        Gem leftOfLeftGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard - 2];

                        if (targetGem.GemType == leftOfTargetGem.GemType && targetGem.GemType == leftOfLeftGem.GemType)
                        {
                            // the current colour of the left gems shall not be the one we want to switch to
                            if (!applicableGemTypes.Remove(leftOfTargetGem.GemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {leftOfTargetGem.GemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = leftOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(leftOfTargetGem.GemType));
                                $"Vertical matching: Found out that the targeted gem cannot be {colouredGemText} because the 2 gems to the left are {colouredGemText}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    // the 2 gems to the right of the target gem, where applicable
                    if (targetGem.ColOnBoard < gemBoard.Columns - 2)
                    {
                        Gem rightOfTargetGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard + 1];
                        Gem rightOfRightGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard + 2];

                        if (targetGem.GemType == rightOfTargetGem.GemType && targetGem.GemType == rightOfRightGem.GemType)
                        {
                            // the current colour of the right gems shall not be the one we want switch to
                            if (!applicableGemTypes.Remove(rightOfTargetGem.GemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {rightOfTargetGem.GemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = rightOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(rightOfTargetGem.GemType));
                                $"Vertical matching: Found out that the targeted gem cannot be {colouredGemText} because the 2 gems to the right are {colouredGemText}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    // the gem to the left and the gem to the right, where applicable
                    if (targetGem.ColOnBoard >= 1 && targetGem.ColOnBoard < gemBoard.Columns - 1)
                    {
                        Gem leftOfTargetGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard - 1];
                        Gem rightOfTargetGem = gemBoard[targetGem.RowOnBoard, targetGem.ColOnBoard + 1];

                        if (targetGem.GemType == leftOfTargetGem.GemType && targetGem.GemType == rightOfTargetGem.GemType)
                        {
                            // the current colour of the gems left and right shall not be the one we want to switch to
                            if (!applicableGemTypes.Remove(rightOfTargetGem.GemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {rightOfTargetGem.GemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = rightOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(rightOfTargetGem.GemType));
                                $"Vertical matching: Found out that the targeted gem cannot be {colouredGemText} because the gem to the right and left are {colouredGemText}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (applicableGemTypes.Count >= 1)
                    {
                        // randomly choose a gem type for the gem to use
                        GemTypes chosenGemType = applicableGemTypes[Random.Range(0, applicableGemTypes.Count)];

                        // log to console so we know what changed
                        string originalColourString = $"{targetGem.GemType}".Color(GemUtils.GetColorBasedOnGemType(targetGem.GemType));
                        string newColourString = $"{chosenGemType}".Color(GemUtils.GetColorBasedOnGemType(chosenGemType));

                        $"Vertical matching: The gem at ({targetGem.RowOnBoard}, {targetGem.ColOnBoard}) will be changed from {originalColourString} to {newColourString}"
                            .Log(targetGem);

                        targetGem.GemType = chosenGemType;
                        //targetGem.UpdateGemColor();
                    }
                    else
                    {
                        // we don't have anything we can choose to stop the match from occurring
                        // for simplicity's sake, we don't alter the gem and just log this
                        ($"Vertical matching: Unable to find a suitable gem type to change to for the gem at ({targetGem.RowOnBoard}, {targetGem.ColOnBoard})!\n"
                            + $"This can happen if we exhaust the list of applicable gem types for the gem to change to.")
                            .LogAsWarning(targetGem);
                    }
                }
            }
        }
    }

    public static bool AreGemsNeighbours(Gem first, Gem second)
    {
        // two gems are considered neighbours if:
        // 1. Either both gems are on the same row, but one on a column to the right or the left of the other gem;
        // 2. Or both gems are on the same column, but one on a row above or below the other gem.

        // since we are finding out if the gems are beside each other, it means either their row or column values must differ by 1
        bool isVerticalNeighbour = first.ColOnBoard == second.ColOnBoard && (Mathf.Abs(first.RowOnBoard - second.RowOnBoard) == 1);
        bool isHorizontalNeighbour = first.RowOnBoard == second.RowOnBoard && (Mathf.Abs(first.ColOnBoard - second.ColOnBoard) == 1);

        return isVerticalNeighbour || isHorizontalNeighbour;
    }

    public static void GenerateGemsForBoard(GemBoard gemBoard, GemTypes[] gemTypesToUse)
    {
        for (int row = 0; row < gemBoard.Rows; row++)
        {
            for (int col = 0; col < gemBoard.Columns; col++)
            {
                var gemTypeToUse = gemTypesToUse[Random.Range(0, gemTypesToUse.Length)];

                var createdGem = new Gem
                {
                    RowOnBoard = row,
                    ColOnBoard = col,
                    GemType = gemTypeToUse
                };

                gemBoard[row, col] = createdGem;
            }
        }
    }
}
