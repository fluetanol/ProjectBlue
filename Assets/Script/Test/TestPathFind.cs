using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TestPathFind : MonoBehaviour
{
    public int RaycastCount = 10;
    public Rigidbody Rigidbody;
    public CapsuleCollider CapsuleCollider;
    public float Speed;
    public float SkinWidth = 0.2f;
    public Transform Destination;
    public float PathfindTick = 0.1f;

    private float _pathfindTick = 0;
    private bool isRight;

    private NavMeshAgent agent;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agent.SetDestination(Destination.position);
    }


    void FixedUpdate()
    {
        //print(CapsuleCollider.bounds.center);
        agent.SetDestination(Destination.position);
        _pathfindTick = 0;
        // pathfinding(CapsuleCollider.bounds.center);

        // Vector3 direction = (Destination.position - Rigidbody.position).normalized;

        // Rigidbody.MovePosition(Rigidbody.position + direction * Speed * Time.fixedDeltaTime);

        // _pathfindTick += Time.fixedDeltaTime;
    
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
