using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    public Vector2Int _Coords;
    public Vector3 _Position;
    public GameObject _Occupant;

    public GridObject(int x, int y, Vector3 worldPos, GridManager gridManager)
    {
        _Coords = new Vector2Int(x, y);
        _Position = worldPos;
    }

    public GridObject(Vector2Int coords, Vector3 worldPos, GridManager gridManager)
    {
        _Coords = coords;
        _Position = worldPos;
    }

    public override string ToString()
    {
        return base.ToString() + " : " + _Coords.ToString();
    }
}
