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
        GemBoardUtils.EnsureNoMatches(gemBoard, gemTypesToUse);

        availableMovesChecker.UpdateMovesAvailableCounter(gemBoard);

        if (GemBoardUtils.GetNumberOfMovesAvailable(gemBoard) <= 0)
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
                GemTypes gemTypeToUse = gemTypesToUse[Random.Range(0, gemTypesToUse.Length)];
                GemBehaviour createdGem = CreateGemForRowAndCol(gemPrefab, currentRow, currentCol, gemTypesToUse[Random.Range(0, gemTypesToUse.Length)]);

                gemBoard[currentRow, currentCol] = new Gem
                {
                    GemType = gemTypeToUse,
                    RowOnBoard = currentRow,
                    ColOnBoard = currentCol,

                    GemBehaviour = createdGem
                };
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
                Debug.Log($"Swap to be performed for gems at ({clickedGem.gem.RowOnBoard}, {clickedGem.gem.ColOnBoard}) and ({previouslyClickedGem.gem.RowOnBoard}, {previouslyClickedGem.gem.ColOnBoard})");

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

    public bool CheckForMatch(GemBehaviour gem)
    {
        // our list will initially contain the target gem so that it can be
        // marked as matched if we do find a match
        List<GemBehaviour> verticalGems = new List<GemBehaviour> { gem };
        List<GemBehaviour> horizontalGems = new List<GemBehaviour> { gem };

        int numVerticallyMatchedGems = 1;
        int numHorizontallyMatchedGems = 1;

        // start checking vertically first
        // from the current gem upwards
        for (int currentRow = gem.gem.RowOnBoard; currentRow > 0; currentRow--)
        {
            if (gemBoard[currentRow - 1, gem.gem.ColOnBoard].GemType == gem.gemType)
            {
                // gem above same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gemBoard[currentRow - 1, gem.gem.ColOnBoard]);
            }
            else
            {
                // gem above different as current
                break;
            }
        }

        // now from the current gem downwards
        for (int currentRow = gem.gem.RowOnBoard; currentRow < 8 - 1; currentRow++)
        {
            if (gemBoard[currentRow + 1, gem.gem.ColOnBoard].GemType == gem.gemType)
            {
                // gem below same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gemBoard[currentRow + 1, gem.gem.ColOnBoard]);
            }
            else
            {
                // gem below different as current
                break;
            }
        }

        // now to check horizontal
        // from the current gem and going towards the left
        for (int currentCol = gem.gem.ColOnBoard; currentCol > 0; currentCol--)
        {
            if (gemBoard[gem.gem.RowOnBoard, currentCol - 1].GemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gemBoard[gem.gem.RowOnBoard, currentCol - 1]);
            }
            else
            {
                // gem to the left different from current
                break;
            }
        }

        // from the current gem and going towards the right
        for (int currentCol = gem.gem.ColOnBoard; currentCol < 8 - 1; currentCol++)
        {
            if (gemBoard[gem.gem.RowOnBoard, currentCol + 1].GemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gemBoard[gem.gem.RowOnBoard, currentCol + 1]);
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
                bool currentGemHasMatch = CheckForMatch(gem);
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
                    gem.GemBehaviour.fallDestination = ComputeGemPositionViaRowAndCol(gem.RowOnBoard, gem.ColOnBoard);
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

        if (GemBoardUtils.GetNumberOfMovesAvailable(gemBoard) <= 0)
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
        bool isVerticalNeighbour = first.gem.ColOnBoard == second.gem.ColOnBoard && (Mathf.Abs(first.gem.RowOnBoard - second.gem.RowOnBoard) == 1);
        bool isHorizontalNeighbour = first.gem.RowOnBoard == second.gem.RowOnBoard && (Mathf.Abs(first.gem.ColOnBoard - second.gem.ColOnBoard) == 1);

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
        gemBoard[first.RowOnBoard, first.ColOnBoard] = second;
        gemBoard[second.RowOnBoard, second.ColOnBoard] = first;

        // store first gem's row and column values so we don't lose
        // the original values when we change it to the second's
        int initialFirstGemRow = first.RowOnBoard;
        int initialFirstGemCol = first.ColOnBoard;

        // update row and column values for both gems
        first.RowOnBoard = second.RowOnBoard;
        first.ColOnBoard = second.ColOnBoard;
        second.RowOnBoard = initialFirstGemRow;
        second.ColOnBoard = initialFirstGemCol;
    }

    private GemBehaviour CreateGemForRowAndCol(GemBehaviour gemPrefab, int row, int col, GemTypes gemType)
    {
        GemBehaviour gemInstance = Instantiate(gemPrefab);
        Gem gemObject = new Gem
        {
            GemBehaviour = gemInstance
        };

        gemInstance.gem = gemObject;

        gemInstance.gem.RowOnBoard = row;
        gemInstance.gem.ColOnBoard = col;
        gemInstance.gemType = gemType;
        gemInstance.gemBoard = this;

        gemInstance.transform.position = ComputeGemPositionViaRowAndCol(row, col);
        gemInstance.transform.rotation = Quaternion.identity;
        gemInstance.transform.SetParent(transform, true);

        return gemInstance;
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
            if (gem && gem.transform.position != ComputeGemPositionViaRowAndCol(gem.gem.RowOnBoard, gem.gem.ColOnBoard))
            {
                gemsLeft++;
                gem.transform.DOMove(ComputeGemPositionViaRowAndCol(gem.gem.RowOnBoard, gem.gem.ColOnBoard), 0.75f)
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
        Debug.Log(GemBoardUtils.GetNumberOfMovesAvailable(gemBoard));
    }
}
