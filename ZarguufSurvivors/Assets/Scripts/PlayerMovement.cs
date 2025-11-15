using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 MoveDir { get { return moveDir; } }
    private Vector2 moveDir;

    InputAction moveAction;

    [SerializeField]
    private bool useRigidbody;
    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private float playerSpeed;

    private Rigidbody2D playerBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        playerBody = GetComponent<Rigidbody2D>();

        if (!useRigidbody)
        {
            playerBody.simulated = false;
        }

        moveDir = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        
        if (moveValue.sqrMagnitude != 0) {
            moveDir = moveValue.normalized;
        }

        if (moveValue.x < 0)
        {
            playerSprite.flipX = true;
        }
        else if (moveValue.x > 0)
        {
            playerSprite.flipX = false;
        }

        if (useRigidbody)
        {
            playerBody.linearVelocity = moveValue * playerSpeed;
        }
        else
        {
            Vector2 oldPos2 = new Vector2(transform.position.x, transform.position.y);
            Vector2 newPos2 = oldPos2 + moveValue * playerSpeed * Time.deltaTime;

            transform.position = new Vector3(newPos2.x, newPos2.y, transform.position.z);
        }
    }
}