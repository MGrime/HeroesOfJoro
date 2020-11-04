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
        StartCoroutine("ProcessSection");
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

    private IEnumerator ProcessSection()
    {
        // Take a connector
        NodeConnector connector = _activeConnectors.Dequeue();

        //Debug.Log("Root: " + connector.name);

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

        // 0. Set the validators to trigger mode
        dungeonPiece.SetTriggerState(true);

        // Pick a random connector to use as the linker
        NodeConnector linkConnector = dungeonPiece.Nodes[_random.Next(0, dungeonPiece.Nodes.Length)];

        //Debug.Log("Link: " + linkConnector.name);

        // Rotate so the two connectors are pointing into each other (work out how to rotate them to be the same then do the inverse rotation)
        float rotateDeterminant = Vector3.Dot(linkConnector.transform.forward, connector.transform.forward);
        //Debug.Log("Rotate: " + rotateDeterminant);

        // If it is equal to 1 they are facing the same way so rotate 180
        // If it is equal to 0 then it is at a 90 degree angle. Rotate by 90. Then check again. If 1 we are done, if -1 rotate by 180
        // If it is equal to -1 then they are already at the correct angles
        if (Mathf.Approximately(rotateDeterminant,1.0f))
        {
           // Debug.Log("Rotated 180!");

            dungeonPiece.Pivot.transform.Rotate(0.0f,180.0f,0.0f,Space.World);
        }
        else if (!Mathf.Approximately(rotateDeterminant, -1.0f))
        {
            dungeonPiece.Pivot.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);

            rotateDeterminant = Vector3.Dot(linkConnector.transform.forward, connector.transform.forward);

            // Just need to check for 1. It is either 1 or -1
            if (Mathf.Approximately(rotateDeterminant, 1.0f))
            {
                dungeonPiece.Pivot.transform.Rotate(0.0f, 180.0f, 0.0f, Space.World);
                //Debug.Log("Rotated 90 then an extra 180!");
            }
        }


        // Now need to move into the correct position

        // 1. Take position of linking connector and get position 1 forward in forward
        Vector3 newConnectorPos = connector.transform.position + (connector.transform.forward);   // This is the location the new connector will sit

        // 2. Place root at connector post
        dungeonPiece.Pivot.transform.position = newConnectorPos;

        // 3. Move by the distance between the conncctors
        Vector3 movementVector = linkConnector.transform.position - newConnectorPos;
        dungeonPiece.Pivot.transform.position -= movementVector;

        // Wait until next physics update to ensure trigger works
        yield return new WaitForFixedUpdate();

        // 4. At this point on trigger enter will collide and invalid the piece if needed
        // If this component isnt found the prefab is wrong.
        if (dungeonPiece.Pivot.GetComponent<DungeonValidator>().Valid)
        {
            // Add new connectors 
            foreach (NodeConnector node in dungeonPiece.Nodes)
            {
                if (node == linkConnector)
                {
                    continue;
                }
                _activeConnectors.Enqueue(node);
            }

            // Change back trigger state
            dungeonPiece.SetTriggerState(false);
        }
        else
        {
            // Delete it
            DestroyImmediate(dungeonPiece.gameObject);
        }

        yield return null;
    }

    #endregion
}
