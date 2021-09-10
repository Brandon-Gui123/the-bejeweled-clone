using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBehaviour : MonoBehaviour
{
    public SpriteRenderer gemSprite;
    public GameObject gemSpriteGameObject;

    public GemBoardBehaviour gemBoard;

    public bool hasBeenMatched = false;

    public Vector3 fallDestination;
    public bool isFalling = false;
    public Vector3 currentVelocity;

    public Gem gem;

    // Start is called before the first frame update
    void Start()
    {
        ColorizeGemSprite();
    }

    public void UpdateGemColor()
    {
        ColorizeGemSprite();
    }

    private void ColorizeGemSprite()
    {
        gemSprite.color = GemUtils.GetColorBasedOnGemType(gem.GemType);
    }

    // Called while the user's cursor is over a collider and the mouse button is pressed down
    private void OnMouseDown()
    {
        gemBoard.OnGemClicked(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            // acceleration due to gravity
            currentVelocity += new Vector3(0f, -10f * Time.deltaTime, 0f);

            transform.Translate(currentVelocity * Time.deltaTime);

            if (transform.position.y <= fallDestination.y)
            {
                transform.position = fallDestination;
                currentVelocity = Vector3.zero;
                isFalling = false;
                fallDestination = Vector3.zero;
            }
        }
    }
}
