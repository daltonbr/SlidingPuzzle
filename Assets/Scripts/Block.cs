using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Block : MonoBehaviour
{
    public event System.Action<Block> OnBlockPressed;
    public event System.Action OnFinishedMoving;
    public Vector2Int coord;
    private Vector2Int _startingCoord;

    public void Init(Vector2Int startingCoord, Texture2D texture2D)
    {
        _startingCoord = coord = startingCoord;
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Block");
        GetComponent<MeshRenderer>().material.mainTexture = texture2D;
    }

    private void OnMouseDown()
    {
        OnBlockPressed?.Invoke(this);
    }

    public void MoveToPosition(Vector2 target, float duration)
    {
        StartCoroutine(AnimateMove(target, duration));
    }

    private IEnumerator AnimateMove(Vector2 target, float duration)
    {
        var initialPosition = transform.position;
        float percent = 0f;
        while (percent < 1f)
        {
            percent +=  Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPosition, target, percent);
            yield return null;
        }

        transform.position = target;
        OnFinishedMoving?.Invoke();
    }

    public bool IsAtStartingCoord()
    {
        return coord == _startingCoord;
    }

}
