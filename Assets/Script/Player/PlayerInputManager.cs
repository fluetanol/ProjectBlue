using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputActionControll
{
    public void OnAttack(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction);
    public void OnAttack(Action performedAction, Action canceledAction);
    public void OnMove(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction);
    public void OnLook(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction);

    public InputSystem_Actions InputActions
    {
        get;
    }

    // Player Click Time
    public float ClickTime
    {
        get;
    }

    public bool IsClicked
    {
        get;
    }

}

public interface IInputData
{
    public Vector3 CursorPosition
    {
        get;
    }

    public Vector3 CursorWorldPosition
    {
        get;
    }

    public Vector3 CursorRaycastingPosition
    {
        get;
    }
}



public class PlayerInputManager : MonoBehaviour, IInputActionControll, IInputData
{
    // Player Input Actions
    private InputSystem_Actions _inputActions;
    public InputSystem_Actions InputActions
    {
        get
        {
            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Enable();
            }
            return _inputActions;
        }
    }

    // Player Click Time
    [SerializeField] private float _clickTime = 0f;
    public float ClickTime
    {
        get
        {
            return _clickTime;
        }
        private set
        {
            _clickTime = value;
        }
    }

    [SerializeField] private bool _isClicked = false; // Maximum time for a click to be considered valid
    public bool IsClicked
    {
        get
        {
            return _isClicked;
        }
        private set { _isClicked = value; }
    }

    public Vector3 CursorPosition
    {
        get
        {
            return InputActions.Player.Look2.ReadValue<Vector2>();
        }
    }

    public Vector3 CursorWorldPosition
    {
        get
        {
            // Convert the cursor position to world space if needed
            Vector2 screenPosition = InputActions.Player.Look2.ReadValue<Vector2>();
            return Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.nearClipPlane));
        }
    }

    public Vector3 CursorRaycastingPosition
    {
        get
        {
            // Convert the cursor position to world space if needed
            Ray ray = Camera.main.ScreenPointToRay(InputActions.Player.Look2.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, LayerMask.GetMask("Ground")))
            {
                print("레이캐스팅 정보 " + hitInfo.point + " " + hitInfo.collider.name + " " + hitInfo.collider.gameObject.layer);
                return hitInfo.point;
            }
            else
            {
                // If the raycast doesn't hit anything, return a default position (e.g., origin)
                return Vector3.zero;
            }

        }
    }

    void OnEnable()
    {

        _inputActions.Player.Attack.performed += ctx => OnClick();
        _inputActions.Player.Attack.canceled += ctx => OnRelease();
    }

    void OnDisable()
    {
        _inputActions.Player.Attack.performed -= ctx => OnClick();
        _inputActions.Player.Attack.canceled -= ctx => OnRelease();
    }

    // Update is called once per frame
    void Update()
    {
        CheckClickTime();
        //forDebug();
    }

    private void CheckClickTime()
    {
        if (IsClicked)
        {
            ClickTime += Time.deltaTime;
        }
    }

    public void OnAttack(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction)
    {
        //if (action == null) return;
        if (performedAction != null) InputActions.Player.Attack.performed += performedAction;
        if (canceledAction != null) InputActions.Player.Attack.canceled += canceledAction;
    }

    public void OnAttack(Action performedAction, Action canceledAction)
    {
        //if (action == null) return;
        if (performedAction != null) InputActions.Player.Attack.performed += (ctx) => performedAction();
        if (canceledAction != null) InputActions.Player.Attack.canceled += (ctx) => canceledAction();
    }


    public void OnMove(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction)
    {
        if (performedAction != null) InputActions.Player.Move.performed += performedAction;
        if (canceledAction != null) InputActions.Player.Move.canceled += canceledAction;
    }

    public void OnLook(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction)
    {
        //if (performedAction == null || canceledAction == null) return;
        if (performedAction != null) InputActions.Player.Look2.performed += performedAction;
        if (canceledAction != null) InputActions.Player.Look2.canceled += canceledAction;
    }

    public void OnClick()
    {
        IsClicked = true;
        ClickTime = 0f;
    }

    public void OnRelease()
    {
        IsClicked = false;
        ClickTime = 0f;
    }

}
