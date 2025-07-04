using UnityEngine;

[ExecuteAlways]
public class tESTMATERITL : MonoBehaviour
{
    // void OnDrawGizmos()
    // {
    //     MeshFilter mf = GetComponent<MeshFilter>();
    //     if (mf == null || mf.sharedMesh == null) return;

    //     Mesh mesh = mf.sharedMesh;
    //     Vector3[] vertices = mesh.vertices;
    //     Vector2[] uvs = mesh.uv;

    //     Gizmos.color = Color.green;
    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         Vector3 worldPos = transform.TransformPoint(vertices[i]);
    //         string uvText = $"UV({uvs[i].x:F2}, {uvs[i].y:F2})";
    //         UnityEditor.Handles.Label(worldPos, uvText, new GUIStyle
    //         {
    //             normal = new GUIStyleState { textColor = Color.black },
    //             fontSize = 12,
    //             alignment = TextAnchor.MiddleCenter
    //         });

    //     }
    // }
}