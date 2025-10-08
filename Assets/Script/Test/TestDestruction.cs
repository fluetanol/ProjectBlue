using UnityEngine;

public class TestDestruction : MonoBehaviour, IForceable
{
    public void Airborne(float force)
    {
        throw new System.NotImplementedException();
    }

    public void Knockback(Vector3 direction, float force)
    {
        // Implement knockback logic here
        // For example, apply a force to the rigidbody in the specified direction
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on the object to apply knockback.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
