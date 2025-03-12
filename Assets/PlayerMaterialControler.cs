using UnityEngine;

public class PlayerMaterialControler : MonoBehaviour
{
    //!!!!*********test***********!!!!//
    public Material fadeMaterial; // DistanceFadeMaterial
    RaycastHit hits;
    void Update()
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, 
        out hits, 100, LayerMask.GetMask("Building")))
        {
    
            Debug.DrawLine(Camera.main.transform.position, hits.point, Color.red);
            if (fadeMaterial != null)
            {
                fadeMaterial.SetVector("_PlayerPosition", PlayerMovement.PlayerPosition);
            }
        }


    }
}
