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
    [SerializeField] private uint _floorCount;

    [SerializeField] private GameObject _player;

    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private GameManager _gameManager;

    [SerializeField] private PickupBase[] _pickupPrefabs;

    [SerializeField] private DungeonPortal _portalPrefab;

    public NavMeshSurface surface;

    #endregion

    #region Private Data

    class DungeonFloor
    {
        public GameObject Root;
        public GameObject Plugs;
        public GameObject Enemies;
        public GameObject Pickups;

        public List<DungeonPiece> BuiltPieces;
        public Queue<NodeConnector> ActiveConnectors;

        public bool Complete;
        public uint Count;
    }

    private Random _random;
    private List<GameObject> _enemyClones;

    private List<DungeonFloor> _floorObjs;

    private uint _currentFloor;

    #endregion

    #region Functions
    private void Start()
    {
        _enemyClones = new List<GameObject>();

        _floorObjs = new List<DungeonFloor>();

        StartGeneration();

    }

    private void Update() 
    {
        bool anyLeft = false;
        foreach (DungeonFloor floor in _floorObjs)
        {
            if (floor.Count < _dungeonSize)
            {
                anyLeft = true;
                StartCoroutine("ProcessSection", floor);
                floor.Count += 1;
            }
            else
            {
                anyLeft = false;
                FinaliseFloor(floor);
            }
        }
        if (!anyLeft)
        {
            _gameManager.LoadingFinished();

            foreach (DungeonFloor floor in _floorObjs)
            {
                floor.Root.SetActive(false);
            }

            _floorObjs[0].Root.SetActive(true);
            _player.transform.position = _floorObjs[0].BuiltPieces[0].Pivot.transform.position;

            enabled = false;
        }
    }

    private void FinaliseFloor(DungeonFloor floor)
    {
        // When its done
        foreach (DungeonPiece piece in floor.BuiltPieces)
        {
            piece.SetValidatorsState(false);
        }

   

        floor.Complete = true;

        //Create the nav mesh
        surface.BuildNavMesh();
        //Create enemies
        CreateEnemyNode(floor);

        // TODO: UPDATE TO BE BETTER
        CreatePickups(floor);

        // Plug all remaining doors
        PlugGaps(floor);

        // Place the portal in the last room TODO: Make it last, using 2nd for now
        PlacePortal(floor);
    }

    private void PlugGaps(DungeonFloor floor)
    {
        foreach (NodeConnector connector in floor.ActiveConnectors)
        {
            // Plug the gap
            GameObject plug = Instantiate(_doorPlug, floor.Plugs.transform);

            plug.transform.rotation = connector.transform.rotation;
            plug.transform.position = connector.transform.position;
            plug.transform.position = plug.transform.position + new Vector3(0.0f, 2.5f, 0.0f) - (connector.transform.forward * 0.5f);
        }
    }

    private void CreatePickups(DungeonFloor floor)
    {
        // For now spawn a pickup in the middle room
        Instantiate(_pickupPrefabs[0].gameObject, floor.BuiltPieces[0].Pivot.transform.position + new Vector3(3.0f,2.0f,3.0f), _pickupPrefabs[0].gameObject.transform.rotation, floor.Pickups.transform);
        Instantiate(_pickupPrefabs[1].gameObject, floor.BuiltPieces[0].Pivot.transform.position - new Vector3(3.0f, 2.0f, 3.0f), _pickupPrefabs[1].gameObject.transform.rotation, floor.Pickups.transform);
    }

    private void PlacePortal(DungeonFloor floor)
    {
        // Place the portal in the last room TODO: Make it last, using 2nd for now
        Instantiate(_portalPrefab.gameObject, floor.BuiltPieces[2].Pivot.transform.position + new Vector3(0, 2.0f, 0), _portalPrefab.gameObject.transform.rotation, floor.Pickups.transform);
    }

    private void CreateEnemyNode(DungeonFloor floor)
    {
        //Once we can tell which rooms are connected to the enemy spawn room Ill implement this algorithm:
        //1. Spawn enemies based on the number of rooms connected and slightly adjusted i.e. randomise it so some casses have more enemies some less 
        //2. Place patrol points in each connected room
        //3. The amount of rooms will be random at first but once difficulty level applies it will change according to that

        for (int i = 0; i < floor.BuiltPieces.Count - 1; i += 2)
        {
            _enemyClones.Add(Instantiate(_enemyPrefab, floor.BuiltPieces[i].Pivot.transform.position, Quaternion.identity));
            _enemyClones[_enemyClones.Count - 1].GetComponentInChildren<EnemyController>().Enable();
            _enemyClones[_enemyClones.Count - 1].GetComponentInChildren<EnemyController>()._patrolPoint.transform.position = floor.BuiltPieces[i + 1].Pivot.transform.position;
            if (i == 0)//Temporary to get rid of enemy that spawn in main room
            {
                _enemyClones[_enemyClones.Count - 1].transform.position = floor.BuiltPieces[i + 1].Pivot.transform.position;
            }
            _enemyClones[_enemyClones.Count - 1].transform.parent = floor.Enemies.transform;
        }
    }


    // Generates a level
    private void StartGeneration()
    {
        _random = new Random(DateTime.Now.Millisecond);

        for (uint i = 0; i < _floorCount; ++i)
        {
            // Add a new floor object
            DungeonFloor floor = new DungeonFloor();
            // Create the subobjects
            floor.Root = new GameObject("Floor");
            // Place roots progressively lower in Y
            floor.Root.transform.position = new Vector3(0.0f, -i * 30.0f, 0.0f);

            floor.Plugs = new GameObject("Plugs");
            floor.Enemies = new GameObject("Enemies");
            floor.Pickups = new GameObject("Pickups"); 
            floor.BuiltPieces = new List<DungeonPiece>();
            floor.ActiveConnectors = new Queue<NodeConnector>();
            floor.Complete = false;
            floor.Count = 0u;

            floor.Root.transform.parent = transform;
            floor.Plugs.transform.parent = floor.Root.transform;
            floor.Enemies.transform.parent = floor.Root.transform;
            floor.Pickups.transform.parent = floor.Root.transform;

            // Place first room and pull DungeonPiece component
            floor.BuiltPieces.Add(Instantiate(_startingPiece, floor.Root.transform).GetComponent<DungeonPiece>());

            // Add its connectors to queue
            foreach (NodeConnector node in floor.BuiltPieces[0].Nodes)
            {
                floor.ActiveConnectors.Enqueue(node);
            }

            _floorObjs.Add(floor);
        }


       


    }

    private IEnumerator ProcessSection(DungeonFloor floor)
    {
        // Take a connector
        NodeConnector connector = floor.ActiveConnectors.Dequeue();

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

            dungeonPiece = Instantiate(picked, floor.Root.transform).GetComponent<DungeonPiece>();
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
                floor.ActiveConnectors.Enqueue(node);
            }

            // Change back trigger state
            dungeonPiece.SetTriggerState(false);

            floor.BuiltPieces.Add(dungeonPiece);

            /*_surfaces.Add(dungeonPiece.gameObject.GetComponent<NavMeshSurface>());
            _surfaces[_surfaces.Count - 1].BuildNavMesh();*/
        }
        else
        {
            // Delete it
            DestroyImmediate(dungeonPiece.gameObject);

            // Plug the gap
            GameObject plug = Instantiate(_doorPlug, floor.Plugs.transform);

            plug.transform.rotation = connector.transform.rotation;
            plug.transform.position = connector.transform.position;
            plug.transform.position = plug.transform.position + new Vector3(0.0f, 2.5f, 0.0f) - (connector.transform.forward * 0.5f);

        }

        yield return null;
    }

    #endregion
}
