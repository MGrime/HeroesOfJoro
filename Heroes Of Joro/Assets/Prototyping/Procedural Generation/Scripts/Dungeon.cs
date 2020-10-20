using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

// This class processes the most stuff
public class Dungeon : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject[] _roomPrefabs;
    [SerializeField] private GameObject[] _hallwayPrefabs;

    #endregion

    #region Private Data

    private List<DungeonPiece> _builtPieces;
    private List<NodeConnector> _activeConnectors;
    private Random _random;

    #endregion

    #region Functions
    void Start()
    {
        StartGeneration();
    }

    // Generates a level
    void StartGeneration()
    {
        _random = new Random(DateTime.Now.Millisecond);

        // Place first room and pull DungeonPiece component
        _builtPieces.Add(Instantiate(_roomPrefabs[_random.Next(0, _roomPrefabs.Length)].GetComponent<DungeonPiece>())) ;

        // Add its connectors to list
        foreach (NodeConnector node in _builtPieces[0].Nodes)
        {
            _activeConnectors.Add(node);
        }

        // Loop till connectors empty
    }

    #endregion
}
