using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class ProcedualRandomMesh : MonoBehaviour
{
    // 三角形の個数
    [SerializeField] private int triangleCount;

    // 三角形の大きさ
    [SerializeField] private float triangleScale;

    // メッシュの大きさ
    [SerializeField] private Vector3 meshScale;

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化 
    /// </summary>
    public void Initialize()
    {
        Vector3[] vertices = new Vector3[triangleCount * 3];
        int[] triangles = new int[triangleCount * 3];
        int pos = 0;
        for (int i = 0; i < triangleCount; i++)
        {
            Vector3 v1 = Vector3.Scale(new Vector3(Random.value, Random.value, Random.value) - Vector3.one * 0.5f,
                this.meshScale);
            Vector3 v2 = v1 + new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * triangleScale;
            Vector3 v3 = v1 + new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * triangleScale;

            vertices[pos] = v1;
            vertices[pos + 1] = v2;
            vertices[pos + 2] = v3;

            triangles[pos] = pos;
            triangles[pos + 1] = pos + 1;
            triangles[pos + 2] = pos + 2;
            pos += 3;
        }

        //メッシュ生成
        Mesh mesh = new Mesh {vertices = vertices, triangles = triangles};

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private float randomness = 0.1f;
    private Vector3 offsetPosition;

    void Update()
    {
        offsetPosition.x = Random.value;
        offsetPosition.z = Random.value;
        transform.localPosition = offsetPosition * randomness;
    }
}