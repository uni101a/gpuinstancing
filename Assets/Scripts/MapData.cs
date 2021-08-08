using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    private Vector3 mapScale = new Vector3(1, 1, 1);
    private Vector2 gridNum = new Vector2(0, 0);
    private Vector3 lowerLeftPosition = new Vector3(0, 0, 0);

    private bool[,] _status;
    private int _scaling = 10;

    private void Start()
    {
        mapScale = transform.Find("Mesh").gameObject.transform.localScale;
        gridNum = new Vector2(mapScale.x, mapScale.z) * _scaling;
        lowerLeftPosition = new Vector3(transform.position.x - (mapScale.x * _scaling)/2, 0, transform.position.z - (mapScale.z * _scaling)/2);

        InitializeStatus();
    }

    private void InitializeStatus()
    {
        int x = (int)gridNum.x, y =(int)gridNum.y;
        _status = new bool[y, x];

        for(int i=0; i<y; i++)
        {
            for(int n=0; n<x; n++)
            {
                _status[i, n] = true;
            }
        }
    }

    public void ChangeState(Vector2 pos, bool state)
    {
        _status[(int)pos.y, (int)pos.x] = state;
    }

    public bool GetGridState(Vector2 pos)
    {
        return _status[(int)pos.y, (int)pos.x];
    }

    public Vector2 GetGridNum()
    {
        return gridNum;
    }

    public int GetActiveGridNum()
    {
        int let = 0;

        foreach(bool state in _status)
        {
            if (state) let++;
        }

        return let;
    }

    public Vector3 GetLowerLeftPosition()
    {
        return lowerLeftPosition;
    }
}
