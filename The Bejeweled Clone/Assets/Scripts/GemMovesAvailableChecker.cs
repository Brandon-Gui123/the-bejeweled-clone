using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemMovesAvailableChecker : MonoBehaviour
{
    public TextMeshProUGUI movesAvailableCounter;

    public void UpdateMovesAvailableCounter(GemBoard gemBoard)
    {
        movesAvailableCounter.text = GemBoardUtils.GetNumberOfMovesAvailable(gemBoard).ToString();
    }
}
