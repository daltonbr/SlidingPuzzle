using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _blockPerLine = 4;
    [SerializeField] private Puzzle _puzzle;

    private GameManager _instance;

    public GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return _instance = this;
            }

            return _instance;
        }
    }

    public int BlockPerLine
    {
        get => _blockPerLine;
        set => _blockPerLine = value;
    }

    private void Awake()
    {
        // TODO: assert

        if (Instance == null)
        {
            _instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    private void InitGame()
    {
        _puzzle.InstantiateQuads(BlockPerLine);
    }

}
