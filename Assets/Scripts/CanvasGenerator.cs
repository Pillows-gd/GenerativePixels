using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class CanvasGenerator : MonoBehaviour
{
    [Range(0,1)]
    [SerializeField] private float scale = 1;
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    private int[,] canvas;
    private int pixelsPerUnit = 100;
    private Vector3 bottomLeft;
    private Vector3 topRight;

    private void Start()
    {
        InitializeCanvas();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pointedPos = Input.mousePosition;

            // To Center
            pointedPos.x -= Screen.width / 2;
            pointedPos.y -= Screen.height / 2;
            
            // To Units
            pointedPos /= pixelsPerUnit;

            // To [0-1]
            pointedPos.x += width * scale / 2;
            pointedPos.y += height * scale / 2;
            pointedPos.x /= width * scale;
            pointedPos.y /= height * scale;
            
            // To Index
            pointedPos.x *= width;
            pointedPos.y *= height;

            canvas[(int)pointedPos.x, (int)pointedPos.y] = 1;

            print("Bottom Left = " + bottomLeft + ", Top Right = " + topRight + ". Pointing: " + pointedPos);
        }
    }

    private void InitializeCanvas()
    {
        canvas = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                canvas[x, y] = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (canvas != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(-width*.5f + .5f + x, -height*.5f + .5f + y, 0.0f);

                    if (x == 0 && y == 0)
                        bottomLeft = new Vector3(pos.x - .5f, pos.y - .5f, 0f) * scale;
                    if (x == width - 1 && y == height - 1)
                        topRight = new Vector3(pos.x + .5f, pos.y + .5f, 0f) * scale;

                    Gizmos.color = canvas[x,y] == 0 ? Color.black : Color.yellow;
                    Gizmos.DrawCube(pos * scale, Vector3.one * scale);
                }
            }
        }
    }

}
