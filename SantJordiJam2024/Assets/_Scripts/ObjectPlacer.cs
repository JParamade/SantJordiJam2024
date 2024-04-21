using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;

public class ObjectPlacer : MonoBehaviour
{
    #region Fields
    [Space(3), Header("Scene References"), Space(3)]
    [SerializeField] Camera _cam;
    [SerializeField] GridManager _gridManager;

    [Space(3), Header("Controls"), Space(3)]
    [SerializeField] string _buildButtonName;
    [SerializeField] string _placeButtonName;
    [SerializeField] string _deleteButtonName;
    [SerializeField] string _rotateAxisName;
    [SerializeField] LayerMask _placeableLayerMask;

    [Space(3), Header("Visuals"), Space(3)]
    [SerializeField] Color _canPlaceColor;
    [SerializeField] Color _cannotPlaceColor;

    #endregion

    #region Vars

    private GridObjectSO _selectedObj;
    public GridObjectSO _obj1;
    public GridObjectSO _obj2;
    public GridObjectSO _obj3;

    private Dictionary<GridObjectSO, PreviewObject> _previewObjects;
    private PreviewObject _currentPreview;
    [HideInInspector] public PlayerState _PlayerState;

    #endregion

    #region Unity Functions

    private void Start()
    {
        if(_cam == null) _cam = Camera.main;
        if(_gridManager == null) _gridManager = FindObjectOfType<GridManager>();

        _selectedObj = null;
        _previewObjects = new Dictionary<GridObjectSO, PreviewObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSelection(_obj1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSelection(_obj2);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSelection(_obj3);

        //INPUTS
        if (Input.GetButtonDown(_buildButtonName))
        {
            if (_PlayerState == PlayerState.Idle) SetIsBuilding(true);
            else SetIsBuilding(false);
        }
        else if (Input.GetButtonDown(_placeButtonName)) TryPlace();
        else if (Input.GetButtonDown(_deleteButtonName)) TryDestroy();

        //BUILDING
        if (_PlayerState == PlayerState.Building && _currentPreview != null)
        {
            GridObject currCell = _gridManager.WorldPosToCell(GetMouseWorldPos());

            _currentPreview.transform.position = currCell._Position;

            _currentPreview.transform.Rotate(new Vector3(0f, 90f * Input.GetAxisRaw(_rotateAxisName), 0f));

            foreach (MeshRenderer rend in _currentPreview._Renderers)
                rend.material.SetColor("_BaseColor", ExtendedDataUtility.Select(IsThereRoom(currCell._Coords, _selectedObj._Size, (int)_currentPreview.transform.rotation.eulerAngles.y), _canPlaceColor, _cannotPlaceColor));
        }
    }

    #endregion

    #region Methods
    private void ChangeSelection(GridObjectSO sel)
    {
        if (_PlayerState == PlayerState.Building && _currentPreview != null && _selectedObj != sel)
        {
            UpdatePreviewObject(_selectedObj, false);
            UpdatePreviewObject(sel, true);
        }
        _selectedObj = sel;
    }


    private void SetIsBuilding(bool set)
    {
        if(_selectedObj == null) return;

        _PlayerState = set? PlayerState.Building : PlayerState.Idle;

        UpdatePreviewObject(_selectedObj, set);
    }

    private void UpdatePreviewObject(GridObjectSO gridObject, bool set = true)
    {
        if (_previewObjects.ContainsKey(gridObject) &&
        _previewObjects.TryGetValue(gridObject, out PreviewObject obj))
        {
            obj.SetActive(set);
            _currentPreview = set ? obj : null;
        }
        else
        {
            PreviewObject prevObj = new PreviewObject(Instantiate(gridObject._Prefab, transform));
            _previewObjects.Add(gridObject, prevObj);
            prevObj.SetActive(set);
            _currentPreview = set ? prevObj : null;
        }
    }

    private bool TryDestroy()
    {
        SetIsBuilding(false);

        GridObject gObj = _gridManager.WorldPosToCell(GetMouseWorldPos());
        if (gObj._Occupant != null)
        {
            Destroy(gObj._Occupant);
            return true;
        }
        else return false;
    }

    private bool TryPlace()
    {
        if (_selectedObj == null || _currentPreview == null || _PlayerState != PlayerState.Building) return false;

        Quaternion rot = _currentPreview.transform.rotation;

        SetIsBuilding(false);

        Vector2Int coords = _gridManager.WorldPosToCoords(GetMouseWorldPos());

        if (IsThereRoom(coords, _selectedObj._Size, (int)rot.eulerAngles.y))
        {
            GameObject go = Instantiate(_selectedObj._Prefab, _gridManager._Grid[coords.x, coords.y]._Position, rot);
            go.transform.SetParent(_gridManager.transform);
            _gridManager.RunThroughGrid(coords, _selectedObj._Size, (gObj) => gObj._Occupant = go, (Direction)((int)rot.eulerAngles.y / 90));

            return true;
        }
        else return false;
    }

    private bool IsThereRoom(Vector2Int origin, Vector2Int spread, int yRot)
    {
        bool isRoom = true;

        //All current spaces need to be InBounds and Unoccupied
        _gridManager.RunThroughGrid(origin, spread, (x, y) =>
        { 
            if (!_gridManager.AreCoordsOnGrid(x, y) || (_gridManager.AreCoordsOnGrid(x, y)) && _gridManager._Grid[x, y]._Occupant != null) isRoom = false;
        }, (Direction)(yRot / 90));

        return isRoom;
    }

    private Vector3 GetMouseWorldPos()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, _placeableLayerMask, QueryTriggerInteraction.Ignore)) return hit.point;
        else return Vector3.zero;
    }

    #endregion
}

public class PreviewObject
{
    public GameObject _Instance;
    public MeshRenderer[] _Renderers;

    public Transform transform
    {
        get { return _Instance.transform; }
    }

    public void SetActive(bool set)
    {
        _Instance.SetActive(set);
    }

    public PreviewObject(GameObject instance)
    {
        _Instance = instance;

        _Renderers = _Instance.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer rend in _Renderers)
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
