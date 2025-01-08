using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    public Vector2 roomSize = new Vector2(1, 1);
    private Vector2 tempSize;
    public Vector2 wallSize = new Vector2(0.5f, 2);

    public Mesh wallMesh;
    public Mesh wallMesh2;
    public Material wallMaterial;

    List<Matrix4x4> wallMatricesN;
    List<Matrix4x4> wallMatricesNB;
    public int seed;

    private BoxCollider _boxCollider;

    void Start(){
        tempSize = roomSize;
        _boxCollider    = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tempSize != roomSize){
            createWalls();
            tempSize = roomSize;
        }
        renderWalls();
    }

    void renderWalls(){
        if (wallMatricesN != null){
            Graphics.DrawMeshInstanced(wallMesh, 0, wallMaterial, wallMatricesN.ToArray(), wallMatricesN.Count);
        }
        if(wallMatricesNB != null){
            Graphics.DrawMeshInstanced(wallMesh2, 0, wallMaterial, wallMatricesNB.ToArray(), wallMatricesNB.Count);
        }
        Vector3 size = _boxCollider.size;
        size.x = roomSize.x;
        _boxCollider.size = size;
    }


    void createWalls(){
        Random.InitState(seed);
        wallMatricesN = new List<Matrix4x4>();
        wallMatricesNB = new List<Matrix4x4>();

        int wallCountX = Mathf.Max(1, (int)(roomSize.x/wallSize.x));
        float Xscale = (roomSize.x/wallCountX) / wallSize.x;

        for(int i=0; i<wallCountX; i++){
            var t = transform.position + new Vector3(-roomSize.x/2 + wallSize.x/2 + i * wallSize.x, 0, -roomSize.y/2);
            var r = transform.rotation;
            var s = new Vector3(Xscale, 1, 1);
            var mat = Matrix4x4.TRS(t, r, s);


            var rand = Random.Range(0, 2);
            if(rand == 1){
                wallMatricesN.Add(mat);
            }
            else{
                wallMatricesNB.Add(mat);
            }
        }

    }



}
