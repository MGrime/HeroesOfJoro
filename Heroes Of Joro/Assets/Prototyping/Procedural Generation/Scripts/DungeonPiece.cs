using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPiece : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private NodeConnector[] _nodeConnectors;
    [SerializeField] private BoxCollider[] _validators;
    [SerializeField] private GameObject _pivotObject;

    #endregion

    #region Code Fields
    public NodeConnector[] Nodes
    {
        get => _nodeConnectors;
        set => _nodeConnectors = value;
    }

    public GameObject Pivot
    {
        get => _pivotObject;
        set => _pivotObject = value;
    }

    #endregion

    #region Functions

    private void Start()
    {
        _validators = _pivotObject.GetComponents<BoxCollider>();
    }

    public void SetTriggerState(bool state)
    {
        foreach (BoxCollider box in _validators)
        {
            box.isTrigger = state;
        }
    }

    public void SetValidatorsState(bool state)
    {
        foreach (BoxCollider box in _validators)
        {
            box.enabled = state;
        }
    }

    #endregion

}
