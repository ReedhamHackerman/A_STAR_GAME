using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFlow : MonoBehaviour
{
    public GridColumn[] gameBoard;
    public Vector2Int startAIPos ;
    public bool[,] Walls { get; private set; } = new bool[10, 10]; //false are open
    AStarAgent AI;
    public GameObject player;
    void Start()
    {
        FillWallArray();
        SpawnPieces();
    }

    
    void Update()
    {
        AI.CalculatePath();
    }
    void FillWallArray()
    {
        for(int i = 0;i<gameBoard.Length;i++)
        {
            for (int j = 0; j < gameBoard[i].squares.Length; j++)
            {
                if(!gameBoard[i].squares[j].activeSelf)
                {
                    Walls[i, j] = true;
                }
            }
        }
    }
    void SpawnPieces()
    {
      AI =   Instantiate<AStarAgent>(Resources.Load<AStarAgent>("Prefabs/AI"), new Vector3(startAIPos.x,startAIPos.y,0), Quaternion.identity);
      AI.Initialize(this);
    }
}
