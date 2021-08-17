using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GemMatchesTracker : MonoBehaviour
{
    int numMatchesMade = 0;
    public TextMeshProUGUI matchesCounter;

    // Start is called before the first frame update
    void Start()
    {
        numMatchesMade = 0;
    }
}
