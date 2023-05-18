using UnityEngine;

public class PlayerMovment : EntityMovement
{
    [SerializeField] private Joystick joystick;

    protected override void Start()
    {
        base.Start();
        speed = 2f;
    }

    void Update()
    {
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;

        if (horizontalInput > 0)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        moveDir = new Vector3(horizontalInput * speed, verticalInput * speed, 0);
        UpdateMovement(moveDir);
    }
}
