using UnityEngine;

public class PlayerAnimationControl : MonoBehaviour
{

    private Animator animator;
    public LayerMask groundLayer;
    public float raycastDistance = 1.0f;
    public float heightOffset = 0.1f; // 발이 지면에 파고드는 것을 방지
    public float lerpSpeed = 5.0f; // 부드러운 보간 속도
    
    public float distanceGround = 0.1f;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        
        print("OnAnimatorIK");
        if (animator)
        {
            // Left Foot
            // Position 과 Rotation weight 설정
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

            ///<summary>
            /// GetIKPosition 
            ///   => IK를 하려는 객체의 위치 값 ( 아래에선 아바타에서 LeftFoot에 해당하는 객체의 위치 값 )
            /// Vector3.up을 더한 이유 
            ///   => origin의 위치를 위로 올려 바닥에 겹쳐 바닥을 인식 못하는 걸 방지하기 위해
            ///      (LeftFoot이 발목 정도에 있기 때문에 발바닥과 어느 정도 거리가 있고, Vector3.up을 더해주지 않으면 발목 기준으로 처리가 되어 발 일부가 바닥에 들어간다.)
            ///</summary>
            Ray leftRay = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            // distanceGround: LeftFoot에서 땅까지의 거리
            // +1을 해준 이유: Vector3.up을 해주었기 때문
            if (Physics.Raycast(leftRay, out RaycastHit leftHit, distanceGround + 1.5f, groundLayer))
            {
                // 걸을 수 있는 땅이라면
                //if (leftHit.transform.tag == "WalkableGround")
                {
                    Vector3 footPosition = leftHit.point;
                    footPosition.y += distanceGround;

                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, leftHit.normal));
                }
            }

            // Right Foot
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

            Ray rightRay = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(rightRay, out RaycastHit rightHit, distanceGround + 1.5f, groundLayer))
            {
                //if (rightHit.transform.tag == "WalkableGround")
                {
                    Vector3 footPosition = rightHit.point;
                    footPosition.y += distanceGround;

                    animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightHit.normal));
                }
            }
        }
        
    }

/*
    void AdjustFoot(AvatarIKGoal foot)
    {
        // 발의 현재 IK 위치 가져오기
        Vector3 footPosition = animator.GetIKPosition(foot);
        Quaternion footRotation = animator.GetIKRotation(foot);

        // 레이캐스트 시작점 (발 위치 + 약간의 오프셋)
        Vector3 rayStart = footPosition + Vector3.up * heightOffset;
        Ray ray = new Ray(rayStart, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance + heightOffset, groundLayer))
        {
            // 경사면의 노멀 벡터와 충돌 지점
            Vector3 targetPosition = hit.point;
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * footRotation;



            // 부드러운 보간 적용
            footPosition = Vector3.Lerp(footPosition, targetPosition, lerpSpeed * Time.deltaTime);
            footRotation = Quaternion.Slerp(footRotation, targetRotation, lerpSpeed * Time.deltaTime);
        }

        // IK 위치와 회전 설정
        animator.SetIKPositionWeight(foot, 1.0f);
        animator.SetIKRotationWeight(foot, 1.0f);
        animator.SetIKPosition(foot, footPosition);
        animator.SetIKRotation(foot, footRotation);

    }


    void AdjustBodyHeight()
    {
        // 양발의 평균 위치 계산
        Vector3 leftFoot = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFoot = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        float averageHeight = (leftFoot.y + rightFoot.y) / 2;

        // 몸통 위치 조정
        //Vector3 bodyPosition = animator.bodyPosition;
        //bodyPosition.y = Mathf.Lerp(bodyPosition.y, averageHeight, lerpSpeed * Time.deltaTime);
        //animator.bodyPosition = bodyPosition;
    }
*/

}
