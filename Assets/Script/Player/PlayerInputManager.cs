using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputAction
{
    public void OnAttack(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction);
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



public class PlayerInputManager : MonoBehaviour, IInputAction
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

    void OnEnable() {
    
        _inputActions.Player.Attack.performed += ctx => OnClick();
        _inputActions.Player.Attack.canceled += ctx => OnRelease();
    }

    void OnDisable() {
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

    public void OnMove(Action<InputAction.CallbackContext> performedAction, Action<InputAction.CallbackContext> canceledAction)
    {
        if(performedAction!=null) InputActions.Player.Move.performed += performedAction;
        if(canceledAction!= null) InputActions.Player.Move.canceled += canceledAction;
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
