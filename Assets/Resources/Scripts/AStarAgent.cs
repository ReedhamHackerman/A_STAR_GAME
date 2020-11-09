using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    const float closeEnough = 0.01f;
    public float movementSpeed;
    MainFlow mainFlow;
    Node currentNode;
    Node nextNodeToMoveTo = null;
    int nextNodeIndex = 0;
    Dictionary<Vector2Int, Node> openNodes = new Dictionary<Vector2Int, Node>();
    Dictionary<Vector2Int, Node> closeNodes = new Dictionary<Vector2Int, Node>();
    List<Node> solutionPath = new List<Node>();

    public void Initialize(MainFlow mainFlow)
    {
        this.mainFlow = mainFlow;
    }

    public void  Refresh()
    {
        MoveAlongPath();
    }


    public void CalculatePath()
    {
        currentNode = new Node(mainFlow.startAIPos, Heuristic(mainFlow.startAIPos), 0, null);
        while((Vector2)mainFlow.playerVectorPos != currentNode.NodePos)
        {
            closeNodes.Add(currentNode.NodePos, currentNode);
            openNodes.Remove(currentNode.NodePos);
            List<Vector2Int> validNeighbourSquares = FindValidNeighbourSquares();
            foreach(Vector2Int neighbourPos in validNeighbourSquares)
            {
                if(!openNodes.ContainsKey(neighbourPos))
                {
                    Node newNode = new Node(neighbourPos, Heuristic(neighbourPos), MovementCost(neighbourPos), currentNode);
                    openNodes.Add(neighbourPos, newNode);
                }
                else if(MovementCost(neighbourPos)<openNodes[neighbourPos].G)
                  openNodes[neighbourPos].ChangeGAndFAndParent(MovementCost(neighbourPos), currentNode);   
            }
            currentNode = ReturnCurrentNode();
         }
        FillSolutionList();
    }
    public List<Vector2Int> FindValidNeighbourSquares()
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        for (int  x = currentNode.NodePos.x-1;  x < currentNode.NodePos.x + 2;  x++)
        {
            if (x < 0 || x > 9)
                continue;
            for (int y = currentNode.NodePos.y; y < currentNode.NodePos.y+2 ; y++)
            {
                if (y < 0 || y > 9)
                    continue;
                if (closeNodes.ContainsKey(new Vector2Int(x, y)))
                    continue;
                if (mainFlow.Walls[x, y])
                    continue;
                neighbours.Add(new Vector2Int(x, y));

            }

        }



        return neighbours;

    }

    public float Heuristic(Vector2Int nodePos)
    {
        return Vector2.Distance(nodePos, mainFlow.playerVectorPos);
    }
    public float MovementCost(Vector2Int nodePos)
    {
        return currentNode.G + ((currentNode.NodePos.x != nodePos.x && currentNode.NodePos.y != nodePos.y) ? Mathf.Sqrt(2) : 1);
    }

    public Node ReturnCurrentNode()
    {
        Node nodeWithLowestF = null;
        foreach (KeyValuePair<Vector2Int,Node> kvp in openNodes)
        {
            if (nodeWithLowestF == null)
                nodeWithLowestF = kvp.Value;
            else
                if (kvp.Value.F < nodeWithLowestF.F)
                nodeWithLowestF = kvp.Value;
        }
        return nodeWithLowestF;
    }
    void FillSolutionList()
    {
        Node nextNodeToAdd = currentNode;
        while(nextNodeToAdd.Parent != null)
        {
            solutionPath.Add(nextNodeToAdd);
            nextNodeToAdd = nextNodeToAdd.Parent;
        }
        solutionPath.Reverse();
    }
    void MoveAlongPath()
    {

        if(Vector2.Distance(transform.position,mainFlow.playerVectorPos)<= closeEnough)
            transform.position = new Vector3(nextNodeToMoveTo.NodePos.x, nextNodeToMoveTo.NodePos.y, 0);
        else
        {
            if (nextNodeToMoveTo == null)
                nextNodeToMoveTo = solutionPath[0];
            if (Vector2.Distance(transform.position, nextNodeToMoveTo.NodePos) < closeEnough)
            {

                transform.position = new Vector3(nextNodeToMoveTo.NodePos.x, nextNodeToMoveTo.NodePos.y, 0);
                nextNodeIndex++;
                nextNodeToMoveTo = solutionPath[nextNodeIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextNodeToMoveTo.NodePos.x, nextNodeToMoveTo.NodePos.y, 0), movementSpeed * Time.deltaTime);
        }
       
    }
}
public class Node
{
    public Vector2Int NodePos { get; private set; }
    public float H { get; private set; }
    public float G { get; private set; }
    public float F { get; private set; }
    public Node Parent { get; private set; }


    public Node(Vector2Int nodePos,float h,float g,Node parent)
    {
        NodePos = nodePos;
        G = g;
        H = h;
        F = g + h;
        Parent = parent;
    }


    public void ChangeGAndFAndParent(float newG,Node newParent)
    {
        G = newG;
        F = G + H;
        Parent = newParent;
    }

}
