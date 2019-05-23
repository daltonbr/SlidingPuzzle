using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPressed;
    public Vector2Int coord;

    public void Init(Vector2Int startingCoord, Texture2D texture2D)
    {
        coord = startingCoord;
        GetComponent<MeshRenderer>().material.mainTexture = texture2D;
    }

    private void OnMouseDown()
    {
        OnBlockPressed?.Invoke(this);
    }

}
