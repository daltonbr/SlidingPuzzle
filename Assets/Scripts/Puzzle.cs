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
    [SerializeField] private float moveDuration = 0.5f;
    [UnityEngine.Range(1,10)]
    [SerializeField] private int _blocksPerLine = 4;
    [SerializeField] private Texture2D image;
    private Camera _camera;
    private Block _emptyBlock;
    private Block[,] _blocks;
    private Queue<Block> moveQueue = new Queue<Block>();
    private Coroutine _animationCoroutine;
    private int shuffleRemaining;

    // Flags
    private bool _blockIsMoving;
    private bool _readyToShuffle = false;

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
        if (Input.GetKeyUp(KeyCode.Space))
        {
            TryToShuffle();
        }
    }

    private void TryToShuffle()
    {
        _readyToShuffle = true;
        shuffleRemaining = shuffleLength;

        if (!_readyToShuffle) return;

        MakeNextShuffleMove();
        for (int i = 0; i < shuffleLength; i++)
        {

        }
        // define a number of random moves to shuffle. TIP we can save a tuple with the current moves to solve.
        //_emptyBlock

        // apply this "movements"

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
        moveQueue.Enqueue(blockToMove);
        MakeNextPlayerMove();
    }

    private void MoveBlock(Block blockToMove)
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
        blockToMove.MoveToPosition(positionToMove, moveDuration);
    }

    private void HandleBlockFinishedMoving()
    {
        _blockIsMoving = false;
        MakeNextPlayerMove();

        if (shuffleRemaining > 0)
        {
            MakeNextShuffleMove();
        }
    }

    private void MakeNextPlayerMove()
    {
        while (moveQueue.Count > 0 && !_blockIsMoving)
        {
            MoveBlock(moveQueue.Dequeue());
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

        for (int i = 0; i < offsets.Length; i++)
        {
            int randomIndex = Random.Range(0, offsets.Length);
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            Vector2Int moveBlockCoord = _emptyBlock.coord + offset;
            if (IsInsideBoundary(moveBlockCoord))
            {
                MoveBlock(_blocks[moveBlockCoord.x, moveBlockCoord.y]);
                shuffleRemaining--;
                break;
            }
        }
    }

    private bool IsInsideBoundary(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < _blocksPerLine && coord.y >= 0 && coord.y < _blocksPerLine;
    }
}

