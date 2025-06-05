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

public class PlayerComponentManager : MonoBehaviour
{
    public BasicComponents BasicComponents;
    public LineRenderer LineRenderer;

    public Rigidbody Rigidbody
    {
        get => BasicComponents.Rigidbody;
        private set {}
    }

    public CapsuleCollider CapsuleCollider
    {
        get => BasicComponents.CapsuleCollider;
        private set {}
    }
    public BoxCollider BoxCollider
    {
        get => BasicComponents.BoxCollider;
        private set {}
    }
    public Animator Animator
    {
        get => BasicComponents.Animator;
        private set{}
    }
    public Transform Transform
    {
        get => BasicComponents.Transform;
        private set {}
    }

}
