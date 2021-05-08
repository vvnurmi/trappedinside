using UnityEngine;

/// <summary>
/// Public interface and main implementation of the Gate prefab.
/// </summary>
public class GateController : MonoBehaviour
{
    private Rigidbody2D blocker;

    public bool IsOpen => blocker?.simulated == false;
    public bool IsClosed => blocker?.simulated == true;

    public void Open()
    {
        if (IsOpen) return;

        Debug.Log($"Opening gate {this.GetFullName()}");
        blocker.simulated = false;
    }

    public void Close()
    {
        if (IsClosed) return;

        Debug.Log($"Closing gate {this.GetFullName()}");
        blocker.simulated = true;
    }

    #region MonoBehaviour overrides

    public void Start()
    {
        blocker = GetComponentInChildren<Rigidbody2D>();
        Debug.Assert(blocker != null);
    }

    #endregion
}
