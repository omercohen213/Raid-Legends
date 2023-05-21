using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EntityMovement : MonoBehaviour
{
    [SerializeField] protected string[] blockingLayers;
    [SerializeField] protected float speed;
    protected Vector3 moveDir;

    private Collider2D coll;

    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    // Checks collision and updates movement
    protected virtual void UpdateMovement(Vector3 moveDir)
    {
        Vector3 size = coll.bounds.extents;
        float rayLengthX = size.x;
        float rayLengthY = size.y;
        Debug.DrawRay(transform.position, new Vector2(moveDir.x, 0) * rayLengthX, Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, moveDir.y) * rayLengthY, Color.blue);


        RaycastHit2D hitX = Physics2D.Raycast(transform.position, new Vector2(moveDir.x, 0), rayLengthX, LayerMask.GetMask(blockingLayers));
        if (hitX.collider == null)
        {
            transform.Translate(new Vector2(Time.deltaTime * moveDir.x, 0), Space.World);
        }

        RaycastHit2D hitY = Physics2D.Raycast(transform.position, new Vector2(0, moveDir.y), rayLengthY, LayerMask.GetMask(blockingLayers));
        if (hitY.collider == null)
        {
            transform.Translate(new Vector2(0, Time.deltaTime * moveDir.y), Space.World);
        }
    }
}
