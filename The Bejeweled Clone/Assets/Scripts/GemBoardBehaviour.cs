using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GemBoardBehaviour : MonoBehaviour
{
    public Gem[,] gems = new Gem[8, 8];

    public Gem gemPrefab;
    public GameObject gemSelectionIndicator;

    public Gem previouslySelectedGem;

    public bool isSwappingAllowed = true;
    public Gem clickedGem;

    // the default for this field is to use all of the gem types
    public GemTypes[] gemTypesToUse = (GemTypes[])System.Enum.GetValues(typeof(GemTypes));

    // Start is called before the first frame update
    void Start()
    {
        GenerateGemsForBoard();
    }

    private void GenerateGemsForBoard()
    {
        for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
        {
            for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
            {
                gems[currentRow, currentCol] = Instantiate(gemPrefab, transform.position, transform.rotation, transform);

                // randomly pick a colour
                gems[currentRow, currentCol].gemType = gemTypesToUse[Random.Range(0, gemTypesToUse.Length)];

                gems[currentRow, currentCol].transform.position =
                    new Vector3(currentCol + (0.1f * currentCol), -(currentRow + (0.1f * currentRow)));

                gems[currentRow, currentCol].rowOnBoard = currentRow;
                gems[currentRow, currentCol].colOnBoard = currentCol;

                gems[currentRow, currentCol].gemBoard = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGemClicked(Gem clickedGem)
    {
        // since we want to process matches outside the scope of this function
        this.clickedGem = clickedGem;

        // do nothing if swapping isn't allowed
        if (!isSwappingAllowed)
        {
            return;
        }

        // do we already have a gem selected?
        if (previouslySelectedGem)
        {
            // check to see if swapping can be done
            
            // if the same gem is clicked as the previous...
            if (clickedGem == previouslySelectedGem)
            {
                // deselect gem
                previouslySelectedGem = null;
                gemSelectionIndicator.SetActive(false);
                return;
            }

            // is the gem we selected adjacent to the gem we previously selected?
            bool isDirectlyAbove = clickedGem.rowOnBoard + 1 == previouslySelectedGem.rowOnBoard && clickedGem.colOnBoard == previouslySelectedGem.colOnBoard;
            bool isDirectlyBelow = clickedGem.rowOnBoard - 1 == previouslySelectedGem.rowOnBoard && clickedGem.colOnBoard == previouslySelectedGem.colOnBoard;
            bool isDirectlyRight = clickedGem.colOnBoard + 1 == previouslySelectedGem.colOnBoard && clickedGem.rowOnBoard == previouslySelectedGem.rowOnBoard;
            bool isDirectlyLeft = clickedGem.colOnBoard - 1 == previouslySelectedGem.colOnBoard && clickedGem.rowOnBoard == previouslySelectedGem.rowOnBoard;

            if (isDirectlyAbove || isDirectlyBelow || isDirectlyRight || isDirectlyLeft)
            {
                // perform the swap
                Debug.Log($"Swap to be performed for gems at ({clickedGem.rowOnBoard}, {clickedGem.colOnBoard}) and ({previouslySelectedGem.rowOnBoard}, {previouslySelectedGem.colOnBoard})");

                // do not allow other gems to be swapped while one is happening
                isSwappingAllowed = false;

                // swap the object instances
                gems[clickedGem.rowOnBoard, clickedGem.colOnBoard] = previouslySelectedGem;
                gems[previouslySelectedGem.rowOnBoard, previouslySelectedGem.colOnBoard] = clickedGem;

                // update positions of the gem so that it appears as swapped
                Vector3 clickedGemOriginalPosition = clickedGem.transform.position;
                clickedGem.transform.DOMove(previouslySelectedGem.transform.position, 0.5f).OnComplete(OnSwappingComplete);
                previouslySelectedGem.transform.DOMove(clickedGemOriginalPosition, 0.5f);

                // update the stored gem positions so that subsequent swapping with the same gems
                // won't cause any issues
                (int rowOnBoard, int colOnBoard) clickedGemOriginalBoardPosition = (clickedGem.rowOnBoard, clickedGem.colOnBoard);
                clickedGem.rowOnBoard = previouslySelectedGem.rowOnBoard;
                clickedGem.colOnBoard = previouslySelectedGem.colOnBoard;
                previouslySelectedGem.rowOnBoard = clickedGemOriginalBoardPosition.rowOnBoard;
                previouslySelectedGem.colOnBoard = clickedGemOriginalBoardPosition.colOnBoard;

                // all gems to be deselected after the swap
                gemSelectionIndicator.SetActive(false);
            }
        }
        else
        {
            previouslySelectedGem = clickedGem;

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
        bool hasMatchForClickedGem = CheckForMatch2(clickedGem);
        bool hasMatchForPreviouslySelectedGem = CheckForMatch2(previouslySelectedGem);

        //if (CheckForMatch(clickedGem) || CheckForMatch(previouslySelectedGem))
        if (hasMatchForClickedGem || hasMatchForPreviouslySelectedGem)
        {
            bool hasGemsToShrink = false;
            bool isShrinkingGems = false;

            // process matched gems
            foreach (Gem gem in gems)
            {
                if (gem.hasBeenMatched)
                {
                    hasGemsToShrink = true;
                    isShrinkingGems = true;

                    gem.transform.DOScale(Vector3.zero, 0.75f).OnComplete(
                        () =>
                        {
                            isShrinkingGems = false;

                            // will cause null errors but that's because
                            // we have not generated new gems
                            Destroy(gem.gameObject);
                        }
                    );
                }
            }

            bool areGemsDoneFalling = false;
            bool hasFallingGems = false;

            bool hasGemsToFill = false;
            bool areGemsDoneFilling = false;

            if (hasGemsToShrink)
            {
                yield return new WaitWhile(() => isShrinkingGems);

                // to wait for Unity to destroy the Gem objects for us
                yield return new WaitForEndOfFrame();

                // bubble sort each column to move empty spaces up and gems down
                for (int col = 0; col < gems.GetLength(1); col++)
                {
                    while (!IsEmptySpacesInGemBoardColumnAllUp(gems, col))
                    {
                        hasFallingGems = true;

                        // start at the top of the column
                        // keep going till we reach right before the last element
                        for (int i = 0; i < gems.GetLength(0) - 1; i++)
                        {
                            // if bottom is an empty space and current isn't
                            // swap current with bottom
                            if (gems[i, col] && !gems[i + 1, col])
                            {
                                Gem tempGem = gems[i, col];

                                gems[i, col] = gems[i + 1, col];
                                gems[i + 1, col] = tempGem;

                                // adjust row and column values for the new gem
                                gems[i + 1, col].rowOnBoard = i + 1;
                                gems[i + 1, col].colOnBoard = col;

                                // update position of the gem
                                tempGem.transform.DOMove(
                                    new Vector3(tempGem.colOnBoard + (0.1f * tempGem.colOnBoard), -(tempGem.rowOnBoard + (0.1f * tempGem.rowOnBoard))),
                                    0.8f
                                ).OnComplete(() => areGemsDoneFalling = true);
                            }
                        }
                    }
                }

                if (hasFallingGems)
                {
                    yield return new WaitUntil(() => areGemsDoneFalling);
                }

                // wherever there are null, fill it with new Gem objects
                for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
                {
                    for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
                    {
                        if (!gems[currentRow, currentCol])
                        {
                            hasGemsToFill = true;

                            gems[currentRow, currentCol] = Instantiate(gemPrefab, transform.position, transform.rotation, transform);

                            gems[currentRow, currentCol].gemType = gemTypesToUse[Random.Range(0, gemTypesToUse.Length)];

                            gems[currentRow, currentCol].transform.position =
                            new Vector3(currentCol + (0.1f * currentCol), -(currentRow + (0.1f * currentRow)));

                            gems[currentRow, currentCol].rowOnBoard = currentRow;
                            gems[currentRow, currentCol].colOnBoard = currentCol;

                            gems[currentRow, currentCol].gemBoard = this;

                            gems[currentRow, currentCol].transform.DOScale(0f, 0.5f).From()
                                                                  .OnComplete(() => areGemsDoneFilling = true);
                        }
                    }
                }
            }

            if (hasGemsToFill)
            {
                yield return new WaitUntil(() => areGemsDoneFilling);
            }
        }
        else
        {
            // the two gems must be moved back to their original positions

            // first, we swap their object instances in the 2-dimensional array
            (int row, int col) clickedGemOriginalCoords = (clickedGem.rowOnBoard, clickedGem.colOnBoard);
            gems[previouslySelectedGem.rowOnBoard, previouslySelectedGem.colOnBoard] = clickedGem;
            gems[clickedGemOriginalCoords.row, clickedGemOriginalCoords.col] = previouslySelectedGem;

            // next, we update the coords on the Gem objects themselves
            clickedGem.rowOnBoard = previouslySelectedGem.rowOnBoard;
            clickedGem.colOnBoard = previouslySelectedGem.colOnBoard;
            previouslySelectedGem.rowOnBoard = clickedGemOriginalCoords.row;
            previouslySelectedGem.colOnBoard = clickedGemOriginalCoords.col;

            // now, we translate the two gem GameObjects
            bool isDoneSwappingBack = false;
            Vector3 clickedGemOriginalPosition = clickedGem.transform.position;
            clickedGem.transform.DOMove(previouslySelectedGem.transform.position, 0.5f);
            previouslySelectedGem.transform.DOMove(clickedGemOriginalPosition, 0.5f).OnComplete(() => isDoneSwappingBack = true);

            // only continue execution after the tweening is done
            yield return new WaitUntil(() => isDoneSwappingBack);
        }

        clickedGem = null;
        previouslySelectedGem = null;
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
            Destroy(gem);
        }

        // generate gems
        for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
        {
            for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
            {
                gems[currentRow, currentCol] = Instantiate(gemPrefab, transform.position, transform.rotation, transform);

                // randomly pick a colour
                gems[currentRow, currentCol].gemType = gemTypesToUse[Random.Range(0, gemTypesToUse.Length)];

                gems[currentRow, currentCol].transform.position =
                    new Vector3(currentCol + (0.1f * currentCol), -(currentRow + (0.1f * currentRow)));

                gems[currentRow, currentCol].rowOnBoard = currentRow;
                gems[currentRow, currentCol].colOnBoard = currentCol;

                gems[currentRow, currentCol].gemBoard = this;
            }
        }
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
}
