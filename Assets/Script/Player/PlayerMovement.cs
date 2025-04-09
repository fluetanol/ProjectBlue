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
    public float skinWidth = 0.01f; //스킨 두께

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
    public float maxStepHeight = 0.5f;
    public float stepCheckDistance = 0.1f; // 계단 감지 거리


    [Space]
    [Header("For Wall Movement")]
    [Range(0,1)] public float wallFriction = 1f;  //벽 마찰, 낮을 수록 강해짐

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


    /// <summary>
    /// 계단을 올라가는 로직
    /// </summary>
    /// <param name="delta">다음 이동 델타</param>
    /// <returns>다음 계단 위치로 가기 위한 delta벡터를 반환합니다. 계단이 아니라면 영벡터를 반환함.</returns>
    private Vector3 StairStepUp(Vector3 delta){
        Vector3 stepUp = Vector3.zero;
        Vector3 direction = delta.normalized;


        Vector3 stepcheck = 
            _rigidbody.position + delta + 
            Vector3.up * maxStepHeight + 
            direction * stepCheckDistance;


        /*

                           pos
                     [     ** ------>|Ray   |-----           플레이어 진행방향으로 Ray를 쏘는데, 이때 max step height만큼 높은 위치에서 쏜다.
               max   [     **        |      |                ray가 충돌하지 않는 다는 것은 계단 형태로 인해 음푹 들어가서 발 디딜곳이 존재한다는 것이다.
              step   [     **  |-----v------|                반대로 충돌한다면 이는 벽이거나, 계단이여도 계단의 경사가 너무 가파르다는 것이다.
            height   [     **  |    Ray2                     
                    ==============================            만약 ray가 충돌 하지 않아 계단임이 확인 되었다면 이번엔 아랫방향으로 ray를 쏜다.
                                                              ray가 충돌 했을 때 평평한 면(Vector3.up)이면 우리가 생각하는 계단이므로 다음 위치치 y값을 반환하는 형태이다.
                                                              다만 이건 내가 "계단은 무조건 90도여야 한다"라는 규정을 내렸기 때문에 가능한거고
                                                              평탄하지 않은 계단 설정이 있다면(이를테면 조금 높은 바위 언덕) 이라면 좀 더 다양한 로직이 필요할 것.
        */

        Ray ray = new Ray(_rigidbody.position + delta + Vector3.up * maxStepHeight, direction);

        
        Debug.DrawRay(_rigidbody.position + delta + Vector3.up * maxStepHeight, direction * stepCheckDistance, Color.green, 5);
        Debug.DrawRay(stepcheck, Vector3.down * maxStepHeight, Color.cyan, 5);

        if(!Physics.Raycast(ray, stepCheckDistance)){
            Ray ray2 = new Ray(stepcheck, Vector3.down);
            
            if(Physics.Raycast(ray2, out RaycastHit hit, maxStepHeight)){
                if(hit.normal == Vector3.up){
                    print("stair");
                    stepUp = new Vector3(0, hit.point.y - _rigidbody.position.y, 0);
                }
            }
        }
        return stepUp;
    }


    /// <summary>
    /// 수평방향 충돌 및 슬라이드
    /// </summary>
    /// <param name="delta">다음 프레임의 velocity 또는 slide한 후 잔여 벡터</param>
    /// <param name="playerPosition">collide and slide 후 플레이어가 있을 위치</param>
    /// <param name="count">재귀 횟수</param>
    /// <param name="maxRecursion">최대 collide and slide 횟수</param>
    /// <returns>재귀가 끝나거나 특정 종료 조건을 맞이한 후 반환되는 잔여 벡터</returns>
    private Vector3 HorizontalCollideAndSlide(Vector3 delta, Vector3 playerPosition, int count, int maxRecursion = 3)
    {
        if (count >= maxRecursion) return delta;

        CapsuleInfo info = CapsuleCastInfo(playerPosition);
        Vector3 direction = delta.normalized;
        Vector3 sumVector = Vector3.zero;

        //Notice!!!
        //skin width의 개념은 매우 중요합니다.
        //0.01f의 여유분을 두고 충돌 검사를 하고, 다음 위치는 그 skin width만큼 다시 뺀 위치로,
        //즉, "덜 충돌한 듯한" 느낌을 주어야 충돌판정이 boundary 위치에서 잘못된 cast가 일어나 노클립하는 현상을 방지할 수 있습니다.
        //물론 최대한 덜 빼야 속도 손실이 덜하긴 하지만, 아무리 못해도 (skin width / 재귀 횟수) 만큼은 빼서 위치를 보정해주세요.
        if (Physics.CapsuleCast(info.point1, info.point2, info.radius, direction, out RaycastHit hit, delta.magnitude + skinWidth)){
            
            //hit하고 남은 거리와 벡터
            float restMagnitude = Math.Abs(delta.magnitude - hit.distance);

            //계단을 올라가거나, 벽에 부딪혔거나 둘 중 하나나
            if (Vector3.Angle(hit.normal, Vector3.up) > 75)
            {
                print("slope or wall");
                Vector3 stepup = StairStepUp(delta);

                // TODO: 계단을 올라가는 경우엔 Y값을 변동시킨 상태로 계산
                //계단을 올라가는 경우가 아니라면 사영시킨 벡터로 변환
                //-> 벽과 계단을 구분하는 로직이며 벽에서 비빌 때는 미끄러지듯 부드럽게 움직이도록 하는 코드입니다.
                // 물론, 마찰을 쎄게 주고 싶다면 restMagnitude에 마찰 가중치를 주면 됩니다.
                if(Vector3.zero == stepup){
                    Vector3 temp = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude * wallFriction;
                    temp.y = 0;
                    return temp;
                }
                
               else{
                    return  delta + stepup;
                }

            }

            if (restMagnitude <= 0.01f) {
                return direction * (hit.distance - 0.003f);
            }


            Vector3 movedelta = direction * (hit.distance - 0.003f);
            //남은 벡터로 충돌 검사
            Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude;
            sumVector += HorizontalCollideAndSlide(projectDelta, playerPosition + movedelta, count + 1) + movedelta;
            
            return sumVector;
        }
        else{
            if(count == 0 && _playerMoveAxisType == EPlayerMoveAxis.XZ)
            {
                //TODO: 내려가는 slope에서 경사면을 인지 못하는 경우를 대비한 것으로,
                //만약 이를 조절하고 싶다면 내려갈 때는 붕 떠서 내려가게 하는 별도의 tag를 가진 지형을 만드십시오
                if (Physics.CapsuleCast(info.point1, info.point2, info.radius, Vector3.down, out RaycastHit hit2, ydelta.magnitude + 0.01f)){
                    Vector3 projectDelta = Vector3.ProjectOnPlane(xdelta, hit2.normal).normalized;
                    //ydelta = -hit2.normal * ydelta.magnitude;
                    //print(projectDelta + " "+ hit2.normal);
                    //Debug.DrawRay(playerPosition, projectDelta, Color.red, 5);
                    return sumVector  + HorizontalCollideAndSlide(projectDelta * xdelta.magnitude, playerPosition , count + 1);
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

            Vector3 movedelta = direction * (hit.distance - 0.01f);

            //남은 벡터로 충돌 검사
            Vector3 projectDelta = Vector3.ProjectOnPlane(delta, hit.normal).normalized * restMagnitude;
            return sumVector += HorizontalCollideAndSlide(projectDelta, playerPosition + movedelta, count + 1) + movedelta;
            
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
       // print("??!");
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

        Gizmos.DrawWireSphere(transform.position, 0.1f);


        Vector3 rayStart = transform.position + Vector3.up * maxStepHeight;

        Gizmos.DrawLine(transform.position, rayStart);

        //Vector3 rayDir = new Vector3(direction.x, -maxStepHeight, direction.z).normalized;
    


    }


}
