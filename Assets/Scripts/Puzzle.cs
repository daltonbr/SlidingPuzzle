using System;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [Range(1,10)]
    [SerializeField] private int _blocksPerLine = 4;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        AdjustCameraView(_blocksPerLine);
        InstantiateQuads(_blocksPerLine);
    }

    public void InstantiateQuads(int blocksPerLine)
    {
        if (blocksPerLine <= 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        var offset = blocksPerLine / 2f - 0.5f;

        for (int row = 0; row < blocksPerLine; row++)
        {
            for (int column = 0; column < blocksPerLine; column++)
            {
                var instantiatePosition = new Vector3(column - offset, row - offset, 0f);
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = instantiatePosition;
                quad.transform.SetParent(transform);
                quad.gameObject.name = $"quad{row}x{column}";
            }
        }

        // TODO: remove the block on the bottom right.
    }

    private void AdjustCameraView(int blocksPerLine)
    {
        if (_camera == null)
        {
            throw new NullReferenceException();
        }

        if (_camera.aspect > 1)
        {
            _camera.orthographicSize = blocksPerLine / 2f;
        }
        else
        {
            _camera.orthographicSize = blocksPerLine / 2f / _camera.aspect;
        }
    }

}
