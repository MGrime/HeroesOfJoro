using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSizeSync : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private RectTransform _parentSize;
    [SerializeField] private RectTransform _imageSize;

    #endregion

    #region Functions

    private void Start()
    {
        Sync();
    }

    private void Update()
    {
        if (!Mathf.Approximately(_imageSize.rect.width,_parentSize.rect.width) || !Mathf.Approximately(_imageSize.rect.height, _parentSize.rect.height))
        {
            Sync();
        }
    }

    private void Sync()
    {
        float squareSize = _parentSize.rect.width;

        if (_parentSize.rect.height > _parentSize.rect.width)
        {
            squareSize = _parentSize.rect.width;
        }

        _imageSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, squareSize);
        _imageSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, squareSize);
    }

    #endregion
}
