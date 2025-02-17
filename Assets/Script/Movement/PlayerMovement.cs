using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{   
    [Header("For Debugging")]
    public LineRenderer lineRenderer;

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

    public static Vector3 LookDirection{
        get;
        private set;
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

    public static bool IsClicked
    {
        get;
        private set;
    }

    public static bool IsMove
    {
        get;
        private set;
    }

    public static Vector3 _lookPosition{
        get;
        private set;
    }


    [SerializeField] private Animator _animator;

    private static InputSystem_Actions _inputActions;
    private        CapsuleCollider     _collider;
    private        Rigidbody           _rigidbody;


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
        _collider = GetComponent<CapsuleCollider>();
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


    void FixedUpdate()
    {
        Vector3 delta = _moveTypeList[(int)_playerMoveAxisType] * PlayerDataManager.currentMoveSpeed * Time.fixedDeltaTime;
        float radius;
        Vector3 point1, point2;
        CapsuleCastInfo(out radius, out point1, out point2);

        if (Physics.CapsuleCast(point1, point2, radius, delta.normalized, out RaycastHit hit, delta.magnitude)){
            delta = MiniCollideAndSlide(delta, hit);
        }

        _rigidbody.MovePosition(_rigidbody.position + delta);
        PlayerPosition = _rigidbody.position;
    }


    // Update is called once per frame
    void Update(){ 
        CheckClickTime();
        forDebug();
    }

    private void CheckClickTime(){
        if (IsClicked){
            ClickTime += Time.deltaTime;
        }
    }


    private Vector3 MiniCollideAndSlide(Vector3 delta, RaycastHit hit)
    {
        //float restMagnitude = delta.magnitude - hit.distance;

        //현 위치와 충돌 위치와의 거리 = delta.magnitude
        delta = delta.normalized * hit.distance;

        //Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal) * restMagnitude;
        //delta = delta + projectDelta;
        delta = Vector3.ProjectOnPlane(delta, hit.normal);

        return delta;
    }

    private void CapsuleCastInfo(out float radius, out Vector3 point1, out Vector3 point2)
    {
        radius = _collider.radius;
        Vector3 center = transform.position + _collider.center;
        float halfHeight = Mathf.Max(_collider.height / 2 - _collider.radius, 0);// 캡슐 높이에서 반지름 제외
        point1 = center + Vector3.up * halfHeight;
        point2 = center + Vector3.down * halfHeight;
    }


    void OnMoveStart(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
        MoveDirction = context.ReadValue<Vector2>();
        IsMove = true;
        SetAnimMove();
    }

    void OnMoveCancel(InputAction.CallbackContext context)
    {
       // Debug.Log(context.ReadValue<Vector2>());
        MoveDirction = Vector2.zero;
        IsMove = false;
        SetAnimMove();
    }
    
    void OnClickStart(InputAction.CallbackContext context)
    {
        IsClicked = true;
        StartCoroutine(Coroutine_Attacking());
       // Debug.Log("ClickedStart");
       SetAnimClick();
    }

    void OnClickCancel(InputAction.CallbackContext context)
    {
        IsClicked = false;
        ClickTime = 0;
        SetAnimClick();
        StopAllCoroutines();
    }


    private void OnLook(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());
        if(Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Ground"))){
            Vector3 hitpoint = hit.point;
            transform.LookAt(hitpoint);
            _lookPosition = hitpoint;
            LookDirection = (_lookPosition - transform.position).normalized;
        }
    }

    private IEnumerator Coroutine_Attacking(){
        float attackSpeed = PlayerDataManager.WeaponStats[PlayerDataManager.PlayerStats.WeaponID].AttackSpeed;
        while(true){
            AttackShooting();
            yield return new WaitForSeconds(attackSpeed);
        }
    }


    private void AttackShooting(){
        PlayerDataManager.weapon.Attack();
    }

    private void SetAnimMove(){
        _animator.SetBool("IsMove", IsMove);
      //  print("IsMove : " + IsMove);
    }

    private void SetAnimClick(){
        _animator.SetBool("IsClicked", IsClicked);
       // print("IsClicked : " + IsClicked);
    }


    public static Color Debugcolor = Color.red;


    void forDebug()
    {
        if (lineRenderer == null) return;
        Vector3 direction = _lookPosition - transform.position;
        Vector3 dir = Quaternion.AngleAxis(30, Vector3.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-30, Vector3.up) * direction;
        dir.y = 0;
        dir2.y = 0;
        dir = dir.normalized * 6;
        dir2 = dir2.normalized * 6;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + dir);
        lineRenderer.SetPosition(2, transform.position);
        lineRenderer.SetPosition(3, transform.position + dir2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Debugcolor;

        Gizmos.DrawWireSphere(transform.position, 6);

        Vector3 direction = (_lookPosition - transform.position).normalized;
        Vector3 dir1 = Quaternion.AngleAxis(30, Vector3.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-30, Vector3.up) * direction;

        Gizmos.DrawRay(transform.position, direction * 6);
        Gizmos.DrawRay(transform.position, dir1 * 6);
        Gizmos.DrawRay(transform.position, dir2 * 6);
    }


}
