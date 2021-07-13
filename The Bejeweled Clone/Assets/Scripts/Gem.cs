using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemTypes gemType;
    public SpriteRenderer gemSprite;

    public GemBoardBehaviour gemBoard;

    public int rowOnBoard;
    public int colOnBoard;

    // Start is called before the first frame update
    void Start()
    {
        gemSprite = GetComponent<SpriteRenderer>();
        
        // colour the gem accordingly
        switch (gemType)
        {
            case GemTypes.Red:
                gemSprite.color = Color.red;
                break;

            case GemTypes.Orange:
                gemSprite.color = new Color(1f, 0.75f, 0f);
                break;

            case GemTypes.Yellow:
                gemSprite.color = Color.yellow;
                break;

            case GemTypes.Green:
                gemSprite.color = Color.green;
                break;

            case GemTypes.Blue:
                gemSprite.color = Color.blue;
                break;

            case GemTypes.Purple:
                gemSprite.color = Color.magenta;
                break;

            case GemTypes.White:
                gemSprite.color = Color.white;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
