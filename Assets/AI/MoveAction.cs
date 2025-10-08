using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move", story: "[Agent] Move [Target] [Speed]", category: "Action", id: "6828c74edbae252cc4a7a4024bf995d8")]
public partial class MoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Target;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> StopDistance;
    [SerializeReference] public BlackboardVariable<bool> IsArrived;

    private Vector3 direction;
    
    protected override Status OnStart()
    {
        Debug.Log("on start");
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        direction = (Target.Value.position - Agent.Value.transform.position).normalized;

        Agent.Value.transform.position += direction * Speed.Value * Time.deltaTime;

        if (Vector3.Distance(Agent.Value.transform.position, Target.Value.position) < StopDistance.Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        Debug.Log("on end");
        IsArrived.Value = true;
    }
}

