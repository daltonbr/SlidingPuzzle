using System;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [UnityEngine.Range(1,10)]
    [SerializeField] private int _blocksPerLine = 4;
    [SerializeField] private Texture2D image;
    private Camera _camera;
    private Block _emptyBlock;
    //private Queue<Block> moveQueue = new Queue<Block>();
    private Coroutine _animationCoroutine;

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
        var imageSlices = ImageSlicer.GetSlices(image, _blocksPerLine);
        var offset = blocksPerLine / 2f - 0.5f;

        for (int row = 0; row < blocksPerLine; row++)
        {
            for (int column = 0; column < blocksPerLine; column++)
            {
                var instantiatePosition = new Vector3(row - offset, column - offset, 0f);
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = instantiatePosition;
                quad.transform.SetParent(transform);
                quad.gameObject.name = $"quad{row}x{column}";
                var block = quad.AddComponent<Block>();
                block.OnBlockPressed += PlayerMoveBlockInput;

                block.Init(new Vector2Int(row, column), imageSlices[row, column]);

                if (column == blocksPerLine - 1 && row == 0)
                {
                    quad.SetActive(false);
                    _emptyBlock = block;
                }
            }
        }
    }

    //TODO: make this adjustment happens when resolution change
    private void AdjustCameraView(int blocksPerLine)
    {
        const float offset = 1f;
        if (_camera == null)
        {
            throw new NullReferenceException();
        }

        if (_camera.aspect > 1)
        {
            _camera.orthographicSize = (blocksPerLine / 2f) + offset;
        }
        else
        {
            _camera.orthographicSize = (blocksPerLine / 2f / _camera.aspect) + offset;
        }
    }

    /// <summary>
    /// Move a block if allowed
    /// </summary>
    private void PlayerMoveBlockInput(Block blockToMove)
    {
        //Debug.Log($"Block pressed: {blockToMove.gameObject.name}");

        if (!IsValidMove(blockToMove.coord))
        {
            return;
        }

        // Change the coord's
        var tempCoord = _emptyBlock.coord;
        _emptyBlock.coord = blockToMove.coord;
        blockToMove.coord = tempCoord;

        // Change the transform's
        var positionToMove = _emptyBlock.transform.position;
        _emptyBlock.transform.position = blockToMove.transform.position;
        blockToMove.MoveToPosition(positionToMove, 0.5f);
    }

    private bool IsValidMove(Block blockToMove)
    {
        return IsValidMove(blockToMove.coord);
    }

    private bool IsValidMove(Vector2Int coord)
    {
        return (coord - _emptyBlock.coord).sqrMagnitude == 1;
    }
}

