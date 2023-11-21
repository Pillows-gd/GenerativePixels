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
    [SerializeField] private float randomLifeDensity = .5f;
    [Range(0, 1)]
    [SerializeField] private float scale = 0.08f;
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 72;
    [SerializeField] private float updateRate = 10f;
    private int[,] canvas;
    private int[,] currentState;
    private int pixelsPerUnit = 108; // Not 100! This makes the pointer more precise. I dunno why D:
    private bool isUpdating = false;

    private void Start()
    {
        InitializeCanvas();
    }

    private void Update()
    {
        // LEFT CLICK -> DRAW
        if (Input.GetMouseButtonDown(0))
        {
            int[] pointedCoords = getPointedCoords();

            try
            {
                canvas[pointedCoords[0], pointedCoords[1]] = 1;
            }
            catch (Exception) { }
        }

        // RIGHT CLICK -> ERASE
        if (Input.GetMouseButtonDown(1))
        {
            int[] pointedCoords = getPointedCoords();

            try
            {
                canvas[pointedCoords[0], pointedCoords[1]] = 0;
            }
            catch (Exception) { print("Pointing out of the canvas!"); }
        }

        // S KEY -> STOP UPDATING
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (isUpdating)
            {
                isUpdating = false;
                StopCoroutine(UpdateState());
            }
        }

        // SPACEBAR -> START UPDATING / RESET
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isUpdating)
            {
                isUpdating = true;
                StartCoroutine(UpdateState());
            }
            else
            {
                isUpdating = false;
                StopCoroutine(UpdateState());
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        canvas[x, y] = 0;
            }
        }

        // R KEY -> RANDOMIZE
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isUpdating)
                RandomizeCanvas();
        }
    }

    private void RandomizeCanvas()
    {
        System.Random randGenerator = new System.Random(Time.time.ToString().GetHashCode());
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                canvas[x, y] = (float)randGenerator.Next(0, 100) / 100 < randomLifeDensity ? 1 : 0;
    }
    IEnumerator UpdateState()
    {
        while (isUpdating)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    currentState[x, y] = canvas[x, y];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    try
                    {
                        // RULES:

                        // Random Map Generator:
                        canvas[x, y] = activeMooreNeighbors(x, y) > 4 ? 1 : canvas[x, y];
                        canvas[x, y] = activeMooreNeighbors(x, y) < 4 ? 0 : canvas[x, y];

                        // Brian's Brain:
                        //if (currentState[x, y] == 2)
                        //    canvas[x, y] = 1;
                        //else if (currentState[x, y] == 1)
                        //    canvas[x, y] = 0;
                        //else if (currentState[x, y] == 0 && activeMooreNeighbors(x, y) == 2)
                        //    canvas[x, y] = 2;

                        // Game of Life:
                        //if (currentState[x, y] == 1 && activeMooreNeighbors(x, y) < 2)
                        //    canvas[x, y] = 0;
                        //if (currentState[x, y] == 1 && activeMooreNeighbors(x, y) > 3)
                        //    canvas[x, y] = 0;
                        //if (currentState[x, y] == 0 && activeMooreNeighbors(x, y) == 3)
                        //    canvas[x, y] = 1;

                        // My Experiment:
                        //if (activeMooreNeighbors(x, y) < 2)
                        //    canvas[x, y] = 0;
                        //if (activeMooreNeighbors(x, y) > 3)
                        //    canvas[x, y] = 1;


                    }
                    catch (Exception) { continue; }
            yield return new WaitForSeconds(1 / updateRate);
        }
    }

    int activeMooreNeighbors(int x, int y, int ray = 1)
    {
        int count = 0;

        for (int xOff = -ray; xOff <= ray; xOff++)
            for (int yOff = -ray; yOff <= ray; yOff++)
                if (xOff != 0 || yOff != 0)
                    try
                    {
                        if (currentState[x + xOff, y + yOff] > 0)
                            count++;
                    }
                    catch (Exception) { continue; }

        return count;
    }

    int activeVonNeumannNeighbors(int x, int y)
    {
        int count = 0;

        try
        {
            if (currentState[x - 1, y] > 0)
                count++;
            if (currentState[x, y + 1] > 0)
                count++;
            if (currentState[x + 1, y] > 0)
                count++;
            if (currentState[x, y - 1] > 0)
                count++;
        }
        catch (Exception) { }

        return count;
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
        currentState = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                canvas[x, y] = 0;
    }

    private void OnDrawGizmos()
    {
        if (canvas != null)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vector3 pos = new Vector3(-width * .5f + .5f + x, -height * .5f + .5f + y, 0.0f);

                    if (canvas[x, y] == 0)
                        Gizmos.color = Color.black;
                    else if (canvas[x, y] == 1)
                        Gizmos.color = Color.white;

                    Gizmos.DrawCube(pos * scale, Vector3.one * scale);
                }
    }

}