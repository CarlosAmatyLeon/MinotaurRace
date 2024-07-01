using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    [SerializeField] GameObject floor;
    [SerializeField] GameObject pilar;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject mazeCenter;

    private float wallDelay;
    private float mazeDelay;
    private float wallSpeed;
    private bool gameOver;
    private int xSize;
    private int zSize;
    private Vector3 position = Vector3.zero;
    private float gap = 5.0f;
    
    class Cell
    {
        public int x;
        public int z;
        public bool top = true;
        public bool right = true;
    }

    List<Cell> Cells = new List<Cell> { };

    public void StartMaze()
    {
        mazeCenter.transform.position = new Vector3((xSize * 5.0f) / 2, 0.0f, (zSize * 5.0f) / 2); ;
        PopulateCells();
        CreateStructure();
        StartCoroutine(CreateNewMaze());
    }

    IEnumerator CreateNewMaze()
    {
        while (!gameOver)
        {   
            ResetCells();
            GenerateMaze();
            StartCoroutine(UpdateWalls());

            yield return new WaitForSeconds(mazeDelay);
        }
    }
    void PopulateCells()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                Cells.Add(new Cell { x = i, z = j });
            }
        }
    }

    void ResetCells()
    {
        foreach (Cell cell in Cells)
        {
            cell.top = true;
            cell.right = true;
        }
    }

    void CreateStructure()
    {
        // Creation of basic structure.
        CreateGridOfObject(xSize + 1, zSize + 1, 0, 0, floor, Quaternion.identity, "floor");
        CreateGridOfObject(xSize + 1, zSize + 1, 0, 0, pilar, Quaternion.identity, "pilar");
        CreateGridOfObject(xSize + 1, zSize, 0, 2.5f, wall, Quaternion.identity, "V");
        CreateGridOfObject(xSize, zSize + 1, 2.5f, 0, wall, Quaternion.Euler(new Vector3(0, 90f, 0)), "H");
    }

    void CreateGridOfObject(int sizeX, int sizeZ, float xOffset, float zOffset, GameObject obj, Quaternion rot, string namePrefix)
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                Vector3 newPosition = new Vector3(i * gap + xOffset, 0f, j * gap + zOffset);
                position = newPosition;
                GameObject newObject = Instantiate(obj, position, rot, transform);
                newObject.name = namePrefix + i + "-" + j;
                Renderer newObjRenderer = newObject.GetComponent<Renderer>();
            }
        }
    }

    void GenerateMaze()
    {
        List<int[]> activeCells = new List<int[]>();
        List<int[]> visitedCells = new List<int[]>();

        int[] startCell = new int[] { UnityEngine.Random.Range(0, xSize - 1), UnityEngine.Random.Range(0, zSize - 1) };
        
        activeCells.Add(startCell);
        visitedCells.Add(startCell);

        while (activeCells.Count > 0)
        {
            int[] currentCell = activeCells[UnityEngine.Random.Range(0, activeCells.Count-1)];
            
            (bool, string, int[]) neighbour = CheckNeighboursAndChoose(currentCell, visitedCells);
            bool validNeighbour = neighbour.Item1;
            string direction = neighbour.Item2;
            int[] newCell = neighbour.Item3;

            if (validNeighbour)
            {
                activeCells.Add(newCell);
                visitedCells.Add(newCell);
                UpdateWall(currentCell, newCell, direction);
            }
            else
            {
                for (int i = 0; i < activeCells.Count; i++)
                {
                    if (activeCells[i][0] == currentCell[0] && activeCells[i][1] == currentCell[1])
                    {
                        activeCells.RemoveAt(i);
                    }
                }
            }
        }
    }

    (bool, string, int[]) CheckNeighboursAndChoose(int[] cell, List<int[]> visitedCells)
    {
        bool validNeighbour = false;
        List<(int[], string)> availableNeighbours = new List<(int[], string)> { };
        string direction = "none";
        int[] newCell = new int[] { -1,-1 };

        //top
        if (cell[1] != zSize - 1)
        {
            int[] topNeighbour = new int[] { cell[0], cell[1] + 1 };
            if (!visitedCells.Any(array => array.SequenceEqual(topNeighbour)))
            {
                availableNeighbours.Add((topNeighbour, "top"));
                validNeighbour = true;
            }
        }

        //right
        if (cell[0] != zSize - 1)
        {
            int[] rightNeighbour = new int[] { cell[0] + 1, cell[1] };
            if (!visitedCells.Any(array => array.SequenceEqual(rightNeighbour)))
            {
                availableNeighbours.Add((rightNeighbour, "right"));
                validNeighbour = true;
            }
        }

        //bottom
        if (cell[1] != 0)
        {
            int[] bottomNeighbour = new int[] { cell[0], cell[1] - 1 };
            if (!visitedCells.Any(array => array.SequenceEqual(bottomNeighbour)))
            {
                availableNeighbours.Add((bottomNeighbour, "bottom"));
                validNeighbour = true;
            }
        }

        //left
        if (cell[0] != 0)
        {
            int[] leftNeighbour = new int[] { cell[0] - 1, cell[1] };
            if (!visitedCells.Any(array => array.SequenceEqual(leftNeighbour)))
            {
                availableNeighbours.Add((leftNeighbour, "left"));
                validNeighbour = true;
            }
        }

        if (availableNeighbours.Count > 0)
        {
            int chosenNeighbour = UnityEngine.Random.Range(0, availableNeighbours.Count - 1);
            direction = availableNeighbours[chosenNeighbour].Item2;
            newCell = availableNeighbours[chosenNeighbour].Item1;
        }
        return (validNeighbour, direction, newCell);
    }

    void UpdateWall(int[] currentCell, int[] newCell, string direction)
    {
        int index;

        if (direction == "top" || direction == "right")
        {
            index = zSize * currentCell[0] + currentCell[1];
            if (direction == "top") Cells[index].top = false;
            if (direction == "right") Cells[index].right = false;
        }
        if (direction == "bottom" || direction == "left")
        {
            index = zSize * newCell[0] + newCell[1];
            if (direction == "bottom") Cells[index].top = false;
            if (direction == "left") Cells[index].right = false;
        }
    }

    IEnumerator UpdateWalls()
    {
        foreach (Cell cell in Cells)
        {
            string rightWall = $"V{cell.x+1}-{cell.z}";
            string topWall = $"H{cell.x}-{cell.z + 1}";

            Transform rWall = transform.Find(rightWall);
            if (rWall != null)
            {
                rWall.gameObject.GetComponent<WallMovement>().speed = wallSpeed;
                rWall.gameObject.GetComponent<WallMovement>().wallUp = cell.right;
            }

            Transform tWall = transform.Find(topWall);
            if (tWall != null)
            {
                tWall.gameObject.GetComponent<WallMovement>().speed = wallSpeed;
                tWall.gameObject.GetComponent<WallMovement>().wallUp = cell.top;
            }

            yield return new WaitForSeconds(wallDelay);
        }
    }

    public void SetMazeDelay(float delay)
    {
        mazeDelay = delay;
    }
    
    public void SetWallDelay(float delay)
    {
        wallDelay = delay;
    }

    public void SetWallSpeed(float speed)
    {
        wallSpeed = speed;
    }
    public void SetGameOver(bool gameStatus)
    {
        gameOver = gameStatus;
    }

    public void SetMazeSize(int size)
    {
        xSize = size;
        zSize = size;
    }
}
