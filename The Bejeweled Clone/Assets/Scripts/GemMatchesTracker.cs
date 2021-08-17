using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemMatchesTracker : MonoBehaviour
{
    public int numMatchesMade = 0;
    public TextMeshProUGUI matchesCounter;

    // Start is called before the first frame update
    void Start()
    {
        numMatchesMade = 0;
        matchesCounter.text = numMatchesMade.ToString();
    }

    public void SetMatchCount(int value)
    {
        numMatchesMade = value;
        matchesCounter.text = numMatchesMade.ToString();
    }
}
