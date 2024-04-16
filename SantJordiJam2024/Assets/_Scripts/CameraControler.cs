using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraControler : MonoBehaviour
{
    #region Fields
    [Space(3), Header("References"), Space(3)]
    [SerializeField] ObjectPlacer _objectPlacer;
    [SerializeField] Camera _camera;
    [SerializeField] Transform _movementTransform;

    [Space(3), Header("Camera Properties"), Space(3)]
    [SerializeField] CamLevel _closeCam;
    [SerializeField] CamLevel _midCam;
    [SerializeField] CamLevel _farCam;

    #endregion

    #region Vars

    private bool _dragging;
    private Vector2 _mouseDelta;
    private CamLevel _currentCamLevel;

    #endregion

    #region UnityFunctions

    private void Start()
    {
        if (_camera == null) _camera = Camera.main;
        if (_objectPlacer == null) _objectPlacer = FindObjectOfType<ObjectPlacer>();
        if (_movementTransform == null) _movementTransform = transform;
        
        _currentCamLevel = DetermineCamLevel();
    }

    private void LateUpdate()
    {
        _camera.transform.localPosition = Vector3.forward * -(30f + _camera.orthographicSize);

        if (_dragging && _objectPlacer._PlayerState == PlayerState.Idle)
        {
            Vector3 pos = (transform.right * _mouseDelta.x + transform.up * _mouseDelta.y) * -Mathf.Lerp(_currentCamLevel._DragSpeed.x, _currentCamLevel._DragSpeed.y, (_camera.orthographicSize - _currentCamLevel._ZoomRange.x)/ _currentCamLevel._ZoomRange.y) ;
            transform.position += pos * Time.deltaTime;
        }
    }

    #endregion

    #region Methods

    public void OnLook(InputAction.CallbackContext context) => _mouseDelta = context.ReadValue<Vector2>();

    public void OnMove (InputAction.CallbackContext context) => _dragging = context.started || context.performed;

    public void OnZoom(InputAction.CallbackContext context) => Zoom(context.ReadValue<float>() * Time.deltaTime * -_closeCam._ZoomSpeed);

    private void Zoom(float zoom)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + zoom, _closeCam._ZoomRange.x, _farCam._ZoomRange.y);
        _currentCamLevel = DetermineCamLevel();
        //transform.position -= transform.forward * _camera.orthographicSize;
    }

    private CamLevel DetermineCamLevel()
    {
        //CLOSE
        if (_camera.orthographicSize >= _closeCam._ZoomRange.x && _camera.orthographicSize < _closeCam._ZoomRange.y && _currentCamLevel != _closeCam)
        {
            return CamTransition(_closeCam);
        }
        //MID
        else if (_camera.orthographicSize >= _midCam._ZoomRange.x && _camera.orthographicSize < _midCam._ZoomRange.y && _currentCamLevel != _midCam)
        {
            return CamTransition(_midCam);
        }
        //FAR
        else if (_camera.orthographicSize >= _farCam._ZoomRange.x && _currentCamLevel != _farCam)
        {
            return CamTransition(_farCam);
        }
        else return _currentCamLevel; //no changes
    }

    private CamLevel CamTransition(CamLevel cam)
    {
        _camera.DOOrthoSize(cam._DefaultZoom, cam._TransTime);
        transform.DOMove(new Vector3(-80f, transform.position.y, -80f), cam._TransTime);
        return cam;
    }

    #endregion
}

[System.Serializable]
public class CamLevel
{
    [Header("Controls"), Space(3)]
    [Tooltip("X component is applied at max zoom, Y component is applied at min zoom")] public Vector2 _DragSpeed;
    public float _ZoomSpeed;

    [Space(3), Header("Range"), Space(3)]
    public float _DefaultZoom;
    public Vector2 _ZoomRange;

    [Space(3), Header("Animation"), Space(3)]
    public float _TransTime;
}