using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPiece : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private NodeConnector[] _nodeConnectors;
    [SerializeField] private BoxCollider[] _validators;

    #endregion

    #region Code Fields
    public NodeConnector[] Nodes
    {
        get => _nodeConnectors;
        set => _nodeConnectors = value;
    }

    #endregion

}
