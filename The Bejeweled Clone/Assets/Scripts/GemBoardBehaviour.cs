using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GemBoardBehaviour : MonoBehaviour
{
    public GemBoard gemBoard = new GemBoard(8, 8);

    public GemBehaviour gemPrefab;
    public GameObject gemSelectionIndicator;

    public GemBehaviour previouslyClickedGem;

    public bool isSwappingAllowed = true;
    public GemBehaviour clickedGem;

    // the default for this field is to use all of the gem types
    public GemTypes[] gemTypesToUse = (GemTypes[])System.Enum.GetValues(typeof(GemTypes));

    public Transform gemSpawnArea;

    public GameObject matchIndicatorPrefab;
    public float matchShowDuration = 8f;
    public GemMovesAvailableChecker availableMovesChecker;

    public GameObject noMoreMovesDisplay;
    public GemMatchesTracker matchesTracker;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGemsForBoard();
        EnsureNoMatches();

        availableMovesChecker.UpdateMovesAvailableCounter(gemBoard);

        if (availableMovesChecker.GetNumberOfMatchesAvailable(gemBoard) <= 0)
        {
            noMoreMovesDisplay.SetActive(true);
            isSwappingAllowed = false;
        }
    }

    private void GenerateGemsForBoard()
    {
        for (int currentRow = 0; currentRow < gemBoard.Rows; currentRow++)
        {
            for (int currentCol = 0; currentCol < gemBoard.Columns; currentCol++)
            {
                GemBehaviour createdGem = CreateGemForRowAndCol(gemPrefab, currentRow, currentCol, gemTypesToUse[Random.Range(0, gemTypesToUse.Length)]);

                gemBoard[currentRow, currentCol] = new Gem
                {
                    GemBehaviour = createdGem
                };
            }
        }
    }

    // Ensures that the board will not have 3 or more adjacent gems
    private void EnsureNoMatches()
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

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGemClicked(GemBehaviour clickedGem)
    {
        // do nothing if swapping isn't allowed
        if (!isSwappingAllowed)
        {
            return;
        }

        // since we want to process matches outside the scope of this function
        this.clickedGem = clickedGem;

        // do we already have a gem selected?
        if (previouslyClickedGem)
        {
            // check to see if swapping can be done

            // if the same gem is clicked as the previous...
            if (clickedGem == previouslyClickedGem)
            {
                // deselect gem
                previouslyClickedGem = null;
                gemSelectionIndicator.SetActive(false);
                return;
            }

            if (AreGemsNeighbours(previouslyClickedGem, clickedGem))
            {
                // perform the swap
                Debug.Log($"Swap to be performed for gems at ({clickedGem.rowOnBoard}, {clickedGem.colOnBoard}) and ({previouslyClickedGem.rowOnBoard}, {previouslyClickedGem.colOnBoard})");

                // do not allow other gems to be swapped while one is happening
                isSwappingAllowed = false;

                // swap gem instances on the board
                SwapGems(clickedGem.gem, previouslyClickedGem.gem);

                // update positions of the gem so that it appears as swapped
                Vector3 clickedGemOriginalPosition = clickedGem.transform.position;
                clickedGem.transform.DOMove(previouslyClickedGem.transform.position, 0.5f).OnComplete(OnSwappingComplete);
                previouslyClickedGem.transform.DOMove(clickedGemOriginalPosition, 0.5f);

                // all gems to be deselected after the swap
                gemSelectionIndicator.SetActive(false);
            }
            else
            {
                // the new gem we clicked on will be selected
                previouslyClickedGem = clickedGem;
                gemSelectionIndicator.transform.position = previouslyClickedGem.transform.position;
                gemSelectionIndicator.transform.Translate(0f, 0f, -1f, Space.World);
            }
        }
        else
        {
            previouslyClickedGem = clickedGem;

            // display the gem selection indicator at that gem's position
            gemSelectionIndicator.SetActive(true);
            gemSelectionIndicator.transform.position = clickedGem.transform.position;
            gemSelectionIndicator.transform.Translate(0f, 0f, -1f, Space.World);
        }
    }

    public bool CheckForMatch2(GemBehaviour gem)
    {
        // our list will initially contain the target gem so that it can be
        // marked as matched if we do find a match
        List<GemBehaviour> verticalGems = new List<GemBehaviour> { gem };
        List<GemBehaviour> horizontalGems = new List<GemBehaviour> { gem };

        int numVerticallyMatchedGems = 1;
        int numHorizontallyMatchedGems = 1;

        // start checking vertically first
        // from the current gem upwards
        for (int currentRow = gem.rowOnBoard; currentRow > 0; currentRow--)
        {
            if (gemBoard[currentRow - 1, gem.colOnBoard].gemType == gem.gemType)
            {
                // gem above same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gemBoard[currentRow - 1, gem.colOnBoard]);
            }
            else
            {
                // gem above different as current
                break;
            }
        }

        // now from the current gem downwards
        for (int currentRow = gem.rowOnBoard; currentRow < 8 - 1; currentRow++)
        {
            if (gemBoard[currentRow + 1, gem.colOnBoard].gemType == gem.gemType)
            {
                // gem below same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gemBoard[currentRow + 1, gem.colOnBoard]);
            }
            else
            {
                // gem below different as current
                break;
            }
        }

        // now to check horizontal
        // from the current gem and going towards the left
        for (int currentCol = gem.colOnBoard; currentCol > 0; currentCol--)
        {
            if (gemBoard[gem.rowOnBoard, currentCol - 1].gemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gemBoard[gem.rowOnBoard, currentCol - 1]);
            }
            else
            {
                // gem to the left different from current
                break;
            }
        }

        // from the current gem and going towards the right
        for (int currentCol = gem.colOnBoard; currentCol < 8 - 1; currentCol++)
        {
            if (gemBoard[gem.rowOnBoard, currentCol + 1].gemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gemBoard[gem.rowOnBoard, currentCol + 1]);
            }
            else
            {
                // gem to the left different from current
                break;
            }
        }

        // gems in vertical match
        if (numVerticallyMatchedGems >= 3)
        {
            foreach (var g in verticalGems)
            {
                g.hasBeenMatched = true;
            }
        }

        // gems in horizontal match
        if (numHorizontallyMatchedGems >= 3)
        {
            foreach (var g in horizontalGems)
            {
                g.hasBeenMatched = true;
            }
        }

        return numHorizontallyMatchedGems >= 3 || numVerticallyMatchedGems >= 3;
    }

    public void OnSwappingComplete()
    {
        StartCoroutine(OnSwappingCompleteRoutine());
    }

    public IEnumerator OnSwappingCompleteRoutine()
    {
        bool isCascading = false;
        bool hasMatchAvailable = false;
        bool hasMatchedBefore = false;

        do
        {
            hasMatchAvailable = false;
            foreach (var gem in gemBoard)
            {
                bool currentGemHasMatch = CheckForMatch2(gem);
                hasMatchAvailable = currentGemHasMatch || hasMatchAvailable;
            }

            hasMatchedBefore = hasMatchedBefore || hasMatchAvailable;

            if (hasMatchAvailable)
            {
                // all gems in a match are to be added to a counter
                foreach (var gem in gemBoard)
                {
                    if (gem.hasBeenMatched)
                    {
                        matchesTracker.SetMatchCount(matchesTracker.numMatchesMade + 1);
                    }
                }

                // shrink matched gems (animation)
                yield return ShrinkMatchedGemsRoutine(isCascading);

                // destroy and clear matched gems (waits for one frame to let Unity clean it up)
                yield return DestroyMatchedGemsRoutine();

                // move gem instances downward if required, ensuring that the gaps are all above
                // to do that, we bubble sort each column
                for (int currentCol = 0; currentCol < gemBoard.Columns; currentCol++)
                {
                    while (!IsEmptySpacesInGemBoardColumnAllUp(gemBoard, currentCol))
                    {
                        // start at the top of the column, then keep going down
                        // the -1 is essential to prevent us from going out of bounds when
                        // looking at the gem BELOW the current
                        for (int currentRow = 0; currentRow < gemBoard.Rows - 1; currentRow++)
                        {
                            // is the current gem NOT a space and the gem below a space?
                            if (gemBoard[currentRow, currentCol].GemBehaviour && !gemBoard[currentRow + 1, currentCol].GemBehaviour)
                            {
                                // swap gem instances
                                SwapGems(gemBoard[currentRow, currentCol], gemBoard[currentRow + 1, currentCol]);
                            }
                        }
                    }
                }

                // generate new gems at the blank spots
                List<GemBehaviour> generatedGems = new List<GemBehaviour>();
                for (int currentCol = 0; currentCol < gemBoard.Columns; currentCol++)
                {
                    int gemsToFillThisRow = 0;

                    for (int currentRow = gemBoard.Rows - 1; currentRow >= 0; currentRow--)
                    {
                        if (!gemBoard[currentRow, currentCol].GemBehaviour)
                        {
                            GemBehaviour newGem = CreateGemForRowAndCol(gemPrefab, currentRow, currentCol, gemTypesToUse[Random.Range(0, gemTypesToUse.Length)]);

                            gemBoard[currentRow, currentCol].GemBehaviour = newGem;
                            generatedGems.Add(newGem);

                            gemsToFillThisRow++;

                            Vector3 newGemPosition = newGem.transform.position;
                            newGemPosition.y = gemSpawnArea.position.y + (1.1f * (gemsToFillThisRow - 1));
                            newGem.transform.position = newGemPosition;
                        }
                    }
                }

                foreach (var gem in gemBoard)
                {
                    gem.GemBehaviour.fallDestination = ComputeGemPositionViaRowAndCol(gem.rowOnBoard, gem.colOnBoard);
                    gem.GemBehaviour.isFalling = true;
                }

                // wait until all the gems fall into place, then continue
                yield return new WaitUntil(() => generatedGems.All(gem => !gem.isFalling));
            }
            else if (!hasMatchedBefore)
            {
                // an invalid match has been made, so we have to swap the 2 gems back
                SwapGems(clickedGem.gem, previouslyClickedGem.gem);

                // animate swapping back
                yield return SwapGemsBack(clickedGem, previouslyClickedGem);
            }

            isCascading = true;
        }
        while (hasMatchAvailable);

        // check for available moves
        availableMovesChecker.UpdateMovesAvailableCounter(gemBoard);

        if (availableMovesChecker.GetNumberOfMatchesAvailable(gemBoard) <= 0)
        {
            noMoreMovesDisplay.SetActive(true);
            yield break;
        }

        // allow player's next turn
        clickedGem = null;
        previouslyClickedGem = null;
        isSwappingAllowed = true;
    }

    // Used to test the IsEmptySpacesInGemBoardColumnAllUp method.
    [ContextMenu("Check all columns")]
    public void CheckAllColumns()
    {
        for (int i = 0; i < gemBoard.Columns; i++)
        {
            Debug.Log($"Column {i}: {IsEmptySpacesInGemBoardColumnAllUp(gemBoard, i)}");
        }
    }

    // Returns true if all the empty spaces in the column are above all gems.
    // False if otherwise.
    public bool IsEmptySpacesInGemBoardColumnAllUp(GemBoard gemBoard, int columnIndex)
    {
        // is we encounter gems, we shall not encounter blank spaces
        // if we encounter blank spaces, keep going until we encounter gems,
        // then check as what is stated above
        bool hasEncounteredGems = false;

        for (int i = 0; i < gemBoard.Rows; i++)
        {
            Gem gem = gemBoard[i, columnIndex];

            if (gem.GemBehaviour)
            {
                hasEncounteredGems = true;
            }

            if (hasEncounteredGems)
            {
                // the column is invalid if we encounter blank spaces
                // after finding gems above
                if (!gem.GemBehaviour)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool AreGemsNeighbours(GemBehaviour first, GemBehaviour second)
    {
        // two gems are considered neighbours if:
        // 1. Either both gems are on the same row, but one on a column to the right or the left of the other gem;
        // 2. Or both gems are on the same column, but one on a row above or below the other gem.

        // since we are finding out if the gems are beside each other, it means either their row or column values must differ by 1
        bool isVerticalNeighbour = first.colOnBoard == second.colOnBoard && (Mathf.Abs(first.rowOnBoard - second.rowOnBoard) == 1);
        bool isHorizontalNeighbour = first.rowOnBoard == second.rowOnBoard && (Mathf.Abs(first.colOnBoard - second.colOnBoard) == 1);

        return isVerticalNeighbour || isHorizontalNeighbour;
    }

    private Vector3 ComputeGemPositionViaRowAndCol(int gemRow, int gemCol)
        => new Vector3(gemCol + (0.1f * gemCol), -(gemRow + (0.1f * gemRow)));

    private void SwapGems(Gem first, Gem second)
    {
        // this swaps the gem instances on the board
        // and updates the row and column values on the
        // gems themselves

        // swap gem object instances on the board
        gemBoard[first.rowOnBoard, first.colOnBoard] = second;
        gemBoard[second.rowOnBoard, second.colOnBoard] = first;

        // store first gem's row and column values so we don't lose
        // the original values when we change it to the second's
        int initialFirstGemRow = first.rowOnBoard;
        int initialFirstGemCol = first.colOnBoard;

        // update row and column values for both gems
        first.rowOnBoard = second.rowOnBoard;
        first.colOnBoard = second.colOnBoard;
        second.rowOnBoard = initialFirstGemRow;
        second.colOnBoard = initialFirstGemCol;
    }

    private GemBehaviour CreateGemForRowAndCol(GemBehaviour gemPrefab, int row, int col, GemTypes gemType)
    {
        GemBehaviour gemInstance = Instantiate(gemPrefab);
        Gem gemObject = new Gem
        {
            GemBehaviour = gemInstance
        };

        gemInstance.gem = gemObject;

        gemInstance.rowOnBoard = row;
        gemInstance.colOnBoard = col;
        gemInstance.gemType = gemType;
        gemInstance.gemBoard = this;

        gemInstance.transform.position = ComputeGemPositionViaRowAndCol(row, col);
        gemInstance.transform.rotation = Quaternion.identity;
        gemInstance.transform.SetParent(transform, true);

        return gemInstance;
    }

    private int GetNumberOfMatchesAvailable(GemBoard gemBoard)
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
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol]);
                    numMatchesFound++;
                }

                // X ?
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol + 1]);
                    numMatchesFound++;
                }

                // ? X
                // X ?
                // X ?
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol]);
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol + 1]);
                    numMatchesFound++;
                }

                // X ?
                // ? X
                // X ?
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 2, currentCol].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol]);
                    numMatchesFound++;
                }

                // ? X
                // X ?
                // ? X
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 2, currentCol + 1].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol + 1]);
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
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 2]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    numMatchesFound++;
                }

                // X X ?
                // ? ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 1].gemType
                    && gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 2]);
                    numMatchesFound++;
                }

                // X ? ?
                // ? X X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType
                    && gemBoard[currentRow + 1, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 2]);
                    numMatchesFound++;
                }

                // ? X X
                // X ? ?
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow, currentCol + 2].gemType
                    && gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow + 1, currentCol].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 2]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    numMatchesFound++;
                }

                // X ? X
                // ? X ?
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 2].gemType
                    && gemBoard[currentRow, currentCol + 2].gemType == gemBoard[currentRow + 1, currentCol + 1].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 2]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 1]);
                    numMatchesFound++;
                }

                // ? X ?
                // X ? X
                if (gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow + 1, currentCol].gemType
                    && gemBoard[currentRow + 1, currentCol].gemType == gemBoard[currentRow + 1, currentCol + 2].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol + 2]);
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
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 1, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 3, currentCol]);
                    numMatchesFound++;
                }

                // X
                // ?
                // X
                // X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow + 2, currentCol].gemType
                    && gemBoard[currentRow + 2, currentCol].gemType == gemBoard[currentRow + 3, currentCol].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 2, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow + 3, currentCol]);
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
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 2]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 3]);
                    numMatchesFound++;
                }

                // X X ? X
                if (gemBoard[currentRow, currentCol].gemType == gemBoard[currentRow, currentCol + 1].gemType
                    && gemBoard[currentRow, currentCol + 1].gemType == gemBoard[currentRow, currentCol + 3].gemType)
                {
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 1]);
                    DisplayMatchIndicationAtGem(gemBoard[currentRow, currentCol + 3]);
                    numMatchesFound++;
                }
            }
        }

        return numMatchesFound;
    }

    private IEnumerator ShrinkMatchedGemsRoutine(bool isCascading)
    {
        int numGemsToShrink = 0;

        foreach (GemBehaviour gem in gemBoard)
        {
            if (gem.hasBeenMatched)
            {
                numGemsToShrink++;

                var sequence = DOTween.Sequence();

                if (isCascading)
                {
                    sequence.Append(gem.gemSpriteGameObject.transform.DOScale(Vector3.one * 1.2f, 0.4f))
                            .Append(gem.gemSpriteGameObject.transform.DOScale(Vector3.zero, 0.3f))
                            .OnComplete(() => numGemsToShrink--);
                }
                else
                {
                    sequence.Append(gem.gemSpriteGameObject.transform.DOScale(Vector3.one * 1.15f, 0.15f))
                            .Append(gem.gemSpriteGameObject.transform.DOScale(Vector3.zero, 0.3f))
                            .OnComplete(() => numGemsToShrink--);
                }
            }
        }

        yield return new WaitUntil(() => numGemsToShrink <= 0);
    }

    private IEnumerator DestroyMatchedGemsRoutine()
    {
        for (int i = 0; i < gemBoard.Rows; i++)
        {
            for (int j = 0; j < gemBoard.Columns; j++)
            {
                if (gemBoard[i, j].hasBeenMatched)
                {
                    Destroy(gemBoard[i, j].GemBehaviour.gameObject);
                }
            }
        }

        // wait for a frame so as to let Unity clean up the destroyed gems
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator MoveFallingGemsDown()
    {
        int gemsLeft = 0;

        foreach (GemBehaviour gem in gemBoard)
        {
            if (gem && gem.transform.position != ComputeGemPositionViaRowAndCol(gem.rowOnBoard, gem.colOnBoard))
            {
                gemsLeft++;
                gem.transform.DOMove(ComputeGemPositionViaRowAndCol(gem.rowOnBoard, gem.colOnBoard), 0.75f)
                             .OnComplete(() => gemsLeft--);
            }
        }

        yield return new WaitUntil(() => gemsLeft <= 0);
    }

    private IEnumerator GrowGemsAtBlankSpots(List<GemBehaviour> gemsToGrow)
    {
        int numGemsToGrow = gemsToGrow.Count;

        foreach (var gem in gemsToGrow)
        {
            gem.transform.DOScale(Vector3.zero, 0.75f).From()
                         .OnComplete(() => numGemsToGrow--);
        }

        yield return new WaitUntil(() => numGemsToGrow <= 0);
    }

    private IEnumerator SwapGemsBack(GemBehaviour first, GemBehaviour second)
    {
        Vector3 firstGemPosition = first.transform.position;
        Vector3 secondGemPosition = second.transform.position;

        var sequence = DOTween.Sequence()
                              .Insert(0f, first.transform.DOMove(secondGemPosition, 0.5f))
                              .Insert(0f, second.transform.DOMove(firstGemPosition, 0.5f));

        yield return sequence.WaitForCompletion();
    }

    private void DisplayMatchIndicationAtGem(GemBehaviour gem)
    {
        GameObject matchIndicatorInstance = Instantiate(matchIndicatorPrefab, gem.transform.position, Quaternion.identity);
        Destroy(matchIndicatorInstance, matchShowDuration);
    }

    [ContextMenu("Reset Gem Types To Use")]
    private void SetGemTypesToUseToDefault()
    {
        Debug.Log("Successfully reset the gems types to use on the board!", this);
        gemTypesToUse = (GemTypes[])System.Enum.GetValues(typeof(GemTypes));
    }

    [ContextMenu("Regenerate Gem Board")]
    private void RegenerateGemBoard()
    {
        // to remove every single gem on the board
        for (int i = 0; i < gemBoard.Rows; i++)
        {
            for (int j = 0; j < gemBoard.Columns; j++)
            {
                Destroy(gemBoard[i, j].GemBehaviour.gameObject);
            }
        }

        GenerateGemsForBoard();
    }

    [ContextMenu("Print Board Representation to Console")]
    private void PrintBoardRepresentationToConsole()
    {
        string representation = "Board representation:\n";

        for (int currentRow = 0; currentRow < gemBoard.Rows; currentRow++)
        {
            for (int currentCol = 0; currentCol < gemBoard.Columns; currentCol++)
            {
                GemBehaviour currentGem = gemBoard[currentRow, currentCol];
                Color characterColor = GemUtils.GetColorBasedOnGemType(currentGem.gemType);
                representation += "■".Color(characterColor);
            }

            representation += "\n";
        }

        Debug.Log(representation, this);
    }

    [ContextMenu("Log Number of Possible Matches")]
    private void LogNumberOfPossibleMatches()
    {
        Debug.Log(GetNumberOfMatchesAvailable(gemBoard));
    }
}
