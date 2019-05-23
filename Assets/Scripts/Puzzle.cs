﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [UnityEngine.Range(1,10)]
    [SerializeField] private int _blocksPerLine = 4;

    private Camera _camera;
    private Block _emptyBlock;

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
                var block = quad.AddComponent<Block>();
                block.coord = new Vector2Int(row, column);
                block.OnBlockPressed += PlayerMoveBlockInput;

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
        blockToMove.transform.position = positionToMove;
    }

    private bool IsValidMove(Block blockToMove)
    {
        return IsValidMove(blockToMove.coord);
    }

    private bool IsValidMove(Vector2Int coord)
    {
        var emptyBlockCoord = _emptyBlock.coord;

        if (coord.x == emptyBlockCoord.x)
        {
            if (Mathf.Abs(coord.y - emptyBlockCoord.y) == 1)
            {
                return true;
            }
        }

        if (coord.y == emptyBlockCoord.y)
        {
            if (Mathf.Abs(coord.x - emptyBlockCoord.x) == 1)
            {
                return true;
            }
        }

        return false;
    }
}

/*
  20 21 22
  10 11 12
  00 01 02
*/
