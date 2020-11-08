using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAgent : MonoBehaviour
{
    MainFlow mainFlow;
    Node currentNode;
    Dictionary<Vector2Int, Node> openNodes = new Dictionary<Vector2Int, Node>();
    Dictionary<Vector2Int, Node> closeNodes = new Dictionary<Vector2Int, Node>();
    public void Initialize(MainFlow mainFlow)
    {
        this.mainFlow = mainFlow;
    }

    public void CalculatePath()
    {
        currentNode = new Node(mainFlow.startAIPos, Heuristic(mainFlow.startAIPos), 0, null);
    }


    public float Heuristic(Vector2Int nodePos)
    {
        return Vector2.Distance(nodePos, mainFlow.player.gameObject.transform.position);
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
