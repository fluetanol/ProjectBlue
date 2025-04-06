using UnityEngine;

public class ScriptableObjectTest2 : MonoBehaviour
{
    public WeaponStats weaponStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print(weaponStats[0].Damage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
