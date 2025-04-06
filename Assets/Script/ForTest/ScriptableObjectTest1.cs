using UnityEngine;

public class ScriptableObjectTest1 : MonoBehaviour
{
    public WeaponStats weaponStats;

    void Awake()
    {
        print(weaponStats[0].Damage);
        WeaponStats.WeaponInfo info =  weaponStats[0];
    }

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
