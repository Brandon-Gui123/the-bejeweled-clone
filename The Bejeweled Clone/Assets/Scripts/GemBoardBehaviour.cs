using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBoardBehaviour : MonoBehaviour
{
    public Gem[,] gems = new Gem[8, 8];

    public Gem gemPrefab;
    public GameObject gemSelectionIndicator;

    public bool hasSelectedGem = false;
    public (int row, int col) currentlySelectedGemPosition;

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

    public void OnGemClicked(int gemRow, int gemCol)
    {
        // do we already have a gem selected?
        if (hasSelectedGem)
        {
            // check to see if swapping can be done
            
            // if the same gem is clicked as the previous, don't do anything
            if (gemRow == currentlySelectedGemPosition.row && gemCol == currentlySelectedGemPosition.col)
            {
                return;
            }

            // is the gem we selected adjacent to the gem we previously selected?
            bool isDirectlyAbove = currentlySelectedGemPosition.row + 1 == gemRow && currentlySelectedGemPosition.col == gemCol;
            bool isDirectlyBelow = currentlySelectedGemPosition.row - 1 == gemRow && currentlySelectedGemPosition.col == gemCol;
            bool isDirectlyRight = currentlySelectedGemPosition.col + 1 == gemCol && currentlySelectedGemPosition.row == gemRow;
            bool isDirectlyLeft = currentlySelectedGemPosition.col - 1 == gemCol && currentlySelectedGemPosition.row == gemRow;

            if (isDirectlyAbove || isDirectlyBelow || isDirectlyRight || isDirectlyLeft)
            {
                // perform the swap
                Debug.Log($"Swap to be performed for gems at ({gemRow}, {gemCol}) and ({currentlySelectedGemPosition.row}, {currentlySelectedGemPosition.col})");

                Gem currentlySelected = gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col];
                Vector3 currentlySelectedOriginalPosition = gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col].transform.position;

                // update positions of the gem so that it appears as swapped
                gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col].transform.position = gems[gemRow, gemCol].transform.position;
                gems[gemRow, gemCol].transform.position = currentlySelectedOriginalPosition;

                // update the stored gem positions so that subsequent swapping with the same gems
                // won't cause any issues
                gems[gemRow, gemCol].rowOnBoard = currentlySelected.rowOnBoard;
                gems[gemRow, gemCol].colOnBoard = currentlySelected.colOnBoard;
                gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col].rowOnBoard = gemRow;
                gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col].colOnBoard = gemCol;

                // swap the object instances of the gem
                gems[currentlySelectedGemPosition.row, currentlySelectedGemPosition.col] = gems[gemRow, gemCol];
                gems[gemRow, gemCol] = currentlySelected;

                // all gems to be deselected after the swap
                hasSelectedGem = false;
                gemSelectionIndicator.SetActive(false);
            }
            else
            {
                // assume we selected a new gem
                currentlySelectedGemPosition.row = gemRow;
                currentlySelectedGemPosition.col = gemCol;

                // display the gem selection indicator at that gem's position
                gemSelectionIndicator.SetActive(true);
                gemSelectionIndicator.transform.position = gems[gemRow, gemCol].transform.position;
            }
        }
        else
        {
            // the gem will clicked on will be the one we select
            hasSelectedGem = true;

            // keep the row and column values for reference later
            currentlySelectedGemPosition.row = gemRow;
            currentlySelectedGemPosition.col = gemCol;

            // display the gem selection indicator at that gem's position
            gemSelectionIndicator.SetActive(true);
            gemSelectionIndicator.transform.position = gems[gemRow, gemCol].transform.position;
        }
    }

    public void CheckForMatch(int gemRow, int gemCol)
    {
        bool hasMatchAbove = false;
        bool hasMatchRight = false;
        bool hasMatchBelow = false;
        bool hasMatchLeft = false;

        bool hasMiddleVerticalMatch = false;
        bool hasMiddleHorizontalMatch = false;

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
        }

        if (hasMatchRight)
        {
            Debug.Log($"Found match to the right for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);
        }

        if (hasMatchBelow)
        {
            Debug.Log($"Found match below for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);
        }

        if (hasMatchLeft)
        {
            Debug.Log($"Found match to the left for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);
        }
    
        if (hasMiddleVerticalMatch)
        {
            Debug.Log($"Found vertical middle match at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);
        }
    
        if (hasMiddleHorizontalMatch)
        {
            Debug.Log($"Found horizontal middle match for gem at ({gemRow}, {gemCol})", gems[gemRow, gemCol]);
        }
    }
}
