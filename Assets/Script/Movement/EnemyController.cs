using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemyController : MonoBehaviour
{
    public List<Rigidbody> testObjects;
    public Rigidbody[] rigidbodys;
    public Transform parent;
    public float speed = 1.5f;

    private NativeArray<Vector3> positions;
    private NativeArray<Vector3> velocitys;


    void Start()
    {
        parent.GetComponentsInChildren<Rigidbody>(true, testObjects);
        rigidbodys = new Rigidbody[testObjects.Count];
        for(int i=0; i< testObjects.Count; i++){
            rigidbodys[i] = testObjects[i].GetComponentInChildren<Rigidbody>();
        }
        
        //for job2 case   
        positions = new NativeArray<Vector3>(testObjects.Count, Allocator.Persistent);
        velocitys = new NativeArray<Vector3>(testObjects.Count, Allocator.Persistent);
        for (int i=0; i< testObjects.Count; i++){
            positions[i] = rigidbodys[i].position;
        }
    }

    void FixedUpdate()
    {   
        MoveByJob();
    }

    private void MoveByJob()
    {
        var moveJob = new MoveJob
        {
            //targetPosition = PlayerDataManager.Instance.transform.position,
            deltaTime = Time.fixedDeltaTime,
            speed = speed,
            positions = positions,
            velocity = velocitys 
        };

        
        // Job 스케줄링: 모든 Transform에 대해 병렬 실행
        JobHandle jobHandle = moveJob.Schedule(testObjects.Count, 8);
        jobHandle.Complete(); // Job 완료 대기 (필요에 따라 비동기 처리도 가능)

        for(int i=0; i< testObjects.Count; i++){
            rigidbodys[i].MovePosition(rigidbodys[i].position + velocitys[i]);
            positions[i] = rigidbodys[i].position; // 업데이트된 위치를 저장합니다.
        }
    }

    void OnDestroy()
    {
        positions.Dispose();
        velocitys.Dispose();
    }

    [BurstCompile]
    struct MoveJob : IJobParallelFor
    {
        public Vector3 targetPosition;
        public float deltaTime;
        public float speed;
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> velocity;

        public void Execute(int index)
        {
            Vector3 currentPos = positions[index];
            Vector3 direction = (targetPosition - currentPos).normalized;
            direction.y = 0; // y축 방향은 무시
            velocity[index] = direction * speed * deltaTime;
        }
    }
}



[BurstCompile]
struct MoveJobByTransform : IJobParallelForTransform
{
    public Vector3 targetPosition;
    public float deltaTime;
    public float speed;

    public void Execute(int index, TransformAccess transform)
    {
        Vector3 currentPos = transform.position;
        // 타겟 위치 방향으로 단위 벡터 계산
        Vector3 direction = (targetPosition - currentPos).normalized;
        // 단순히 일정 속도로 이동하도록 계산 (더 복잡한 로직도 가능)
        direction.y = 0; // y축 방향은 무시
        transform.position = currentPos + direction * speed * deltaTime;
        // print(transform.position +" " + "!!??");
    }

    /*
    private void MoveJobTest()
    {
        var moveJob = new MoveJob
        {
            targetPosition = PlayerDataManager.Instance.transform.position,
            deltaTime = Time.fixedDeltaTime,
            speed = speed
        };

        // Job 스케줄링: 모든 Transform에 대해 병렬 실행
        JobHandle jobHandle = moveJob.Schedule(transformAccessArray);
        jobHandle.Complete(); // Job 완료 대기 (필요에 따라 비동기 처리도 가능)
    }*/
}