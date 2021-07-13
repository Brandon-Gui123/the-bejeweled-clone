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

    // Start is called before the first frame update
    void Start()
    {
        for (int currentRow = 0; currentRow < gems.GetLength(0); currentRow++)
        {
            for (int currentCol = 0; currentCol < gems.GetLength(1); currentCol++)
            {
                gems[currentRow, currentCol] = Instantiate(gemPrefab, transform.position, transform.rotation, transform);

                // randomly pick a colour
                gems[currentRow, currentCol].gemType =
                    (GemTypes)Random.Range(0, System.Enum.GetNames(typeof(GemTypes)).Length);

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

                // since we want to process matches outside of this scope
                this.clickedGem = clickedGem;
            }
        }
        else
        {
            previouslySelectedGem = clickedGem;

            // display the gem selection indicator at that gem's position
            gemSelectionIndicator.SetActive(true);
            gemSelectionIndicator.transform.position = clickedGem.transform.position;
        }
    }

    public void CheckForMatch(Gem gem)
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
    }

    public void OnSwappingComplete()
    {
        isSwappingAllowed = true;

        CheckForMatch(clickedGem);
        CheckForMatch(previouslySelectedGem);

        clickedGem = null;
        previouslySelectedGem = null;
    }
}
