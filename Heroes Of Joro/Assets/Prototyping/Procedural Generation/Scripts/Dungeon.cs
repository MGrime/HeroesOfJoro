﻿using System;
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

        DungeonPiece dungeonPiece = null;
        dungeonPiece = Instantiate(_piecePrefabs[_random.Next(0, _piecePrefabs.Length)].GetComponent<DungeonPiece>());

        // Check to supress errors in VS
        if (dungeonPiece != null)
        {
            int positionOfSelectedNode = 0;

            // Select connector 
            NodeConnector linkingConnector = null;

            foreach (NodeConnector node in dungeonPiece.Nodes)
            {
                if (node.transform.forward != connector.transform.forward)
                {
                    linkingConnector = node;
                    break;
                }

                ++positionOfSelectedNode;
            }

            // Rotate so orientation is correct
            // 1. Get Z of linking connector
            Vector3 localZLink = linkingConnector.transform.forward;
            Debug.Log(linkingConnector.transform.forward);

            // 2. Get Z of root connector
            Vector3 localZRoot = connector.transform.forward;
            Debug.Log(connector.transform.forward);


            // 3. Work out rotation needed to align
            double angleRootLink = (180.0 / Math.PI) * Math.Acos(Vector3.Dot(localZLink, localZRoot));

            // Stops low float errors
            if (angleRootLink > 1.0f)
            {
                // 4. Rotate dungeon piece using pivot piece
                dungeonPiece.Pivot.transform.Rotate(0.0f, (float)-angleRootLink, 0.0f);
            }

            // Now we position
            // 1. Get Offset from linkingConnector to root object position
            Vector3 linkConnectorOffset;
            if (angleRootLink >= 1.0f)
            {
                linkConnectorOffset = dungeonPiece.transform.position - linkingConnector.transform.position;
            }
            else
            {
                linkConnectorOffset = linkingConnector.transform.position - dungeonPiece.transform.position;
            }

            // 2. Move root object to position of connector
            dungeonPiece.transform.position = connector.transform.position;

            // 3. Move by offset
            dungeonPiece.transform.position += linkConnectorOffset;
            dungeonPiece.transform.position -= linkingConnector.transform.forward;  // Move back one

            for (int i = 0; i < dungeonPiece.Nodes.Length; ++i)
            {
                if (i != positionOfSelectedNode)
                {
                    _activeConnectors.Enqueue(dungeonPiece.Nodes[i]);
                }
            }
        }

    }

    #endregion
}
