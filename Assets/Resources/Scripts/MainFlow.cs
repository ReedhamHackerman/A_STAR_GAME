using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFlow : MonoBehaviour
{
    public GridColumn[] gameBoard;
    public Vector2Int startAIPos ;
    public bool[,] Walls { get; private set; } = new bool[10, 10]; //false are open
    AStarAgent AI;
    Vector2 RandomValidPositionOnBoard;
    public GameObject player;
    public Vector2 playerVectorPos;
     public  List<Vector2> availablePos;
    // public new Collider2D collider2D;
    public GameObject collider;
    void Start()
    {

        collider = Resources.Load<GameObject>("Prefabs/collider");
        //collider2D = new Collider2D();
        FillWallArray();
        SpawnPieces();
        
      
    }

    
    void LateUpdate()
    {
        GetThePlayerPos();
        Debug.Log(GetThePlayerPos());
    }


    public Vector2 GetThePlayerPos()
    {
        playerVectorPos = new Vector2((int)(player.transform.position.x+0.2f), (int)(player.transform.position.y+0.2f));
        return playerVectorPos;
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
                    // gameBoard[i].squares[j].AddComponent<BoxCollider2D>();
                    Instantiate<GameObject>(collider, gameBoard[i].squares[j].transform.position, Quaternion.identity);
                   
                }
                else
                {
                    availablePos.Add(new Vector2(i, j));
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
