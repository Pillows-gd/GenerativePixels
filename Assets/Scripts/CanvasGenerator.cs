using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class CanvasGenerator : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float scale = 0.08f;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 72;
    private int[,] canvas;
    private int pixelsPerUnit = 108; // Not 100! This makes the pointer more precise. I dunno why D:

    private void Start()
    {
        InitializeCanvas();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int[] pointedCoords = getPointedCoords();

            try
            {
                canvas[pointedCoords[0], pointedCoords[1]] = 1;
            }
            catch (Exception)
            {
                print("Pointing out of the canvas!");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            int[] pointedCoords = getPointedCoords();

            try
            {
                canvas[pointedCoords[0], pointedCoords[1]] = 0;
            }
            catch (Exception)
            {
                print("Pointing out of the canvas!");
            }
        }
    }

    int[] getPointedCoords()
    {
        double pointedX = Input.mousePosition.x;
        double pointedY = Input.mousePosition.y;

        // Origin To Center
        pointedX -= Screen.width / 2;
        pointedY -= Screen.height / 2;

        // Pixels To Units
        pointedX /= pixelsPerUnit;
        pointedY /= pixelsPerUnit;

        // Origin To Corner
        pointedX += (double)width * scale / 2;
        pointedY += (double)height * scale / 2;

        // Units To Matrix Indices
        pointedX /= scale;
        pointedY /= scale;

        int[] coords = new int[2];
        coords[0] = (int)pointedX;
        coords[1] = (int)pointedY;

        return coords;
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
                    Vector3 pos = new Vector3(-width * .5f + .5f + x, -height * .5f + .5f + y, 0.0f);

                    Gizmos.color = canvas[x, y] == 0 ? Color.black : Color.yellow;
                    Gizmos.DrawCube(pos * scale, Vector3.one * scale);
                }
            }
        }
    }

}
