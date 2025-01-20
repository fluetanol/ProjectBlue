using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static Vector3 PlayerPosition;

    enum EPlayerMoveAxis{
        XY = 0,
        XZ = 1
    }
    [SerializeField] private EPlayerMoveAxis _playerMoveAxisType;

    // Player Input Actions
    public static InputSystem_Actions InputActions
    {
        get
        {

            if (_inputActions == null)
            {
                _inputActions = new InputSystem_Actions();
            }

            return _inputActions;
        }
        private set { }
    }

    // Player Move Direction
    public static Vector3 MoveDirction
    {
        get;
        private set;
    }

    // Player Click Time
    public static float ClickTime
    {
        get;
        private set;
    }

    public Transform ShootPoint;

    [Header("Player Stats")]
    public PlayerStats PlayerStats;

    [SerializeField] GameObject BulletPrefab;

    private static InputSystem_Actions _inputActions;
    private        Rigidbody           _rigidbody;
    private        bool                _isClicked = false;
    private        Vector2             _lookPosition;

    private        Vector3[]           _moveTypeList{
        get{
            return new Vector3[]
            {
                new Vector3(MoveDirction.x,  MoveDirction.y),
                new Vector3(MoveDirction.x, 0, MoveDirction.y)
            };
        }
    }


    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _inputActions = new();
    }

    void OnEnable() {
        _inputActions.Player.Move.performed += OnMoveStart;
        _inputActions.Player.Move.canceled += OnMoveCancel;
        _inputActions.Player.Attack.started += OnClickStart;
        _inputActions.Player.Attack.canceled += OnClickCancel;
        _inputActions.Player.Look2.performed += OnLook;
        _inputActions.Enable();   
    }


    void FixedUpdate() {
        Vector3 delta = _moveTypeList[(int)_playerMoveAxisType] * PlayerStats.MoveSpeed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + delta);
        PlayerPosition = _rigidbody.position;
    }


    // Update is called once per frame
    void Update()
    {
        if(_isClicked) {
            ClickTime += Time.deltaTime;
            
        }
    }
    
    void OnMoveStart(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
        MoveDirction = context.ReadValue<Vector2>();
    }

    void OnMoveCancel(InputAction.CallbackContext context)
    {
       // Debug.Log(context.ReadValue<Vector2>());
        MoveDirction = Vector2.zero;
    }
    
    void OnClickStart(InputAction.CallbackContext context)
    {
        _isClicked = true;
        AttackShooting();
        Debug.Log("ClickedStart");
    }

    void OnClickCancel(InputAction.CallbackContext context)
    {
        _isClicked = false;
        ClickTime = 0;
      //  Debug.Log("ClickedCancel");
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());
        if(Physics.Raycast(ray, out RaycastHit hit)){
            transform.LookAt(hit.point);
            _lookPosition = hit.point;
        }
    }

    private void AttackShooting(){
        GameObject Bullet = Instantiate(BulletPrefab, ShootPoint.position, ShootPoint.rotation);
        Bullet bulletComponent = Bullet.GetComponent<Bullet>();
        bulletComponent.bulletDiretion = transform.forward;

    }


}
