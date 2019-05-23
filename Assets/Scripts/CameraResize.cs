using UnityEngine;
using UnityEngine.EventSystems;

public class CameraResize : UIBehaviour
{
    private Camera _camera;
    private bool _isAwake;
    //public int BlocksPerLine => GameManager.Instance.BlockPerLine;

    protected override void Awake()
    {
        _camera = Camera.main;
        _isAwake = true;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        if (!_isAwake) return;

//        AdjustCameraView();
    }

//    private void AdjustCameraView()
//    {
//        if (_camera == null) return;
//        var blocksPerLine = GameManager.Instance.BlockPerLine;
//
//        if (_camera.aspect > 1)
//        {
//            _camera.orthographicSize = BlocksPerLine / 2f;
//        }
//        else
//        {
//            _camera.orthographicSize = BlocksPerLine / 2f / _camera.aspect;
//        }
//    }
}
