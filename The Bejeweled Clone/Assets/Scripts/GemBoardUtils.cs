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

    // Ensures that the board will not have 3 or more adjacent gems
    public static void EnsureNoMatches(GemBoard gemBoard, GemTypes[] gemTypesToUse)
    {
        // we'll start off with finding matches horizontally
        for (int currentRow = 0; currentRow < gemBoard.Rows; currentRow++)
        {
            for (int currentCol = 0; currentCol < gemBoard.Columns - 2; currentCol++)
            {
                GemBehaviour currentGem = gemBoard[currentRow, currentCol];
                GemBehaviour rightOfCurrentGem = gemBoard[currentRow, currentCol + 1];
                GemBehaviour rightOfRightGem = gemBoard[currentRow, currentCol + 2];

                if (currentGem.gemType == rightOfCurrentGem.gemType && rightOfCurrentGem.gemType == rightOfRightGem.gemType)
                {
                    // since we're starting off with no randomness,
                    // we'll choose on the gem to the right of the current
                    // that will be removed
                    GemBehaviour targetGem = rightOfCurrentGem;

                    // a list of gem types that the gem can become
                    // this will be changed as we look around
                    List<GemTypes> applicableGemTypes = gemTypesToUse.ToList();

                    // the current gem type shall be removed from the list
                    // since this has been the type we've been avoiding all the while
                    // we also make use of the return value to log us additional messages
                    // if things go wrong
                    if (!applicableGemTypes.Remove(targetGem.gemType))
                    {
                        ($"Horizontal matching: Unable to remove the gem type of the target gem itself ({targetGem.gemType})!\n"
                            + $"This may happen if the list of applicable gem types differ from the gem types available during gem generation.")
                            .Log(targetGem);
                    }

                    // the gems detected in horizontal matches shall only check upwards and downwards

                    if (rightOfCurrentGem.rowOnBoard >= 2)
                    {
                        GemBehaviour aboveTargeted = gemBoard[targetGem.rowOnBoard - 1, targetGem.colOnBoard];
                        GemBehaviour aboveAboveTargeted = gemBoard[targetGem.rowOnBoard - 2, targetGem.colOnBoard];

                        if (aboveTargeted.gemType == aboveAboveTargeted.gemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(aboveTargeted.gemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {aboveTargeted.gemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = aboveTargeted.gemType.ToString().Color(GemUtils.GetColorBasedOnGemType(aboveTargeted.gemType));

                                $"Horizontal matching: Found out that the targeted gem cannot be {colouredGemTypeString} because the two gems above are {colouredGemTypeString}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (rightOfCurrentGem.rowOnBoard < gemBoard.Rows - 2)
                    {
                        GemBehaviour belowTargeted = gemBoard[targetGem.rowOnBoard + 1, targetGem.colOnBoard];
                        GemBehaviour belowBelowTargeted = gemBoard[targetGem.rowOnBoard + 2, targetGem.colOnBoard];

                        if (belowTargeted.gemType == belowBelowTargeted.gemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(belowTargeted.gemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {belowTargeted.gemType} from the list of applicable gem types!"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = belowTargeted.gemType.ToString().Color(GemUtils.GetColorBasedOnGemType(belowTargeted.gemType));

                                $"Horizontal matching: Found out that the targeted gem cannot be {colouredGemTypeString} because the two gems below are {colouredGemTypeString}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    if (rightOfCurrentGem.rowOnBoard >= 1 && rightOfCurrentGem.rowOnBoard < gemBoard.Rows - 1)
                    {
                        // can look up and down by 1 space
                        // if these 2 gems are the same type, a match will occur, so we also need to
                        // consider this situation

                        GemBehaviour aboveTargeted = gemBoard[targetGem.rowOnBoard - 1, targetGem.colOnBoard];
                        GemBehaviour belowTargeted = gemBoard[targetGem.rowOnBoard + 1, targetGem.colOnBoard];

                        if (aboveTargeted.gemType == belowTargeted.gemType)
                        {
                            // this is not the gem colour we want to change to
                            // placed in conditional so we can log if things go wrong
                            if (!applicableGemTypes.Remove(belowTargeted.gemType))
                            {
                                ($"Horizontal matching: Unable to remove the gem type {aboveTargeted.gemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemTypeString = aboveTargeted.gemType.ToString().Color(GemUtils.GetColorBasedOnGemType(aboveTargeted.gemType));

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
                        string originalColourString = $"{targetGem.gemType}".Color(GemUtils.GetColorBasedOnGemType(targetGem.gemType));
                        string newColourString = $"{chosenGemType}".Color(GemUtils.GetColorBasedOnGemType(chosenGemType));

                        $"Horizontal matching: The gem at ({targetGem.rowOnBoard}, {targetGem.colOnBoard}) will be changed from {originalColourString} to {newColourString}"
                            .Log(targetGem);

                        targetGem.gemType = chosenGemType;
                        targetGem.UpdateGemColor();
                    }
                    else
                    {
                        // we don't have anything we can choose to stop the match from occurring
                        // for simplicity's sake, we don't alter the gem and just log this
                        ($"Horizontal matching: Unable to find a suitable gem type to change to for the gem at ({targetGem.rowOnBoard}, {targetGem.colOnBoard})!\n"
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
                GemBehaviour currentGem = gemBoard[currentRow, currentCol];
                GemBehaviour bottomOfCurrentGem = gemBoard[currentRow + 1, currentCol];
                GemBehaviour bottomOfBottomGem = gemBoard[currentRow + 2, currentCol];

                if (currentGem.gemType == bottomOfCurrentGem.gemType && currentGem.gemType == bottomOfBottomGem.gemType)
                {
                    // same as above, we'll chose the gem just 1 space below the current
                    GemBehaviour targetGem = bottomOfCurrentGem;

                    // this is a list of gem types that the gem can become
                    // note that this list will be changed as we progress
                    List<GemTypes> applicableGemTypes = gemTypesToUse.ToList();

                    // remove the current gem type from the list, since that
                    // type is what we want to switch away from
                    // we put this in a conditional so we will get alerted if we're
                    // unable to remove the gem type from the list
                    if (!applicableGemTypes.Remove(targetGem.gemType))
                    {
                        ($"Vertical matching: Unable to remove the gem type of the target gem itself ({targetGem.gemType})!\n"
                            + $"This may happen if the list of applicable gem types differ from the gem types available during gem generation.")
                            .LogAsWarning(targetGem);
                    }

                    // gems detected in vertical matches shall be checked for other possible
                    // matches horizontal to them

                    // the 2 gems to the left of the target gem, where applicable
                    if (targetGem.colOnBoard >= 2)
                    {
                        GemBehaviour leftOfTargetGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard - 1];
                        GemBehaviour leftOfLeftGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard - 2];

                        if (targetGem.gemType == leftOfTargetGem.gemType && targetGem.gemType == leftOfLeftGem.gemType)
                        {
                            // the current colour of the left gems shall not be the one we want to switch to
                            if (!applicableGemTypes.Remove(leftOfTargetGem.gemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {leftOfTargetGem.gemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = leftOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(leftOfTargetGem.gemType));
                                $"Vertical matching: Found out that the targeted gem cannot be {colouredGemText} because the 2 gems to the left are {colouredGemText}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    // the 2 gems to the right of the target gem, where applicable
                    if (targetGem.colOnBoard < gemBoard.Columns - 2)
                    {
                        GemBehaviour rightOfTargetGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard + 1];
                        GemBehaviour rightOfRightGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard + 2];

                        if (targetGem.gemType == rightOfTargetGem.gemType && targetGem.gemType == rightOfRightGem.gemType)
                        {
                            // the current colour of the right gems shall not be the one we want switch to
                            if (!applicableGemTypes.Remove(rightOfTargetGem.gemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {rightOfTargetGem.gemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = rightOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(rightOfTargetGem.gemType));
                                $"Vertical matching: Found out that the targeted gem cannot be {colouredGemText} because the 2 gems to the right are {colouredGemText}"
                                    .Log(targetGem);
                            }
                        }
                    }

                    // the gem to the left and the gem to the right, where applicable
                    if (targetGem.colOnBoard >= 1 && targetGem.colOnBoard < gemBoard.Columns - 1)
                    {
                        GemBehaviour leftOfTargetGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard - 1];
                        GemBehaviour rightOfTargetGem = gemBoard[targetGem.rowOnBoard, targetGem.colOnBoard + 1];

                        if (targetGem.gemType == leftOfTargetGem.gemType && targetGem.gemType == rightOfTargetGem.gemType)
                        {
                            // the current colour of the gems left and right shall not be the one we want to switch to
                            if (!applicableGemTypes.Remove(rightOfTargetGem.gemType))
                            {
                                ($"Vertical matching: Unable to remove the gem type {rightOfTargetGem.gemType} from the list of applicable gem types!\n"
                                    + $"This can happen if the gem type is already removed by an earlier match detection.")
                                    .Log(targetGem);
                            }
                            else
                            {
                                string colouredGemText = rightOfTargetGem.ToString().Color(GemUtils.GetColorBasedOnGemType(rightOfTargetGem.gemType));
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
                        string originalColourString = $"{targetGem.gemType}".Color(GemUtils.GetColorBasedOnGemType(targetGem.gemType));
                        string newColourString = $"{chosenGemType}".Color(GemUtils.GetColorBasedOnGemType(chosenGemType));

                        $"Vertical matching: The gem at ({targetGem.rowOnBoard}, {targetGem.colOnBoard}) will be changed from {originalColourString} to {newColourString}"
                            .Log(targetGem);

                        targetGem.gemType = chosenGemType;
                        targetGem.UpdateGemColor();
                    }
                    else
                    {
                        // we don't have anything we can choose to stop the match from occurring
                        // for simplicity's sake, we don't alter the gem and just log this
                        ($"Vertical matching: Unable to find a suitable gem type to change to for the gem at ({targetGem.rowOnBoard}, {targetGem.colOnBoard})!\n"
                            + $"This can happen if we exhaust the list of applicable gem types for the gem to change to.")
                            .LogAsWarning(targetGem);
                    }
                }
            }
        }
    }
}
