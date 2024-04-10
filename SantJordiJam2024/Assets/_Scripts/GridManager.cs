using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridManager : MonoBehaviour
{
    #region Fields
    [Space(3), Header("Grid Properties"), Space(3)]
    [SerializeField] Vector2Int _gridSize = new(10, 10);
    public float _CellSize = 5;

    [Space(3), Header("Debug Properties"), Space(3)]
    [SerializeField] GridEditorVisuals _gridVisuals;

    #endregion

    #region Vars

    public GridObject[,] _Grid;

    #endregion

    #region Unity Functions

    private void Start()
    {
        GenerateGrid();
    }

    #endregion

    #region Methods

    public bool AreCoordsOnGrid(int x, int y)
    {
        return ExtendedDataUtility.IsIndexInRange(x, _gridSize.x) && ExtendedDataUtility.IsIndexInRange(y, _gridSize.y);
    }

    public GridObject WorldPosToCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / _CellSize) + _gridSize.x/2;
        int y = Mathf.FloorToInt(pos.z / _CellSize) + _gridSize.y/2;

        return _Grid[Mathf.Clamp(x, 0, _gridSize.x-1), Mathf.Clamp(y, 0, _gridSize.y-1)];
    }

    public Vector2Int WorldPosToCoords(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / _CellSize) + _gridSize.x / 2;
        int y = Mathf.FloorToInt(pos.z / _CellSize) + _gridSize.y / 2;

        return new Vector2Int(Mathf.Clamp(x, 0, _gridSize.x - 1), Mathf.Clamp(y, 0, _gridSize.y - 1));
    }

    [ContextMenu("Actions/Regenerate Grid")]
    public void GenerateGrid()
    {
        _Grid = new GridObject[_gridSize.x, _gridSize.y];

        RunThroughGrid((x, y) => _Grid[x, y] = new GridObject(x, y, new Vector3(x * _CellSize + _CellSize*0.5f - _gridSize.x*_CellSize*0.5f, transform.position.y, y * _CellSize + _CellSize*0.5f - _gridSize.y*_CellSize*0.5f), this));
    }

    [ContextMenu("Actions/Print Grid Contents")]
    public void PrintGrid() => RunThroughGrid((gObj) => Debug.Log(gObj.ToString()));

    public void RunThroughGrid(Action<int, int> func) => RunThroughGrid(new Vector2Int(0, 0), _gridSize, func);
    public void RunThroughGrid(Action<GridObject> func) => RunThroughGrid(new Vector2Int(0, 0), _gridSize, func);
    public void RunThroughGrid(Vector2Int origin, Vector2Int spread, Action<int, int> func, Direction dir = 0)
    {
        CellRunner(origin, spread, dir, (x,y) => func(x,y));
    }

    public void RunThroughGrid(Vector2Int origin, Vector2Int spread, Action<GridObject> func, Direction dir = 0)
    {
        CellRunner(origin, spread, dir, (x, y) => func(_Grid[x, y]));
    }

    private void CellRunner(Vector2Int origin, Vector2Int spread, Direction dir, Action<int, int> cellAction)
    {
        //int incr = (int)dir <= 1 ? 1 : -1; //[0,1] is N,E [2,3] is S,W

        bool verFirst = (int)dir % 2 != 0;
        int sign = (int)dir <= 1 ? 1 : -1;

        Vector2Int start = verFirst ? new Vector2Int(origin.x, origin.y) : new Vector2Int(origin.y, origin.x);
        Vector2Int size = verFirst ? new Vector2Int(spread.x, spread.y) : new Vector2Int(spread.y, spread.x);

        for (int x = start.x; x != start.x + size.x * sign; x += 1)
        {
            for (int y = start.y; y != start.y + size.y * sign; y += 1)
            {
                cellAction(x, y);
            }
        }
    }

    #endregion

    #region Debug

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (_gridVisuals._visualizeOnlyOnSelect) DrawGridVisuals();
    }

    private void OnDrawGizmos()
    {
        if (!_gridVisuals._visualizeOnlyOnSelect) DrawGridVisuals();
    }

    private void DrawGridVisuals()
    {
        if (_Grid == null) return;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.LowerCenter;
        labelStyle.fontSize = _gridVisuals._labelSize;

        //DRAW VERTICAL LINES
        Handles.color = _gridVisuals._linesColor.colorKeys[0].color;
        for (int x = 0; x < _gridSize.x; x++)
        {
            Handles.DrawLine(_Grid[x, 0]._Position - new Vector3(_CellSize * 0.5f, 0f, _CellSize * 0.5f), _Grid[x, _gridSize.y-1]._Position - new Vector3(_CellSize * 0.5f, 0f, -_CellSize * 0.5f));
        }
        Handles.DrawLine(_Grid[_gridSize.x-1, 0]._Position - new Vector3(-_CellSize * 0.5f, 0f, _CellSize * 0.5f), _Grid[_gridSize.x-1, _gridSize.y-1]._Position - new Vector3(-_CellSize * 0.5f, 0f, -_CellSize * 0.5f));

        //DRAW HOPRIZONTAL LINES
        Handles.color = _gridVisuals._linesColor.colorKeys[^1].color;
        for (int y = 0; y < _gridSize.y; y++)
        {
            Handles.DrawLine(_Grid[0, y]._Position - new Vector3(_CellSize * 0.5f, 0f, _CellSize * 0.5f), _Grid[_gridSize.x-1, y]._Position - new Vector3(-_CellSize * 0.5f, 0f, _CellSize * 0.5f));
        }
        Handles.DrawLine(_Grid[0, _gridSize.y - 1]._Position - new Vector3(_CellSize * 0.5f, 0f, -_CellSize * 0.5f), _Grid[_gridSize.x - 1, _gridSize.y - 1]._Position - new Vector3(-_CellSize * 0.5f, 0f, -_CellSize * 0.5f));

        //DRAW LABELS
        RunThroughGrid((gObj) => ColorLabel(ref labelStyle, gObj));
    }

    private void ColorLabel(ref GUIStyle style, GridObject gridObj)
    {
        if (ExtendedMathUtility.VectorXZDistance(Camera.current.transform.position, gridObj._Position) >= _gridVisuals._drawLabelDistance) return;

        style.normal.textColor = ExtendedDataUtility.Select(gridObj._Occupant != null, _gridVisuals._labelColor.colorKeys[^1].color, _gridVisuals._labelColor.colorKeys[0].color);
        Handles.Label(gridObj._Position, "(" + gridObj._Position.x + " : " + gridObj._Position.y + ")" + "\n[" + gridObj._Coords.x + ", " + gridObj._Coords.y + "]", style);
    }

    #endif

    #endregion
}

[System.Serializable]
public struct GridEditorVisuals
{
    public bool _visualizeOnlyOnSelect;
    public Gradient _linesColor;
    public Gradient _labelColor;
    public float _drawLabelDistance;
    public int _labelSize;
}