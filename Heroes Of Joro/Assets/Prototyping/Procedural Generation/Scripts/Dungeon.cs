using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = System.Random;

// This class processes the most stuff
public class Dungeon : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject[] _piecePrefabs;
    #endregion

    #region Private Data

    private List<DungeonPiece> _builtPieces;
    private Queue<NodeConnector> _activeConnectors;
    private Random _random;

    #endregion

    #region Functions
    private void Start()
    {
        _builtPieces = new List<DungeonPiece>();
        _activeConnectors = new Queue<NodeConnector>();

        StartGeneration();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessSection();
        }
    }



    // Generates a level
    private void StartGeneration()
    {
        _random = new Random(DateTime.Now.Millisecond);

        // Place first room and pull DungeonPiece component
        _builtPieces.Add(Instantiate(_piecePrefabs[_random.Next(0, _piecePrefabs.Length)].GetComponent<DungeonPiece>())) ;

        // Add its connectors to queue
        foreach (NodeConnector node in _builtPieces[0].Nodes)
        {
            _activeConnectors.Enqueue(node);
        }
        #region temp close
        //DungeonPiece dungeonPiece = null;
        //// Loop till connectors empty
        //while (_activeConnectors.Count != 0)
        //{
        //    // Take a connector
        //    NodeConnector connector = _activeConnectors.Dequeue();

        //    // If room connector
        //    if (connector.ConnectionType == NodeConnector.ConnectorType.Room)
        //    {
        //        dungeonPiece = Instantiate(_roomPrefabs[_random.Next(0, _roomPrefabs.Length)].GetComponent<DungeonPiece>());
        //    }
        //    // This could be an else but may add more in future so leaving as an else if
        //    else if (connector.ConnectionType == NodeConnector.ConnectorType.Hallway)
        //    {
        //        // Spawn new
        //        dungeonPiece = Instantiate(_hallwayPrefabs[_random.Next(0, _hallwayPrefabs.Length)].GetComponent<DungeonPiece>());
        //    }

        //    int positionOfSelectedNode = 0;
        //    // Check to supress errors in VS
        //    if (dungeonPiece != null)
        //    {
        //        // Select connector 
        //        NodeConnector linkingConnector = null;
        //        // check connectors
        //        foreach (NodeConnector node in dungeonPiece.Nodes)
        //        {
        //            // Find first connector of matching type
        //            if (node.ConnectionType == _builtPieces[_builtPieces.Count - 1].MyConnectorType)
        //            {
        //                linkingConnector = node;
        //                break;
        //            }

        //            ++positionOfSelectedNode;
        //        }

        //        // Rotate so orientation is correct
        //        // 1. Get Z of linking connector
        //        Vector3 localZLink = linkingConnector.transform.forward;

        //        // 2. Get Z of root connector
        //        Vector3 localZRoot = connector.transform.forward;

        //        // 3. Work out rotation needed to align
        //        double angleRootLink = (180.0 / Math.PI) * Math.Acos(Vector3.Dot(localZLink, localZRoot));

        //        // 4. Rotate dungeon piece using pivot piece
        //        dungeonPiece.Pivot.transform.Rotate(0.0f, (float)-angleRootLink, 0.0f);

        //        // Now we position
        //        // 1. Get Offset from linkingConnector to root object position
        //        Vector3 linkConnectorOffset = dungeonPiece.transform.position - linkingConnector.transform.position;

        //        // 2. Move root object to position of connector
        //        dungeonPiece.transform.position = connector.transform.position;

        //        // 3. Move by offset
        //        dungeonPiece.transform.position += linkConnectorOffset;
        //    }

        //    for (int i = 0; i < dungeonPiece.Nodes.Length; ++i)
        //    {
        //        if (positionOfSelectedNode != i)
        //        {
        //            _activeConnectors.Enqueue(dungeonPiece.Nodes[i]);
        //        }
        //    }


        //}
#endregion
    }

    private void ProcessSection()
    {
        // Take a connector
        NodeConnector connector = _activeConnectors.Dequeue();

        Debug.Log("Root: " + connector.name);

        DungeonPiece dungeonPiece = null;
        // Dont Connect the same pieces together
        while (dungeonPiece == null)
        {
            GameObject picked = _piecePrefabs[_random.Next(0, _piecePrefabs.Length)];
            if (picked.GetComponent<DungeonPiece>().name.Contains(connector.transform.parent.parent.parent.name)) // This item is always nested 3 times
            {
                break;
            }

            dungeonPiece = Instantiate(picked).GetComponent<DungeonPiece>();
        }

        // Now we need to process the new piece

        // Move entire room into position of the activeConnector

        // Pick a random connector to use as the linker
        NodeConnector linkConnector = dungeonPiece.Nodes[_random.Next(0, dungeonPiece.Nodes.Length)];

        Debug.Log("Link: " + linkConnector.name);

        // Rotate so the two connectors are pointing into each other (work out how to rotate them to be the same then do the inverse rotation)
        float rotateDeterminant = Vector3.Dot(linkConnector.transform.forward, connector.transform.forward);
        Debug.Log("Rotate: " + rotateDeterminant);

        // This means they are point the same
        if (Mathf.Approximately(1.0f,rotateDeterminant))
        {
            dungeonPiece.Pivot.transform.rotation = Quaternion.Euler(
                new Vector3(
                    dungeonPiece.Pivot.transform.rotation.eulerAngles.x,
                    180.0f,
                    dungeonPiece.Pivot.transform.rotation.eulerAngles.z
                ));
            Debug.Log("Rotated 180");
        }
        else
        {
            float rotateAmount = 360.0f - (float)(180.0f / Math.PI * Math.Acos(rotateDeterminant));

            dungeonPiece.Pivot.transform.rotation = Quaternion.Euler(
                new Vector3(
                    dungeonPiece.Pivot.transform.rotation.eulerAngles.x,
                    rotateAmount,
                    dungeonPiece.Pivot.transform.rotation.eulerAngles.z
                ));

            Debug.Log("Rotated inverse!");
        }

        

        // Now need to move into the correct position

        // 1. Take position of linking connector and get position 1 forward in forward
        Vector3 newConnectorPos = connector.transform.position /*+ connector.transform.forward*/;   // This is the location the new connector will sit

        // 2. Place root at connector post
        dungeonPiece.Pivot.transform.position = newConnectorPos;

        // 3. Move by the distance between the conncctors
        Vector3 movementVector = linkConnector.transform.position - newConnectorPos;
        dungeonPiece.Pivot.transform.position -= movementVector;

        // Add new connectors 
        foreach (NodeConnector node in dungeonPiece.Nodes)
        {
            if (node == linkConnector)
            {
                continue;
            }
            _activeConnectors.Enqueue(node);
        }

        //TODO : IMPLEMENT COLLISION CHECKS


    }

    #endregion
}
