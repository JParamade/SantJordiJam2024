using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;

public class Spawner : MonoBehaviour
{
    [Space(3), Header("Scene References"), Space(3)]
    [SerializeField] GridManager _gridManager;

    [Space(3), Header("Properties"), Space(3)]
    [SerializeField] Vector2 _spawns;
    [SerializeField] AnimationCurve _density;
    

    [Space(3), Header("Spaw Table"), Space(3)]
    [SerializeField] SpawnObject[] _spawnObjects;

    private Dictionary<GridObjectSO, int> _spawnTable;
    private AnimationCurveSampler _sampler;

    private void Start()
    {
        //scene references
        if (_gridManager == null) _gridManager = FindObjectOfType<GridManager>();

        //setup dictionary
        for (int i = 0; i < _spawnObjects.Length; i++) _spawnTable.Add(_spawnObjects[i]._GridObject, _spawnObjects[i]._weight);

        //spawning
        _sampler = new AnimationCurveSampler(_density);

        for (int i = 0; i < Random.Range(_spawns.x, _spawns.y); i++)
        {
            //first, determine object


            //then, determine position
        }
    }
}

[System.Serializable]
public class SpawnObject
{
    public GridObjectSO _GridObject;
    public int _weight;
}
