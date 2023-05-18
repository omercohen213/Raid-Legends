using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    protected float speed;
    protected Vector3 moveDir;

    private BoxCollider2D boxCollider;

    protected virtual void Awake()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
        else Debug.Log("Missing box collider " + transform.name);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Checks collision and updates movement
    protected virtual void UpdateMovement(Vector3 moveDir)
    {
        Vector3 size = boxCollider.bounds.extents;
        float rayLengthX = size.x;
        float rayLengthY = size.y;
        Debug.DrawRay(transform.position, new Vector2(moveDir.x, 0) * rayLengthX, Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, moveDir.y) * rayLengthY, Color.blue);

        RaycastHit2D hitX = Physics2D.Raycast(transform.position, new Vector2(moveDir.x, 0), rayLengthX, LayerMask.GetMask("Obstacle", "Minion", "Tower"));
        if (hitX.collider == null)
        {
            transform.Translate(new Vector2(Time.deltaTime * moveDir.x, 0), Space.World);
        }

        RaycastHit2D hitY = Physics2D.Raycast(transform.position, new Vector2(0, moveDir.y), rayLengthY, LayerMask.GetMask("Obstacle", "Minion", "Tower"));
        if (hitY.collider == null)
        {
            transform.Translate(new Vector2(0, Time.deltaTime * moveDir.y), Space.World);
        }
    }
}
