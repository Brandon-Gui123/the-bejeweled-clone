using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GemBoardBehaviour : MonoBehaviour
{
    public Gem[,] gems = new Gem[8, 8];

    public Gem gemPrefab;
    public GameObject gemSelectionIndicator;

    public Gem previouslyClickedGem;

    public bool isSwappingAllowed = true;
    public Gem clickedGem;

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

        availableMovesChecker.UpdateMovesAvailableCounter(gems);

        if (availableMovesChecker.GetNumberOfMatchesAvailable(gems) <= 0)
        {
            noMoreMovesDisplay.SetActive(true);
            isSwappingAllowed = false;
        }
    }

    private void GenerateGemsForBoard()
    {
        for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
        {
            for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
            {
                Gem createdGem = CreateGemForRowAndCol(gemPrefab, currentRow, currentCol, gemTypesToUse[Random.Range(0, gemTypesToUse.Length)]);

                gems[currentRow, currentCol] = createdGem;
            }
        }
    }

    // Ensures that the board will not have 3 or more adjacent gems
    private void EnsureNoMatches()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGemClicked(Gem clickedGem)
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
                SwapGems(clickedGem, previouslyClickedGem);

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

    public bool CheckForMatch(Gem gem)
    {
        bool hasMatchAbove = false;
        bool hasMatchRight = false;
        bool hasMatchBelow = false;
        bool hasMatchLeft = false;

        bool hasMiddleVerticalMatch = false;
        bool hasMiddleHorizontalMatch = false;

        int gemRow = gem.rowOnBoard;
        int gemCol = gem.colOnBoard;

        // check 2 gems above - gemRow must be greater than 1
        if (gemRow > 1)
        {
            hasMatchAbove =
                gems[gemRow - 2, gemCol].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow - 1, gemCol].gemType == gems[gemRow, gemCol].gemType;
        }

        // check 2 gems to the right - gemCol must be less than 6
        if (gemCol < 6)
        {
            hasMatchRight =
                gems[gemRow, gemCol + 2].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow, gemCol + 1].gemType == gems[gemRow, gemCol].gemType;
        }

        // check 2 gems below - gemRow must be less than 6
        if (gemRow < 6)
        {
            hasMatchBelow =
                gems[gemRow + 2, gemCol].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow + 1, gemCol].gemType == gems[gemRow, gemCol].gemType;
        }

        // check 2 gems to the left - gemCol must be greater than 1
        if (gemCol > 1)
        {
            hasMatchLeft =
                gems[gemRow, gemCol - 2].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow, gemCol - 1].gemType == gems[gemRow, gemCol].gemType;
        }

        // check a gem above and a gem below - gemRow must be within 1 to 6
        if (gemRow >= 1 && gemRow <= 6)
        {
            hasMiddleVerticalMatch =
                gems[gemRow - 1, gemCol].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow + 1, gemCol].gemType == gems[gemRow, gemCol].gemType;
        }

        // check a gem to the right and a gem to the left - gemCol must be within 1 to 6
        if (gemCol >= 1 && gemCol <= 6)
        {
            hasMiddleHorizontalMatch =
                gems[gemRow, gemCol - 1].gemType == gems[gemRow, gemCol].gemType
                && gems[gemRow, gemCol + 1].gemType == gems[gemRow, gemCol].gemType;
        }

        if (hasMatchAbove)
        {
            Debug.Log($"Found match above for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow - 1, gemCol].hasBeenMatched = true;
            gems[gemRow - 2, gemCol].hasBeenMatched = true;
        }

        if (hasMatchRight)
        {
            Debug.Log($"Found match to the right for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow, gemCol + 2].hasBeenMatched = true;
            gems[gemRow, gemCol + 1].hasBeenMatched = true;
        }

        if (hasMatchBelow)
        {
            Debug.Log($"Found match below for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow + 2, gemCol].hasBeenMatched = true;
            gems[gemRow + 1, gemCol].hasBeenMatched = true;
        }

        if (hasMatchLeft)
        {
            Debug.Log($"Found match to the left for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow, gemCol - 2].hasBeenMatched = true;
            gems[gemRow, gemCol - 1].hasBeenMatched = true;
        }

        if (hasMiddleVerticalMatch)
        {
            Debug.Log($"Found vertical middle match at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow - 1, gemCol].hasBeenMatched = true;
            gems[gemRow + 1, gemCol].hasBeenMatched = true;
        }

        if (hasMiddleHorizontalMatch)
        {
            Debug.Log($"Found horizontal middle match for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);

            gem.hasBeenMatched = true;
            gems[gemRow, gemCol + 1].hasBeenMatched = true;
            gems[gemRow, gemCol - 1].hasBeenMatched = true;
        }

        return hasMatchAbove || hasMatchBelow || hasMatchLeft || hasMatchRight || hasMiddleHorizontalMatch || hasMiddleVerticalMatch;
    }

    public bool CheckForMatch2(Gem gem)
    {
        // our list will initially contain the target gem so that it can be
        // marked as matched if we do find a match
        List<Gem> verticalGems = new List<Gem> { gem };
        List<Gem> horizontalGems = new List<Gem> { gem };

        int numVerticallyMatchedGems = 1;
        int numHorizontallyMatchedGems = 1;

        // start checking vertically first
        // from the current gem upwards
        for (int currentRow = gem.rowOnBoard; currentRow > 0; currentRow--)
        {
            if (gems[currentRow - 1, gem.colOnBoard].gemType == gem.gemType)
            {
                // gem above same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gems[currentRow - 1, gem.colOnBoard]);
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
            if (gems[currentRow + 1, gem.colOnBoard].gemType == gem.gemType)
            {
                // gem below same as current
                numVerticallyMatchedGems++;
                verticalGems.Add(gems[currentRow + 1, gem.colOnBoard]);
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
            if (gems[gem.rowOnBoard, currentCol - 1].gemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gems[gem.rowOnBoard, currentCol - 1]);
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
            if (gems[gem.rowOnBoard, currentCol + 1].gemType == gem.gemType)
            {
                // gem to the left of current is the same as current
                numHorizontallyMatchedGems++;
                horizontalGems.Add(gems[gem.rowOnBoard, currentCol + 1]);
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
            foreach (var gem in gems)
            {
                bool currentGemHasMatch = CheckForMatch2(gem);
                hasMatchAvailable = currentGemHasMatch || hasMatchAvailable;
            }

            hasMatchedBefore = hasMatchedBefore || hasMatchAvailable;

            if (hasMatchAvailable)
            {
                // all gems in a match are to be added to a counter
                foreach (var gem in gems)
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
                for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
                {
                    while (!IsEmptySpacesInGemBoardColumnAllUp(gems, currentCol))
                    {
                        // start at the top of the column, then keep going down
                        // the -1 is essential to prevent us from going out of bounds when
                        // looking at the gem BELOW the current
                        for (int currentRow = 0; currentRow < gems.GetLength(0) - 1; currentRow++)
                        {
                            // is the current gem NOT a space and the gem below a space?
                            if (gems[currentRow, currentCol] && !gems[currentRow + 1, currentCol])
                            {
                                // swap gem instances
                                SwapGems(gems[currentRow, currentCol], gems[currentRow + 1, currentCol]);
                            }
                        }
                    }
                }

                // generate new gems at the blank spots
                List<Gem> generatedGems = new List<Gem>();
                for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
                {
                    int gemsToFillThisRow = 0;

                    for (int currentRow = gems.GetLength(0) - 1; currentRow >= 0; currentRow--)
                    {
                        if (!gems[currentRow, currentCol])
                        {
                            Gem newGem = CreateGemForRowAndCol(gemPrefab, currentRow, currentCol, gemTypesToUse[Random.Range(0, gemTypesToUse.Length)]);

                            gems[currentRow, currentCol] = newGem;
                            generatedGems.Add(newGem);

                            gemsToFillThisRow++;

                            Vector3 newGemPosition = newGem.transform.position;
                            newGemPosition.y = gemSpawnArea.position.y + (1.1f * (gemsToFillThisRow - 1));
                            newGem.transform.position = newGemPosition;
                        }
                    }
                }

                foreach (var gem in gems)
                {
                    gem.fallDestination = ComputeGemPositionViaRowAndCol(gem.rowOnBoard, gem.colOnBoard);
                    gem.isFalling = true;
                }

                // wait until all the gems fall into place, then continue
                yield return new WaitUntil(() => generatedGems.All(gem => !gem.isFalling));
            }
            else if (!hasMatchedBefore)
            {
                // an invalid match has been made, so we have to swap the 2 gems back
                SwapGems(clickedGem, previouslyClickedGem);

                // animate swapping back
                yield return SwapGemsBack(clickedGem, previouslyClickedGem);
            }

            isCascading = true;
        }
        while (hasMatchAvailable);

        // check for available moves
        availableMovesChecker.UpdateMovesAvailableCounter(gems);

        if (availableMovesChecker.GetNumberOfMatchesAvailable(gems) <= 0)
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
        for (int i = 0; i < gems.GetLength(1); i++)
        {
            Debug.Log($"Column {i}: {IsEmptySpacesInGemBoardColumnAllUp(gems, i)}");
        }
    }

    // Returns true if all the empty spaces in the column are above all gems.
    // False if otherwise.
    public bool IsEmptySpacesInGemBoardColumnAllUp(Gem[,] gemBoard, int columnIndex)
    {
        // is we encounter gems, we shall not encounter blank spaces
        // if we encounter blank spaces, keep going until we encounter gems,
        // then check as what is stated above
        bool hasEncounteredGems = false;

        for (int i = 0; i < gemBoard.GetLength(0); i++)
        {
            Gem gem = gemBoard[i, columnIndex];

            if (gem)
            {
                hasEncounteredGems = true;
            }

            if (hasEncounteredGems)
            {
                // the column is invalid if we encounter blank spaces
                // after finding gems above
                if (!gem)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool AreGemsNeighbours(Gem first, Gem second)
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
        gems[first.rowOnBoard, first.colOnBoard] = second;
        gems[second.rowOnBoard, second.colOnBoard] = first;

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

    private Gem CreateGemForRowAndCol(Gem gemPrefab, int row, int col, GemTypes gemType)
    {
        Gem gemInstance = Instantiate(gemPrefab);

        gemInstance.rowOnBoard = row;
        gemInstance.colOnBoard = col;
        gemInstance.gemType = gemType;
        gemInstance.gemBoard = this;

        gemInstance.transform.position = ComputeGemPositionViaRowAndCol(row, col);
        gemInstance.transform.rotation = Quaternion.identity;
        gemInstance.transform.SetParent(transform, true);

        return gemInstance;
    }

    private int GetNumberOfMatchesAvailable(Gem[,] gemBoard)
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
        for (int currentRow = 0; currentRow <= gemBoard.GetLength(0) - 1; currentRow++)
        {
            for (int currentCol = 0; currentCol <= gemBoard.GetLength(1) - 4; currentCol++)
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

        foreach (var gem in gems)
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
        foreach (var gem in gems)
        {
            if (gem.hasBeenMatched)
            {
                Destroy(gem.gameObject);
            }
        }

        // wait for a frame so as to let Unity clean up the destroyed gems
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator MoveFallingGemsDown()
    {
        int gemsLeft = 0;

        foreach (var gem in gems)
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

    private IEnumerator GrowGemsAtBlankSpots(List<Gem> gemsToGrow)
    {
        int numGemsToGrow = gemsToGrow.Count;

        foreach (var gem in gemsToGrow)
        {
            gem.transform.DOScale(Vector3.zero, 0.75f).From()
                         .OnComplete(() => numGemsToGrow--);
        }

        yield return new WaitUntil(() => numGemsToGrow <= 0);
    }

    private IEnumerator SwapGemsBack(Gem first, Gem second)
    {
        Vector3 firstGemPosition = first.transform.position;
        Vector3 secondGemPosition = second.transform.position;

        var sequence = DOTween.Sequence()
                              .Insert(0f, first.transform.DOMove(secondGemPosition, 0.5f))
                              .Insert(0f, second.transform.DOMove(firstGemPosition, 0.5f));

        yield return sequence.WaitForCompletion();
    }

    private void DisplayMatchIndicationAtGem(Gem gem)
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
        foreach (var gem in gems)
        {
            Destroy(gem.gameObject);
        }

        GenerateGemsForBoard();
    }

    [ContextMenu("Print Board Representation to Console")]
    private void PrintBoardRepresentationToConsole()
    {
        string representation = "Board representation:\n";

        for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
        {
            for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
            {
                Gem currentGem = gems[currentRow, currentCol];
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
        Debug.Log(GetNumberOfMatchesAvailable(gems));
    }
}
