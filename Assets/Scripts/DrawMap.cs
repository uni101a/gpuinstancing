using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawMap : MonoBehaviour
{
    [SerializeField] Mesh m_mesh = default;

    [SerializeField] Material m_instanceMaterial = default;

    [SerializeField] Bounds m_bounds = default;

    [SerializeField] ShadowCastingMode m_shadowCastingMode = default;

    [SerializeField] bool m_receiveShadows = false;

    [SerializeField] string layerName = "Default";

    [SerializeField] private Camera m_camera = default;

    private int m_instanceCount = 0;
    private Vector2 initialMapGrid = Vector2.zero;

    private ComputeBuffer m_argsBuffer;
    private ComputeBuffer m_positionBuffer;
    private ComputeBuffer m_eulerAngleBuffer;

    private int m_layer;

    private MapData _mapData;
    private float _meshScale = 1.0f;
    private Vector3 _mapLowerLeftPos;


    void Start()
    {
        m_layer = LayerMask.NameToLayer(layerName);

        //mapのグリッド数の立方体を描画
        _mapData = GameObject.Find("Map").GetComponent<MapData>();
        initialMapGrid = _mapData.GetGridNum();
        m_instanceCount = (int)(initialMapGrid.x * initialMapGrid.y);

        _mapLowerLeftPos = _mapData.GetLowerLeftPosition();

        InitializeArgsBuffer();
        InitializePositionBuffer();
        InitializeEulerAngleBuffer();
    }

    void Update()
    {

        Graphics.DrawMeshInstancedIndirect(
            m_mesh,
            0,
            m_instanceMaterial,
            m_bounds,
            m_argsBuffer,
            0,
            null,
            m_shadowCastingMode,
            m_receiveShadows/**,
            m_layer,
            m_camera,
            LightProbeUsage.BlendProbes,
            null*/
            );

    }

    public void UpadateBuffers()
    {
        m_instanceCount = _mapData.GetActiveGridNum();

        UpdateArgsBuffer();
        UpdatePositionBuffer();
        UpdateEulerAngleBuffer();
    }

    private void InitializeArgsBuffer()
    {
        UpdateArgsBuffer();
    }
    private void InitializePositionBuffer()
    {
        UpdatePositionBuffer();
    }

    private void InitializeEulerAngleBuffer()
    {
        UpdateEulerAngleBuffer();
    }

    private void UpdateArgsBuffer()
    {
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        uint numIndices = (m_mesh != null) ? (uint)m_mesh.GetIndexCount(0) : 0;

        args[0] = numIndices;
        args[1] = (uint)m_instanceCount;

        m_argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        m_argsBuffer.SetData(args);
    }


    private void UpdatePositionBuffer()
    {
        Vector4[] positions = new Vector4[m_instanceCount];

        int xi = -1, zi = -1;

        for(int i = 0; i < m_instanceCount; i++)
        {
            xi = (int)((xi + 1) % _mapData.GetGridNum().x);
            if (xi % _mapData.GetGridNum().x == 0) zi++;
            Vector2 gridPos = GetNextActiveGridPos(new Vector2(xi, zi));

            xi = xi != (int)gridPos.x ? (int)gridPos.x : xi;
            zi = zi != (int)gridPos.y ? (int)gridPos.y : zi;

            if (gridPos.x > -1.0f)
            {
                positions[i].x = _mapLowerLeftPos.x + _meshScale / 2 + gridPos.x;
                positions[i].y = -_meshScale / 2;
                positions[i].z = _mapLowerLeftPos.z + _meshScale / 2 + gridPos.y;
                positions[i].w = _meshScale;
            }
        }

        m_positionBuffer = new ComputeBuffer(m_instanceCount, 4 * 4);
        m_positionBuffer.SetData(positions);

        m_instanceMaterial.SetBuffer("positionBuffer", m_positionBuffer);
    }

    private void UpdateEulerAngleBuffer()
    {
        Vector3[] angles = new Vector3[m_instanceCount];

        for (int i = 0; i < m_instanceCount; i++)
        {
            angles[i] = new Vector3(0, 0, 0);
        }

        m_eulerAngleBuffer = new ComputeBuffer(m_instanceCount, 4 * 3);
        m_eulerAngleBuffer.SetData(angles);

        m_instanceMaterial.SetBuffer("eulerAngleBuffer", m_eulerAngleBuffer);
    }

    private Vector2 GetNextActiveGridPos(Vector2 previousGridPos)
    {
        for(int y = (int)previousGridPos.y; y < (int)initialMapGrid.y; y++)
        {
            for(int x = (int)previousGridPos.x; x < (int)initialMapGrid.x; x++)
            {
                if (_mapData.GetGridState(new Vector2(x, y)))
                    return new Vector2(x, y);
            }
        }

        return new Vector2(-1.0f, -1.0f);
    }



    private void OnDisable()
    {
        if (m_eulerAngleBuffer != null)
            m_eulerAngleBuffer.Release();
        m_eulerAngleBuffer = null;

        if (m_positionBuffer != null)
            m_positionBuffer.Release();
        m_positionBuffer = null;

        if (m_argsBuffer != null)
            m_argsBuffer.Release();
        m_argsBuffer = null;

    }
}
