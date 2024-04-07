using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    #region Fields
    [Space(3), Header("Scene References"), Space(3)]
    [SerializeField] Camera _cam;
    [SerializeField] GridManager _gridManager;

    [Space(3), Header("Controls"), Space(3)]
    [SerializeField] string _placeButtonName;
    [SerializeField] string _deleteButtonName;
    [SerializeField] LayerMask _placeableLayerMask;

    #endregion

    #region Vars

    public GridObjectSO _selectedObj;
    public GridObject[] _areaBuffer = new GridObject[0];

    #endregion

    #region Unity Functions

    private void Start()
    {
        if(_cam == null) _cam = Camera.main;
        if(_gridManager == null) _gridManager = FindObjectOfType<GridManager>();
    }

    private void Update()
    {
        if(Input.GetButtonDown(_placeButtonName) && _selectedObj != null)
        {
            Vector2Int coords = _gridManager.WorldPosToCoords(GetMouseWorldPos());
            bool valid = true;

            //All current spaces need to be InBounds and Unoccupied
            _gridManager.RunThroughGrid(coords, _selectedObj._Size, (x,y) => {
                if (!_gridManager.AreCoordsOnGrid(x, y) || (_gridManager.AreCoordsOnGrid(x, y)) && _gridManager._Grid[x, y]._Occupant != null) valid = false;});

            if (valid)
            {
                GameObject go = Instantiate(_selectedObj._Prefab, _gridManager._Grid[coords.x, coords.y]._Position, Quaternion.identity);
                go.transform.SetParent(_gridManager.transform);
                _gridManager.RunThroughGrid(coords, _selectedObj._Size, (gObj) => gObj._Occupant = go);
            } 
        }
        else if (Input.GetButtonDown(_deleteButtonName) && _selectedObj != null)
        {
            GridObject gObj = _gridManager.WorldPosToCell(GetMouseWorldPos());

            if (gObj._Occupant != null) Destroy(gObj._Occupant);
        }
    }

    #endregion

    #region Methods



    private Vector3 GetMouseWorldPos()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, _placeableLayerMask, QueryTriggerInteraction.Ignore)) return hit.point;
        else return Vector3.zero;
    }

    #endregion
}
