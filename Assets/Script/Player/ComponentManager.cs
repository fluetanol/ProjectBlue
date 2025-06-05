using System;
using UnityEngine;


[Serializable]
public class BasicComponents
{
    public Transform Transform;
    public Rigidbody Rigidbody;
    public CapsuleCollider CapsuleCollider;
    public BoxCollider BoxCollider;
    public Animator Animator;

    public BasicComponents(Transform transform = null, Rigidbody rigidbody = null,
                           CapsuleCollider capsuleCollider = null, BoxCollider boxCollider = null,
                           Animator animator = null)
    {
        Transform = transform;
        Rigidbody = rigidbody;
        CapsuleCollider = capsuleCollider;
        BoxCollider = boxCollider;
        Animator = animator;
    }
}

public class ComponentManager : MonoBehaviour
{
    public BasicComponents objectComponents;
}
