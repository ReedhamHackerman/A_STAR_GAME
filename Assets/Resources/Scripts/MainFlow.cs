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
    public Vector2 playerVectorPos;
   
    void Start()
    {


       playerVectorPos = new Vector2((int)player.transform.position.x, (int)player.transform.position.y);
        FillWallArray();
        SpawnPieces();
        AI.CalculatePath();

    }

    
    void Update()
    {
       
        playerVectorPos = new Vector2((int)player.transform.position.x, (int)player.transform.position.y);
        AI.Refresh();
        
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
