using System;
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


    struct CapsuleInfo
    {
        public float radius;
        public Vector3 point1;
        public Vector3 point2;
    }

    [Header("For Movement")]
    public int HorizontalCollideRecursion = 3;
    public int VerticalCollideRecursion = 3;
    public Transform leftFootIK, rightFootIK;


    //추후엔 별도의 물리 시스템 설정값 다루는 스크립터블 오브젝트로 편입시킬 예정
    public float GravityMultiplier = 1.25f;

    /// <summary>
    /// for movement vector
    /// </summary>
    public Vector3 ydelta = Vector3.zero;
    public Vector3 xdelta = Vector3.zero;
    public Vector3 nextDelta = Vector3.zero;
    public bool isGrounded = false;

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


    private Vector3 groundNormal, groundPoint;
    
    void FixedUpdate()
    {
        xdelta =  _moveTypeList[(int)_playerMoveAxisType] * PlayerDataManager.currentMoveSpeed * Time.fixedDeltaTime;
        if(isGrounded) ydelta = Vector3.zero;
        ydelta += Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime * GravityMultiplier;

        //print(ydelta.y+" "+ Physics.gravity * Time.fixedDeltaTime * Time.fixedDeltaTime);

        nextDelta = HorizontalCollideAndSlide(xdelta, _rigidbody.position, 0);
        nextDelta += VerticalCollideAndSlide(ydelta, _rigidbody.position + nextDelta, 0);
    
        _rigidbody.MovePosition(_rigidbody.position + nextDelta);
        PlayerPosition = _rigidbody.position;



        //RaycastHit hit;
        //if (Physics.Raycast(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up * 0.5f, Vector3.down, out hit, 1.0f))
        //{
        //    groundNormal = hit.normal;
        //}
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


    private Vector3 HorizontalCollideAndSlide(Vector3 delta, Vector3 playerPosition, int count, int maxRecursion = 3)
    {
        if (count >= maxRecursion) return delta;

        CapsuleInfo info = CapsuleCastInfo(playerPosition);
        Vector3 direction = delta.normalized;
        Vector3 sumVector = Vector3.zero;

        if (Physics.CapsuleCast(info.point1, info.point2, info.radius, direction, out RaycastHit hit, delta.magnitude + 0.01f)){
            if(count == 0) print(hit.normal +" "+ direction +" " + Vector3.Dot(direction, hit.normal));
            //hit하고 남은 거리와 벡터
            float restMagnitude = delta.magnitude - hit.distance;
            if(restMagnitude <= 0.01f) return direction * (hit.distance - 0.01f);

            Vector3 movedelta = direction * hit.distance;

            //남은 벡터로 충돌 검사
            Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude;
            return sumVector + HorizontalCollideAndSlide(projectDelta, playerPosition + movedelta , count + 1);
        }else{
            return delta;
        }
    }



     private Vector3 VerticalCollideAndSlide(Vector3 delta, Vector3 playerPosition, int count, int maxRecursion = 3)
    {
        if (count >= maxRecursion) return delta;

        CapsuleInfo info = CapsuleCastInfo(playerPosition);
        Vector3 direction = delta.normalized;
        Vector3 sumVector = Vector3.zero;

        if (Physics.CapsuleCast(info.point1, info.point2, info.radius, direction, out RaycastHit hit, delta.magnitude + 0.01f))
        {
            if(!isGrounded){
                isGrounded = true;
            }

            //hit하고 남은 거리와 벡터
            float restMagnitude = delta.magnitude - hit.distance;
            if (restMagnitude <= 0.01f) return direction * (hit.distance - 0.01f);

            Vector3 movedelta = direction * hit.distance;

            //남은 벡터로 충돌 검사
            Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude;
            return sumVector + HorizontalCollideAndSlide(projectDelta, playerPosition + movedelta, count + 1);
        }
        else
        {
            if(isGrounded) isGrounded = false;
            return delta;
        }
    }





    private CapsuleInfo CapsuleCastInfo(Vector3 playerPosition)
    {
        float radius = _collider.radius;
        Vector3 center = playerPosition + _collider.center;
        float halfHeight = Mathf.Max(_collider.height / 2 - _collider.radius, 0);// 캡슐 높이에서 반지름 제외
        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center + Vector3.down * halfHeight;

        return new CapsuleInfo()
        {
            radius = radius,
            point1 = point1,
            point2 = point2
        };
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
            hitpoint.y = transform.position.y;
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

        //Gizmos.DrawWireSphere(transform.position, 6);
        //(x-a)^2 + (z-b)^2 = r^2;


        Vector3 direction = (_lookPosition - transform.position).normalized;
        Vector3 dir1 = Quaternion.AngleAxis(30, Vector3.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-30, Vector3.up) * direction;

        Gizmos.DrawRay(transform.position, direction * 6);
        Gizmos.DrawRay(transform.position, dir1 * 6);
        Gizmos.DrawRay(transform.position, dir2 * 6);
    }


}
