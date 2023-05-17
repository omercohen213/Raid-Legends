using UnityEngine;
using Unity.Netcode;

public class PlayerMovment : MonoBehaviour
{
    private Joystick joystick;
    private readonly float speed = 0.1f;
    protected float pushTolerance = 0.2f;
    protected Vector3 moveDir;
    protected RaycastHit2D hit;
    protected BoxCollider2D boxCollider;

    private void Awake()
    {
        joystick = GameObject.Find("UI/FloatingJoystick").GetComponent<Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        // arrow keys (returns 1/-1 on key down)
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        if (moveDir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        moveDir = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(moveDir * speed, Space.World);

        //CheckCollision(new Vector3(horizontalInput, verticalInput, 0), speed);
    }

    private void CheckCollision(Vector3 input, float speed)
    {
        Vector2 playerPos = transform.position;

        // Calculate the movement direction based on player input
        Vector3 moveDir = new Vector3(joystick.Horizontal, Input.GetAxis("Vertical"),0);

        // Calculate the ray length based on the player's speed
        float rayLength = moveDir.magnitude * Time.deltaTime * speed;

        // Cast rays in the movement direction to detect collisions
        RaycastHit2D hitX = Physics2D.Raycast(playerPos, new Vector2(moveDir.x, 0), rayLength, LayerMask.GetMask("Obstacle"));
        RaycastHit2D hitY = Physics2D.Raycast(playerPos, new Vector2(0, moveDir.y), rayLength, LayerMask.GetMask("Obstacle"));

        // Check if either ray hit an obstacle
        if (hitX.collider != null || hitY.collider != null)
        {
            // Collision detected, prevent movement in that direction
            moveDir = Vector2.zero;
        }

        // Move the player based on the movement direction
        transform.Translate(moveDir * Time.deltaTime * speed);

    }

}
