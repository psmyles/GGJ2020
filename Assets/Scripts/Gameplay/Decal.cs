using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Decal : MonoBehaviour
{
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    private List<Vector3> m_Points;
    private Vector3 m_UpVector = Vector3.up;

    [SerializeField]
    private float m_Thickness = 1.0f;

    [SerializeField]
    private float m_DestroyTime = 1.0f;

    bool m_DestroyStarted = false;

    public Vector3 UpVector
    {
        get 
        {
            return m_UpVector;   
        }
        set
        {
            m_UpVector = value;
        }
    }

    public float Thickness
    {
        get
        {
            return m_Thickness;
        }
        set
        {
            m_Thickness = value;
        }
    }

    public int PointsCount
    {
        get
        {
            return (m_Points == null) ? 0 : m_Points.Count;
        }
    }

    void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDestroy()
    {
        if (m_MeshFilter.sharedMesh != null)
        {
            Mesh.Destroy(m_MeshFilter.sharedMesh);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DestroyStarted)
        {
            m_DestroyTime -= Time.deltaTime;
            if (m_DestroyTime <= 0.0f)
            {
                //Debug.Log("#### Destroyed : " + gameObject.name);
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void AddPoint(Vector3 point)
    {
        if (m_Points == null)
            m_Points = new List<Vector3>();

        m_Points.Add(point);
        RefreshMesh();
    }

    public void AddPoints(List<Vector3> points)
    {
        if (m_Points == null)
            m_Points = new List<Vector3>();

        m_Points.AddRange(points);
        RefreshMesh();
    }

    public Vector3 GetPoint(int Index)
    {
        if (m_Points == null)
            return Vector3.zero;

        if (Index >= 0 && Index < m_Points.Count)
            return m_Points[Index];

        return Vector3.zero;
    }

    public void ResetPoints()
    {
        m_Points.Clear();
        RefreshMesh();
    }

    private void RefreshMesh()
    {
        if (m_MeshFilter.sharedMesh == null)
        {
            m_MeshFilter.sharedMesh = new Mesh();
        }

        List<Vector3> vertices;
        List<Vector2> uvs;
        List<int> indices;
        MeshUtils.CreateLineMesh(m_Points, m_UpVector, m_Thickness, out vertices, out uvs, out indices);

        if (vertices != null && uvs != null && indices != null)
        {
            m_MeshFilter.sharedMesh.SetVertices(vertices);
            m_MeshFilter.sharedMesh.SetUVs(0, uvs);
            m_MeshFilter.sharedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        }
    }

    public void SetToDestroy()
    {
        m_DestroyStarted = true;
        //Debug.Log("#### Set to destroy : " + gameObject.name);
    }
}
