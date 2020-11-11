using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

// This class processes the most stuff
public class Dungeon : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject[] _piecePrefabs;

    [SerializeField] private uint _dungeonSize;

    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _enemyNode;


    #endregion

    #region Private Data

    private List<DungeonPiece> _builtPieces;
    private Queue<NodeConnector> _activeConnectors;
    private Random _random;
    private uint _count;
    private bool _complete;
    private List<NavMeshSurface> _surfaces;
    


    #endregion

    #region Functions
    private void Start()
    {
        _builtPieces = new List<DungeonPiece>();
        _activeConnectors = new Queue<NodeConnector>();
        _surfaces = new List< NavMeshSurface>();
        


        StartGeneration();

        _complete = false;

    }

    private void Update() 
    {
        if (!_complete)
        {
            if (_count < _dungeonSize)
            {
                StartCoroutine("ProcessSection");
                ++_count;
            }
            else
            {
                // When its done
                foreach (DungeonPiece piece in _builtPieces)
                {
                    piece.SetValidatorsState(false);
                }

                _player.transform.position = _builtPieces[0].Pivot.transform.position + new Vector3(0.0f, 2.46f, 0.0f);

                _complete = true;
                //Build NavMesh
                int index = 0;
                foreach(DungeonPiece piece in _builtPieces)
                {
                    _surfaces.Add(piece.gameObject.GetComponent<NavMeshSurface>());
                    _surfaces[index].BuildNavMesh();
                    
                    index++;
                }
                //Create enemies
                CreateEnemyNode();
                // Enable movement
                _player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = true;
                _player.GetComponent<Mage>().enabled = true;

            }
        }
    }
    private void CreateEnemyNode()
    {
        _enemyNode.transform.position= _builtPieces[18].Pivot.transform.position;
        _enemyNode.GetComponentInChildren<EnemyController>().enabled = true;    // Enable enemy controller
    }


    // Generates a level
    private void StartGeneration()
    {
        _random = new Random(DateTime.Now.Millisecond);

        // Place first room and pull DungeonPiece component
        _builtPieces.Add(Instantiate(_piecePrefabs[_random.Next(0, _piecePrefabs.Length)],transform).GetComponent<DungeonPiece>());

        // Add its connectors to queue
        foreach (NodeConnector node in _builtPieces[0].Nodes)
        {
            _activeConnectors.Enqueue(node);
        }
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

            dungeonPiece = Instantiate(picked,transform).GetComponent<DungeonPiece>();
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

            _builtPieces.Add(dungeonPiece);
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
