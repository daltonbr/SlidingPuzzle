using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPressed;
    public Vector2Int coord;

    public void Init(Vector2Int startingCoord, Texture2D texture2D)
    {
        coord = startingCoord;
        var material = GetComponent<MeshRenderer>().material;
        material.shader = Shader.Find("Unlit/Texture");
        material.mainTexture = texture2D;
    }

    private void OnMouseDown()
    {
        OnBlockPressed?.Invoke(this);
    }

}
