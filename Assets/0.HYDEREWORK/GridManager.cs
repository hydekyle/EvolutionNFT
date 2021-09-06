using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Unity;

public class GridManager : MonoBehaviour {

    int modelScaleUnits = 3;

    public LayerMask bloqLayer;
    public BloqTable bloqList;
    public int filas;
    public int columnas;
    public int[,] matrixMap;
    public Node[,] matrixNode;

    private void Start()
    {
        BuildMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) Click();
        if (Input.GetKeyDown(KeyCode.F9)) Test();
        if (Input.GetKeyDown(KeyCode.F10)) FirebaseTest();
        if (Input.GetKeyDown(KeyCode.F11)) FirebaseGetPlayerData();
    }

    void BuildMap()
    {
        // Crear Matrix
        matrixNode = new Node[filas, columnas];
        for (var x = 0; x < filas; x++)
            for (var y = 0; y < columnas; y++)
                matrixNode[x, y] = new Node(Random.Range(0,10) < 8 ? true : false, new Vector3(x, y, 0));
        
        // Generar sprites
        for (var x = 0; x < filas; x++)
        {
            for (var y = 0; y < columnas; y++)
            {
                Node node = matrixNode[x, y];
                GameObject go;
                if (node.walkable)
                {
                    go = Instantiate(bloqList.suelo, new Vector3(node.worldPosition.x * modelScaleUnits, 0, node.worldPosition.y * modelScaleUnits), Quaternion.identity);
                }
                else
                {
                    if (Random.Range(0, 10) > 7)
                    {
                        go = Instantiate(bloqList.pared, new Vector3(node.worldPosition.x * modelScaleUnits, 0, node.worldPosition.y * modelScaleUnits), Quaternion.identity);
                    }
                    else
                    {
                        go = Instantiate(bloqList.pared2, new Vector3(node.worldPosition.x * modelScaleUnits, 0, node.worldPosition.y * modelScaleUnits), Quaternion.identity);
                    }
                }
                node.mySprite = go.transform.GetComponent<MeshRenderer>();
            }
        }
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(var i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost <= currentNode.fCost && openSet[i].hCost < currentNode.hCost) currentNode = openSet[i];
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        foreach (Node n in path)
        {
            print(n.worldPosition);
            foreach (Material m in n.mySprite.materials)
            {
                m.color = Color.blue;
            }
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.RoundToInt(Mathf.Abs(nodeA.worldPosition.x - nodeB.worldPosition.x));
        int dstY = Mathf.RoundToInt(Mathf.Abs(nodeA.worldPosition.y - nodeB.worldPosition.y));

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        int x = (int)node.worldPosition.x;
        int y = (int)node.worldPosition.y;

        if (x - 1 >= 0) neighbours.Add(matrixNode[x - 1, y]);
        if (x + 1 < columnas) neighbours.Add(matrixNode[x + 1, y]);

        if (y - 1 >= 0) neighbours.Add(matrixNode[x, y - 1]);
        if (y + 1 < filas) neighbours.Add(matrixNode[x, y + 1]);

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / modelScaleUnits);
        int y = Mathf.RoundToInt(worldPos.z / modelScaleUnits);

        try
        {
            return matrixNode[x, y];
        }
        catch
        {
            return null;
        }
        
    }

    byte by = 0;
    Vector3 n1;

    void Click()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Node node = NodeFromWorldPoint(hit.transform.position);
            Material[] materiales = hit.transform.GetComponent<MeshRenderer>().materials;
            foreach(Material m in materiales)
            {
                m.color = Color.red;
            }
            if (by == 0)
            {
                print("!");
                n1 = hit.transform.position;
                by++;
            }
            else
            {
                print("!!");
                FindPath(n1, hit.transform.position);
                by = 0;
            }

        }
    }

    void Test()
    {
        print(GetNeighbours(matrixNode[5, 5]).Count);
        print(GetNeighbours(matrixNode[0, 0]).Count);
    }


    void FirebaseGetPlayerData()
    {
        //FirebaseDatabase.DefaultInstance.RootReference.Child(SystemInfo.deviceUniqueIdentifier).GetValueAsync().ContinueWith(data =>
        //{
        //    jugadorActivo = JsonUtility.FromJson<Jugador>(data.Result.GetRawJsonValue());
        //    print("ok");
        //});
    }

    void FirebaseTest()
    {
        //Jugador jugador = new Jugador()
        //{
        //    nivel = 1,
        //    nombre = "Ayoze",
        //    gold = 1000,
        //    diamond = 0
        //};
        //FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task => {
        //    print("conectado");
        //    FirebaseDatabase.DefaultInstance.RootReference.Child(SystemInfo.deviceUniqueIdentifier).SetRawJsonValueAsync(JsonUtility.ToJson(jugador));

        //});
    }
	
}
