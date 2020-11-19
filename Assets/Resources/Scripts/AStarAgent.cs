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
    AIstate aiState = AIstate.Wander;
    Vector2 currentWanderDestination;
    
   // public SpriteRenderer VisionRadius;
    float vision;
    public void Initialize(MainFlow mainFlow)
    {
        this.mainFlow = mainFlow;
    }


    private void Start()
    {
        WanderEnter();
        vision = 1.5f;
        //SelectNewWanderDestination();
        //CalculatePath(currentWanderDestination);
        //VisionRadius.transform.localScale = new Vector2(VisionRadius.transform.localScale.x*2, VisionRadius.transform.localScale.y * 2)
        // vision = VisionRadius.transform.localScale.x;
    }



    public void CalculatePath(Vector2 target)
    {
       // Debug.Log("Calculating for target " + target);

        currentNode = new Node(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)), Heuristic(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y))), 0, null);
       
        while(target != currentNode.NodePos)
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
            if (openNodes.Count == 0)
            {
               // Debug.Log("cancelling algorithm because no open nodes");
                break;
            }
                
            currentNode = ReturnCurrentNode();
         }
        if (target == currentNode.NodePos)
        {
            FillSolutionList();
           // Debug.Log("solution found of length: " + solutionPath.Count);
        }
        else
            Debug.Log("No available solution was found");
        
       
    }
    public void Update()
    {
        switch (aiState)
        {
            case AIstate.Wander:
                WanderState();
                break;
            case AIstate.Chase:
                ChaseState();
                break;
            case AIstate.Investigate:
                Investigating();
                break;
        }
        MoveAlongPath(currentWanderDestination);
    }

    void WanderState()
    {
        if (Vector2.Distance(transform.position, mainFlow.player.transform.position) < vision)
        {
            Transition(AIstate.Chase);
        }
        if (Vector2.Distance(transform.position,currentWanderDestination)<=closeEnough)
        {
           // Debug.Log("wander state detects destination reached");
            Transition(AIstate.Wander);
        }
       
    }
    void Transition(AIstate newState)
    {
       // Debug.Log("Transition to state: " + newState.ToString());
        switch (newState)
        {
            case AIstate.Wander:
                WanderEnter();
                aiState = AIstate.Wander;
                break;
            case AIstate.Chase:
                ChaseEnter();
                aiState = AIstate.Chase;
                break;
            case AIstate.Investigate:
                InvestigateEnter();
                aiState = AIstate.Investigate;
                break;
        }
    }
    void WanderEnter()
    {
       // Debug.Log("Wander enter");
        ClearListOpenAndCloseAndSolutionPath();
        SelectNewWanderDestination();
        CalculatePath(currentWanderDestination);
        
            
    }
    void ChaseEnter()
    {
        Debug.Log("Chase is called");
      //  currentNode.NodePos =  new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        ClearListOpenAndCloseAndSolutionPath();
        currentWanderDestination = mainFlow.GetThePlayerPos();
        Debug.Log(currentWanderDestination);
        CalculatePath(currentWanderDestination);
       
    }


    void InvestigateEnter()
    {

    }
    void SelectNewWanderDestination()
    {
        int random = Random.Range(0, mainFlow.availablePos.Count);
    //    Debug.Log(random);
      
        currentWanderDestination = mainFlow.availablePos[random];
        //Debug.Log("Length:" + mainFlow.availablePos.Count);
        //Debug.Log(currentWanderDestination.x);
        //Debug.Log(currentWanderDestination.y);
      //  Debug.Log("new wander dest: " + currentWanderDestination);
    }

    void ChaseState()
    {
        if(Vector2.Distance(transform.position,mainFlow.GetThePlayerPos())>vision)
        {
            Transition(AIstate.Wander);
        }
    }

   void  Investigating()
    {

    }

    public void ClearListOpenAndCloseAndSolutionPath()
    {
      //  Debug.Log("Clear Called");
        openNodes.Clear();
        closeNodes.Clear();
        solutionPath.Clear();
        nextNodeToMoveTo = null;
        nextNodeIndex = 0;
       
    }
    public List<Vector2Int> FindValidNeighbourSquares()
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        for (int  x = currentNode.NodePos.x-1;  x < currentNode.NodePos.x + 2;  x++)
        {
            if (x < 0 || x > 9)
                continue;
            for (int y = currentNode.NodePos.y - 1; y < currentNode.NodePos.y+2 ; y++)
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
    void MoveAlongPath(Vector2 target)
    {

        if (Vector2.Distance(transform.position,target) <= closeEnough)
            transform.position = new Vector3(target.x, target.y, 0);
        else
        {
            if (nextNodeToMoveTo == null && solutionPath.Count > 0)
                nextNodeToMoveTo = solutionPath[0];
            if (Vector2.Distance(transform.position, nextNodeToMoveTo.NodePos) < closeEnough)
            {
    
                transform.position = new Vector3(nextNodeToMoveTo.NodePos.x, nextNodeToMoveTo.NodePos.y, 0);
                nextNodeIndex++;
                //if(nextNodeIndex>=solutionPath.Count)
                //{
                //    Debug.Log("Next node index:" + nextNodeIndex);
                //    Debug.Log("Solution path count :" + solutionPath.Count);
                //}
                nextNodeToMoveTo = solutionPath[nextNodeIndex];
               
            }
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextNodeToMoveTo.NodePos.x, nextNodeToMoveTo.NodePos.y, 0), movementSpeed * Time.deltaTime);
        }
    
    }
}

public enum AIstate
{
    Wander, Chase, Investigate
}
public class Node
{
    public Vector2Int NodePos { get;  set; }
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
