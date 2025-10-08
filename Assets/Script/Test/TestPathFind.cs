using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TestPathFind : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public CapsuleCollider CapsuleCollider;
    public float Speed;
    public float SkinWidth = 0.2f;
    public Transform Destination;
    
    public float PathfindTick = 0.1f;

    private bool isRight;

    public NavMeshAgent agent;

    public NavMeshPath path;



    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    void Start()
    {
        //agent.SetDestination(Destination.position);
        print(agent.isOnNavMesh +" " + path.status);
        if (agent.CalculatePath(Destination.position, path))
        {
            print("Path status: " + path.status);
            agent.SetPath(path);

            var corners = path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.green, 30f);
            }

        }
       
    }

    float _deltaTick = 0;

    void FixedUpdate()
    {
        //print(CapsuleCollider.bounds.center);
        //agent.SetDestination(Destination.position);

        // pathfinding(CapsuleCollider.bounds.center);

        // Vector3 direction = (Destination.position - Rigidbody.position).normalized;

        // Rigidbody.MovePosition(Rigidbody.position + direction * Speed * Time.fixedDeltaTime);

        // _pathfindTick += Time.fixedDeltaTime;

        _deltaTick+=Time.fixedDeltaTime;
        if (_deltaTick >= PathfindTick && agent.CalculatePath(Destination.position, path))
        {
            agent.SetPath(path);
            _deltaTick = 0;
            print("[NavMesh] Path status: " + path.status);
        }
    }


    private void pathfinding(Vector3 position)
    {

        float height = CapsuleCollider.height;
        float radius = CapsuleCollider.radius;
        float halfHeight = height / 2 - radius;
        Vector3 point1 = position + Vector3.up * halfHeight;
        Vector3 point2 = position - Vector3.up * halfHeight;
        Vector3 direction = (Destination.position - position).normalized;


        if (Physics.CapsuleCast(point1, point2, radius + SkinWidth, direction, out RaycastHit hit, 1f))
        {
            Vector3 hitpoint = hit.point;
            Vector3 normal = hit.normal;



            if (isRight) direction = new Vector3(-normal.z, 0, normal.x).normalized;
            else direction = new Vector3(normal.z, 0, -normal.x).normalized;


            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Player") &&
            Physics.Raycast(hitpoint + normal * 0.5f, direction, 0.5f))
            {
                direction = -direction;
                isRight = !isRight;
            }
            
            

        }


        Rigidbody.MovePosition(Rigidbody.position + direction * Speed * Time.fixedDeltaTime);
    }
    
}
