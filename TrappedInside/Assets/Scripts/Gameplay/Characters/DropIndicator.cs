using UnityEngine;

public class DropIndicator : MonoBehaviour
{

    BoxCollider2D groudCollider;

    void Start()
    {
        groudCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsGroundLayer(collision))
            IsGroundAhead = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsGroundLayer(collision))
            IsGroundAhead = false;
    }

    private bool IsGroundLayer(Collider2D collision) => 
        LayerMask.LayerToName(collision.gameObject.layer) == "Ground";


    public bool IsGroundAhead { get; private set; } = false;
}
