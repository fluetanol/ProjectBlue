using UnityEngine;


public class PlayerInputManager : MonoBehaviour
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
}
