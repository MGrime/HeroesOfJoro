using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This node is part of the room prefabs and tells the generator how to connect this point to other prefabs
[System.Serializable]
public class NodeConnector : MonoBehaviour
{
    #region Type Definitons

    // Add other connection types here as needed
    [System.Serializable]
    public enum ConnectorType
    {
        Hallway,
        Room
    }
    // Tells 
    [System.Serializable]
    public enum PointingOrientation
    {
        PosZ,
        PosX,
        NegZ,
        NegX
    }

    #endregion

    #region Editor Fields

    [SerializeField] private ConnectorType _connectionType;
    [SerializeField] private PointingOrientation _connectorOrientation;

    #endregion

    #region Code Properties
    public ConnectorType ConnectionType
    {
        get => _connectionType;
        set => _connectionType = value;
    }

    public PointingOrientation ConnectorOrientation
    {
        get => _connectorOrientation;
        set => _connectorOrientation = value;
    }
    

    #endregion
}