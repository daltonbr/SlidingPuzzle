using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Puzzle : MonoBehaviour
{
    [Header("Shuffle")]
    [SerializeField]
    private int shuffleLength;

    [Header("Board & move")]
    [SerializeField] private float defaultMoveDuration = 0.3f;
    [SerializeField] private float shuffleMoveMoveDuration = 0.1f;
    [UnityEngine.Range(1,10)]
    [SerializeField] private int _blocksPerLine = 4;
    [SerializeField] private Texture2D image;
    private Camera _camera;
    private Block _emptyBlock;
    private Block[,] _blocks;
    private Queue<Block> moveQueue = new Queue<Block>();
    private Coroutine _animationCoroutine;
    private int _shuffleMovesRemaining;
    private Vector2Int _previousShuffleOffset;

    private enum PuzzleState
    {
        Solved,
        Shuffling,
        InPlay
    };

    private PuzzleState _state;
    private bool _blockIsMoving;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        AdjustCameraView(_blocksPerLine);
        InstantiateQuads(_blocksPerLine);
    }

    private void Update()
    {
        if (_state == PuzzleState.Solved && Input.GetKeyUp(KeyCode.Space))
        {
            StartShuffle();
        }
    }

    private void StartShuffle()
    {
        _state = PuzzleState.Shuffling;
        _shuffleMovesRemaining = shuffleLength;

        MakeNextShuffleMove();
    }

    public void InstantiateQuads(int blocksPerLine)
    {
        _blocks = new Block[blocksPerLine, blocksPerLine];
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
                block.OnFinishedMoving += HandleBlockFinishedMoving;

                block.Init(new Vector2Int(row, column), imageSlices[row, column]);
                _blocks[row, column] = block;
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
        if (_state == PuzzleState.InPlay)
        {
            moveQueue.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    private void MoveBlock(Block blockToMove, float duration)
    {
        if (!IsValidMove(blockToMove.coord))
        {
            return;
        }

        _blocks[blockToMove.coord.x, blockToMove.coord.y] = _emptyBlock;
        _blocks[_emptyBlock.coord.x, _emptyBlock.coord.y] = blockToMove;

        // Change the coord's
        var tempCoord = _emptyBlock.coord;
        _emptyBlock.coord = blockToMove.coord;
        blockToMove.coord = tempCoord;

        // Change the transform's
        var positionToMove = _emptyBlock.transform.position;
        _emptyBlock.transform.position = blockToMove.transform.position;
        _blockIsMoving = true;
        blockToMove.MoveToPosition(positionToMove, duration);
    }

    private void HandleBlockFinishedMoving()
    {
        _blockIsMoving = false;
        if (_state == PuzzleState.InPlay)
        {
            MakeNextPlayerMove();
        }
        else if (_state == PuzzleState.Shuffling)
        {
            if (_shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                _state = PuzzleState.InPlay;
            }
        }
    }

    private void MakeNextPlayerMove()
    {
        while (moveQueue.Count > 0 && !_blockIsMoving)
        {
            MoveBlock(moveQueue.Dequeue(), defaultMoveDuration);
        }
    }

    private bool IsValidMove(Block blockToMove)
    {
        return IsValidMove(blockToMove.coord);
    }

    private bool IsValidMove(Vector2Int coord)
    {
        return (coord - _emptyBlock.coord).sqrMagnitude == 1;
    }

    private void MakeNextShuffleMove()
    {
        Vector2Int[] offsets =
        {
            new Vector2Int (1, 0),    // right
            new Vector2Int( 0, 1),    // above
            new Vector2Int(-1, 0),    // bellow
            new Vector2Int( 0,-1)     // left
        };

        int randomIndex = Random.Range(0, offsets.Length);
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != _previousShuffleOffset * -1)
            {
                Vector2Int moveBlockCoord = _emptyBlock.coord + offset;
                if (IsInsideBoundary(moveBlockCoord))
                {
                    MoveBlock(_blocks[moveBlockCoord.x, moveBlockCoord.y], shuffleMoveMoveDuration);
                    _shuffleMovesRemaining--;
                    _previousShuffleOffset = offset;
                    break;
                }
            }
        }
    }

    private bool IsInsideBoundary(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < _blocksPerLine && coord.y >= 0 && coord.y < _blocksPerLine;
    }
}

