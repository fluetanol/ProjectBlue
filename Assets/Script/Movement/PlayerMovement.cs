using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

//[ExecuteInEditMode]
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

    //추후엔 별도의 물리 시스템 설정값 다루는 스크립터블 오브젝트로 편입시킬 예정
    public float GravityMultiplier = 1.25f;

    /// <summary>
    /// for movement vector
    /// </summary>
    public Vector3 ydelta = Vector3.zero;
    public Vector3 xdelta = Vector3.zero;
    public Vector3 nextDelta = Vector3.zero;
    public bool isGrounded = false;

    [Space]
    [Header("For Stair Movement")]
    public Transform stairRayCastPoint1;
    public Transform stairRayCastPoint2;
    public Transform slopeRayCastPoint;
    public float stairRayCastDistance = 0.5f;
    


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

        //stair 검사
        if(IsStair() && isGrounded && xdelta != Vector3.zero){
             nextDelta = StairMovement();
            // nextDelta += VerticalCollideAndSlide(ydelta, _rigidbody.position + nextDelta, 0);
        }
        else{
            //없는 경우 일반 collide and slide
           nextDelta = HorizontalCollideAndSlide(xdelta, _rigidbody.position, 0);
           nextDelta += VerticalCollideAndSlide(ydelta, _rigidbody.position + nextDelta, 0);
        }


        _rigidbody.MovePosition(_rigidbody.position + nextDelta);
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
    

    private bool IsStair(){
        Ray stairConfimRay = new Ray(slopeRayCastPoint.position, slopeRayCastPoint.forward);

        //1. stair인지 확인 (stair는 최소조건이 반드시 90도 입니다.)
        if(Physics.Raycast(stairConfimRay, out RaycastHit hit, PlayerDataManager.currentMoveSpeed * Time.fixedDeltaTime)){
           // print(hit.normal);
            if(hit.normal  == Vector3.right || hit.normal == Vector3.left || hit.normal == Vector3.forward || hit.normal == Vector3.back){
                
                
                //2. 올라갈 수 있는 stair인지 확인
                Ray climbConfirmRay = new Ray(stairRayCastPoint1.position, stairRayCastPoint1.forward);
                if(!Physics.Raycast(climbConfirmRay, PlayerDataManager.currentMoveSpeed * Time.fixedDeltaTime)){
                        print("can climb stair"); 
                        return true;
                    
                }

                //올라갈 수 없는 stair면 벽일 확률이 높음...
            }
        }
        return false;
    }

    private Vector3 StairMovement(){
        Ray stairPointRay = new Ray(stairRayCastPoint2.position, Vector3.down);
        Vector3 delta = Vector3.zero;
        if (Physics.Raycast(stairPointRay, out RaycastHit hit, stairRayCastDistance)){
            Vector3 targetPosition = hit.point;
           // targetPosition.y += 0.01f;
            //targetPosition.y += _collider.height / 2;   0.01f는 오차 보정

            print((targetPosition - _rigidbody.position).magnitude +" " + xdelta.magnitude);
            Debug.DrawRay(_rigidbody.position, targetPosition - _rigidbody.position, Color.green, 5);
            delta = (targetPosition - _rigidbody.position).normalized * xdelta.magnitude;
        }
        return delta;
    }





    private Vector3 HorizontalCollideAndSlide(Vector3 delta, Vector3 playerPosition, int count, int maxRecursion = 3)
    {
        if (count >= maxRecursion) return delta;

        CapsuleInfo info = CapsuleCastInfo(playerPosition);
        Vector3 direction = delta.normalized;
        Vector3 sumVector = Vector3.zero;

        if (Physics.CapsuleCast(info.point1, info.point2, info.radius, direction, out RaycastHit hit, delta.magnitude + 0.01f)){
            if(count == 0) {
                //print(hit.normal +" "+ direction +" " + Vector3.Dot(direction, hit.normal));
                //Debug.DrawRay(hit.point, hit.normal, Color.green, 5);
            }
            //hit하고 남은 거리와 벡터
            float restMagnitude = delta.magnitude - hit.distance;
            if(restMagnitude <= 0.01f) return direction * (hit.distance - 0.01f);

            Vector3 movedelta = direction * hit.distance;

            //남은 벡터로 충돌 검사
            Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude;
            return sumVector + HorizontalCollideAndSlide(projectDelta, playerPosition + movedelta , count + 1);
        }else{
            if(count == 0 && _playerMoveAxisType == EPlayerMoveAxis.XZ)
            {
                //print("!!");
                //내려가는 slope에서 경사면을 인지 못하는 경우를 대비한 것으로,
                //만약 이를 조절하고 싶다면 내려갈 때는 붕 떠서 내려가게 하는 별도의 tag를 가진 지형을 만드십시오
                if(Physics.CapsuleCast(info.point1, info.point2, info.radius, Vector3.down, out RaycastHit hit2, ydelta.magnitude + 0.01f)){
                    Vector3 projectDelta = Vector3.ProjectOnPlane(xdelta, hit2.normal).normalized;
                    //ydelta = -hit2.normal * ydelta.magnitude;

                    print(projectDelta + " "+ hit2.normal);
                    Debug.DrawRay(playerPosition, projectDelta, Color.red, 5);
                    return sumVector  + HorizontalCollideAndSlide(projectDelta * xdelta.magnitude, playerPosition , count + 1);
                }else{
                    print("no?");
                }
            }


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
        /*
        Vector3 direction = (_lookPosition - transform.position).normalized;
        Vector3 dir1 = Quaternion.AngleAxis(30, Vector3.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-30, Vector3.up) * direction;

        Gizmos.DrawRay(transform.position, direction * 6);
        Gizmos.DrawRay(transform.position, dir1 * 6);
        Gizmos.DrawRay(transform.position, dir2 * 6);
        */

/*
        Vector3 point1 = stairRayCastPoint1.position;
        Vector3 point2 = stairRayCastPoint2.position;
        Gizmos.DrawRay(point1, stairRayCastPoint1.forward * Vector3.Distance(point1, point2));
        Gizmos.DrawRay(point2, Vector3.down * stairRayCastDistance);
        if(_rigidbody!=null) Debug.DrawRay(_rigidbody.position, nextDelta, Color.blue, 5);
        if (!Physics.Raycast(point1, stairRayCastPoint1.forward * Vector3.Distance(point1, point2), Vector3.Distance(point1, point2))){
            if (Physics.Raycast(point2, Vector3.down, out RaycastHit hit2, stairRayCastDistance))
            {
                Gizmos.DrawSphere(hit2.point, 0.05f);
              //  print("stair");
            }
        }

        Vector3 point3 = slopeRayCastPoint.position;

        Gizmos.DrawRay(point3, slopeRayCastPoint.forward * PlayerDataManager.currentMoveSpeed * Time.fixedDeltaTime);
*/
    }


}
