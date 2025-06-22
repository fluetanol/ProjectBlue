using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralGenerator : MonoBehaviour
{
    [Header("Room Size")]
    public Vector2 roomSize = new Vector2(1, 1);
    public Vector2 wallSize = new Vector2(0.5f, 2);
    private Vector2 tempSize;

    //좌, 우, 상, 하 순서로 들어갑니다.
    [Header("wall Meshes and Materials")]
    [SerializeField] private List<Mesh>         _wallMeshes;
    [SerializeField] private List<Material>     _wallMaterials;
    [SerializeField] private List<BoxCollider> _wallColliders;
    private List<Matrix4x4> _wallMatrices;

    [Header("floor Meshes and Materials")]
    [SerializeField] private List<Mesh>         _floorMeshes;
    [SerializeField] private List<Material>     _floorMaterials;
    [SerializeField] private List<BoxCollider> _floorColliders;


    public Mesh wallMesh;
    public Mesh wallMesh2;
    public Material wallMaterial;

    List<Matrix4x4> wallMatricesN;
    List<Matrix4x4> wallMatricesNB;
    public int seed;

    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private BoxCollider _boxCollider2;


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

        Vector3 center = _boxCollider.center;
        center.z =roomSize.y/2;
        _boxCollider.center = center;

        Vector3 center2 = _boxCollider2.center;
        center2.z = -roomSize.y/2;
        _boxCollider2.center = center2;
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

            var t2 = transform.position + new Vector3(-roomSize.x / 2 + wallSize.x / 2 + i * wallSize.x, 0, roomSize.y / 2);
            r = transform.rotation * Quaternion.Euler(0, 180, 0);
            var mat2 = Matrix4x4.TRS(t2, r, s);
            var rand2 = Random.Range(0, 2);
            if (rand2 == 1)
            {
                wallMatricesN.Add(mat2);
            }
            else
            {
                wallMatricesNB.Add(mat2);
            }
        }


        int wallCountY = Mathf.Max(1, (int)(roomSize.y/wallSize.y));
        float Yscale = (roomSize.y/wallCountY) / wallSize.y;

        for(int i=0; i<wallCountY; i++){
            var t = transform.position + new Vector3(-roomSize.x/2, 0, -roomSize.y/2 + wallSize.y/2 + i * wallSize.y);
            var r = Quaternion.Euler(0, 90, 0);
            var s = new Vector3(Yscale, 1, 1);
            var mat = Matrix4x4.TRS(t, r, s);

            var rand = Random.Range(0, 2);
            if(rand == 1){
                wallMatricesN.Add(mat);
            }
            else{
                wallMatricesNB.Add(mat);
            }

            var t2 = transform.position + new Vector3(roomSize.x / 2, 0, -roomSize.y / 2 + wallSize.y / 2 + i * wallSize.y);
            var mat2 = Matrix4x4.TRS(t2, r, s);
            var rand2 = Random.Range(0, 2);
            if (rand2 == 1)
            {
                wallMatricesN.Add(mat2);
            }
            else
            {
                wallMatricesNB.Add(mat2);
            }
        }




    }



}
