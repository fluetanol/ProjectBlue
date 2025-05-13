using UnityEngine;

enum test
{
    A = 3,
    B = 4,
    E = 3,
    C = 0b0001,
    D = 0b0011
}

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(test.A == test.E){
            print("A == E");
        }
        else{
            print("A != E");
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
