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
    [SerializeField] private GameObject _startingPiece;
    [SerializeField] private GameObject _doorPlug;

    [SerializeField] private uint _dungeonSize;

    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private GameManager _gameManager;

    [SerializeField] private PickupBase[] _pickupPrefabs;

    #endregion

    #region Private Data

    private List<DungeonPiece> _builtPieces;
    private Queue<NodeConnector> _activeConnectors;
    private Random _random;
    private uint _count;
    private bool _complete;
    private List<NavMeshSurface> _surfaces;
    private List<GameObject> _enemyClones;


    #endregion

    #region Functions
    private void Start()
    {
        _builtPieces = new List<DungeonPiece>();
        _activeConnectors = new Queue<NodeConnector>();
        _surfaces = new List<NavMeshSurface>();
        _enemyClones = new List<GameObject>();

        StartGeneration();

        _complete = false;

    }

    private void Update() 
    {
        if (!_complete)
        {
            if (_count < _dungeonSize)
            {
                //if (Input.GetKeyDown(KeyCode.Space))
                //{
                    StartCoroutine("ProcessSection");
                    ++_count;
                //}
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
                //Create enemies
                CreateEnemyNode();

                // TODO: UPDATE TO BE BETTER
                CreatePickups();

                // Enable movement
                _player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = true;
                _player.GetComponent<Mage>().enabled = true;

                _gameManager.LoadingFinished();

            }
        }
        else
        {
            enabled = false;
        }
    }
    private void CreatePickups()
    {
        // For now spawn a pickup in the middle room
        Instantiate(_pickupPrefabs[0].gameObject, _builtPieces[0].Pivot.transform.position + new Vector3(3.0f,2.0f,3.0f), _pickupPrefabs[0].gameObject.transform.rotation);
        Instantiate(_pickupPrefabs[1].gameObject, _builtPieces[0].Pivot.transform.position - new Vector3(3.0f, 2.0f, 3.0f), _pickupPrefabs[1].gameObject.transform.rotation);
    }


    private void CreateEnemyNode()
    {
        //Once we can tell which rooms are connected to the enemy spawn room Ill implement this algorithm:
        //1. Spawn enemies based on the number of rooms connected and slightly adjusted i.e. randomise it so some casses have more enemies some less 
        //2. Place patrol points in each connected room
        //3. The amount of rooms will be random at first but once difficulty level applies it will change according to that

        int index = 0;
        //Single enemy spawn keep for testing attacks
        /*_enemyClones.Add(Instantiate(_enemyPrefab, _builtPieces[1].Pivot.transform.position, Quaternion.identity));
        _enemyClones[0].GetComponentInChildren<EnemyController>().Enable();
        _enemyClones[0].GetComponentInChildren<EnemyController>()._patrolPoint.transform.position = _builtPieces[1 + 1].Pivot.transform.position;
        */
        foreach (DungeonPiece piece in _builtPieces)
        {
            _enemyClones.Add(Instantiate(_enemyPrefab, _builtPieces[index].Pivot.transform.position, Quaternion.identity));
            _enemyClones[index].GetComponentInChildren<EnemyController>().Enable();
            _enemyClones[index].GetComponentInChildren<EnemyController>()._patrolPoint.transform.position = _builtPieces[index + 1].Pivot.transform.position;
            if (index == 0)//Temporary to get rid of enemy that spawn in main room
            {
                _enemyClones[index].transform.position = _builtPieces[index+1].Pivot.transform.position;
            }

            ++index;

            if (index >= _builtPieces.Count - 1)
            {
                --index;
            }
        }
    }


    // Generates a level
    private void StartGeneration()
    {
        _random = new Random(DateTime.Now.Millisecond);

        // Place first room and pull DungeonPiece component
        _builtPieces.Add(Instantiate(_startingPiece).GetComponent<DungeonPiece>());

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
        dungeonPiece.Initalise();
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
        Vector3 newConnectorPos = connector.transform.position /*+ connector.transform.forward*/;   // This is the location the new connector will sit

        // 2. Place root at connector post
        dungeonPiece.Pivot.transform.position = newConnectorPos;

        // 3. Move by the distance between the conncctors
        Vector3 movementVector = linkConnector.transform.position - newConnectorPos;
        dungeonPiece.Pivot.transform.position -= movementVector;

        // Wait until next physics update to ensure trigger works
        yield return new WaitForFixedUpdate();

        // 4. At this point on trigger enter will collide and invalid the piece if needed
        // If this component isnt found the prefab is wrong.
        bool valid = dungeonPiece.Pivot.GetComponent<DungeonValidator>().Valid;
        //Debug.Log("Placement: " + valid);
        if (valid)
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

            _surfaces.Add(dungeonPiece.gameObject.GetComponent<NavMeshSurface>());
            _surfaces[_surfaces.Count - 1].BuildNavMesh();
        }
        else
        {
            // Delete it
            DestroyImmediate(dungeonPiece.gameObject);

            // Plug the gap
            GameObject plug = Instantiate(_doorPlug);

            plug.transform.rotation = connector.transform.rotation;
            plug.transform.position = connector.transform.position;
            plug.transform.position = plug.transform.position + new Vector3(0.0f, 2.5f, 0.0f) - (connector.transform.forward * 0.5f);

        }

        yield return null;
    }

    #endregion
}
