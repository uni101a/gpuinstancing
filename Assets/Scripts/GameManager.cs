using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private DrawMap _drawMap;
    private MapData _mapData;

    private float x;
    private float z;

    private void Start()
    {
        _drawMap = GetComponent<DrawMap>();
        _mapData = GameObject.Find("Map").GetComponent<MapData>();
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(40, 20, 280, 180), "消したい座標");

        GUI.Label(new Rect(60, 75, 40, 20), "x :");
        x = GUI.HorizontalSlider(new Rect(90, 80, 150, 20), x, 0, _mapData.GetGridNum().x-1);
        GUI.Label(new Rect(260, 75, 40, 20), x.ToString());

        GUI.Label(new Rect(60, 115, 20, 20), "z :");
        z = GUI.HorizontalSlider(new Rect(90, 120, 150, 20), z, 0, _mapData.GetGridNum().y-1);
        GUI.Label(new Rect(260, 115, 40, 20), z.ToString());

        if (GUI.Button(new Rect(60, 160, 80, 20), "更新"))
        {
            _mapData.ChangeState(new Vector2(x, z), false);
            _drawMap.UpadateBuffers();
        }
    }
}
