using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPressed;
    public Vector2Int coord;
    private void OnMouseDown()
    {
        OnBlockPressed?.Invoke(this);
    }

}
