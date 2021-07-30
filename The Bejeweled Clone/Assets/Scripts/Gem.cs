using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemTypes gemType;
    public SpriteRenderer gemSprite;
    public GameObject gemSpriteGameObject;

    public GemBoardBehaviour gemBoard;

    public int rowOnBoard;
    public int colOnBoard;

    public bool hasBeenMatched = false;

    public Transform groundCheckTransform;
    public bool isGrounded = false;

    public Vector3 fallDestination;
    public bool isFalling = false;
    public Vector3 currentVelocity;

    // Start is called before the first frame update
    void Start()
    {
        ColorizeGemSprite();
    }

    private void ColorizeGemSprite()
    {
        gemSprite.color = GemUtils.GetColorBasedOnGemType(gemType);
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

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled
    private void FixedUpdate()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(groundCheckTransform.position, -groundCheckTransform.up, 0.2f);

        if (hitInfo.collider)
        {
            if (hitInfo.collider.TryGetComponent(out Gem otherGem))
            {
                // only considered grounded if the other gem is grounded
                isGrounded = otherGem.isGrounded;
            }
            else
            {
                // we're landed on some other collider
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    // Implement this OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.cyan : Color.grey;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
