using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance{get{return _instance;}}

    private List<CargoLoot> _lootInScene = new List<CargoLoot>();
    public List<CargoLoot> LootsInScene{get{return _lootInScene;}}

    private void Awake()
    {
        _instance = this;
    }

    public void AddPawn(MonoBehaviour m)
    {
        if(m is CargoLoot)
            _lootInScene.Add(m as CargoLoot);
    }

    public void RemovePawn(MonoBehaviour m)
    {
        if(m is CargoLoot)
            _lootInScene.Remove(m as CargoLoot);
    }
}
