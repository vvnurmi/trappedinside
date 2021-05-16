using System.Collections.Generic;
using UnityEngine;

using static InputActions;
using static UnityEngine.InputSystem.InputAction;

public class TIInputStateManager : MonoBehaviour, IPlayerActions, IUIActions
{
    private static GameObject host;
    public static TIInputStateManager instance;

    private InputActions inputActions;
    private List<TIInputContext> inputContexts = new List<TIInputContext>();

    // Made private so that contexts are not created without registering them.
    private class TIInputContext : ITIInputContext
    {
        public TIInputState inputState;

        public TIInputState GetStateAndResetEventFlags()
        {
            var currentInput = inputState;
            inputState.fire1Pressed = false;
            inputState.fire2Pressed = false;
            inputState.jumpPressed = false;
            inputState.jumpReleased = false;
            inputState.uiSubmitPressed = false;
            inputState.uiCancelPressed = false;
            return currentInput;
        }

        public void Dispose()
        {
            instance.Unregister(this);
        }
    }

    public TIInputStateManager()
    {
        Debug.Assert(instance == null, $"Don't create {nameof(TIInputStateManager)} manually. Access {nameof(instance)} instead.");
    }

    /// <summary>
    /// Returns a new input context. Call this once from each script that needs to keep
    /// track of input state.
    /// </summary>
    public ITIInputContext CreateContext()
    {
        var context = new TIInputContext();
        Register(context);
        return context;
    }

    public void DisablePlayerInput() =>
        inputActions.Player.Disable();

    public void EnablePlayerInput() =>
        inputActions.Player.Enable();

    private void Register(TIInputContext inputContext)
    {
        inputContexts.Add(inputContext);
    }

    private void Unregister(TIInputContext inputContext)
    {
        var success = inputContexts.Remove(inputContext);
        Debug.Assert(success, $"Couldn't find {nameof(TIInputContext)} to unregister");
    }

    #region MonoBehaviour overrides

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Assert(host == null);
        host = new GameObject("TI Input State Manager");
        instance = host.AddComponent<TIInputStateManager>();
        DontDestroyOnLoad(host);
    }

    private void Awake()
    {
        Debug.Assert(inputActions == null);
        inputActions = new InputActions();
        inputActions.Player.SetCallbacks(this);
        inputActions.UI.SetCallbacks(this);
        inputActions.Enable();
    }

    #endregion

    #region IPlayerActions

    public void OnMove(CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.horizontal = value.x;
            ic.inputState.vertical = value.y;
        }
    }

    public void OnShield(CallbackContext context)
    {
        var value = context.ReadValue<float>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.fire2Pressed |= !ic.inputState.fire2Active && value >= 0.5f;
            ic.inputState.fire2Active = value >= 0.5f;
        }
    }

    public void OnJump(CallbackContext context)
    {
        var value = context.ReadValue<float>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.jumpPressed |= !ic.inputState.jumpActive && value >= 0.5f;
            ic.inputState.jumpReleased |= ic.inputState.jumpActive && value < 0.5f;
            ic.inputState.jumpActive = value >= 0.5f;
        }
    }

    #endregion

    #region IUIActions

    public void OnNavigate(CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.uiNavigate = value;
        }
    }

    public void OnSubmit(CallbackContext context)
    {
        var value = context.ReadValue<float>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.uiSubmitPressed |= !ic.inputState.uiSubmitActive && value >= 0.5f;
            ic.inputState.uiSubmitActive = value >= 0.5f;
        }
    }

    public void OnCancel(CallbackContext context)
    {
        var value = context.ReadValue<float>();
        foreach (var ic in inputContexts)
        {
            ic.inputState.uiCancelPressed |= !ic.inputState.uiCancelActive && value >= 0.5f;
            ic.inputState.uiCancelActive = value >= 0.5f;
        }
    }

    #endregion
}
