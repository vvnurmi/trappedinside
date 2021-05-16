using UnityEngine;

/// <summary>
/// Public interface and main implementation of the Gate prefab.
/// </summary>
public class GateController : MonoBehaviour
{
    private Rigidbody2D blocker;
    private GameObject blockerObject => blocker.gameObject;
    private float closedY, openY, targetY;

    public enum GateState
    {
        Closed,
        Opening,
        Open,
        Closing,
    }

    public GateState State { get; private set; }

    public void Open()
    {
        if (State == GateState.Open || State == GateState.Opening) return;

        Debug.Log($"Opening gate {this.GetFullName()}");
        State = GateState.Opening;
        targetY = openY;
    }

    public void Close()
    {
        if (State == GateState.Closed || State == GateState.Closing) return;

        Debug.Log($"Closing gate {this.GetFullName()}");
        State = GateState.Closing;
        targetY = closedY;
    }

    #region MonoBehaviour overrides

    public void Start()
    {
        blocker = GetComponentInChildren<Rigidbody2D>();
        Debug.Assert(blocker != null);
        closedY = blockerObject.transform.position.y;
        var colliders = new Collider2D[blocker.attachedColliderCount];
        blocker.GetAttachedColliders(colliders);
        Debug.Assert(colliders.Length == 1);
        openY = closedY - colliders[0].bounds.size.y;
    }

    private void FixedUpdate()
    {
        if (State == GateState.Opening || State == GateState.Closing)
        {
            var oldPos = blockerObject.transform.position;
            var newY = Mathf.Lerp(oldPos.y, targetY, 0.05f);
            if (Mathf.Abs(targetY - newY) < 0.01f)
            {
                newY = targetY;
                State = State == GateState.Opening
                    ? GateState.Open
                    : GateState.Closed;
            }
            var newPos = new Vector3(oldPos.x, newY, oldPos.z);
            blockerObject.transform.position = newPos;
        }
    }

    #endregion
}
