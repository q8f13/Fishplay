using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractionBeam 
{
    private float _range;
    // private List<CargoLoot> _tractingLoots = new List<CargoLoot>();

    private System.Action<CargoData> OnLootHandler = null;
    private Transform _bodyT;

    public TractionBeam(Transform body, float range, System.Action<CargoData> lootHandler)
    {
        OnLootHandler = lootHandler;
        _range = range;
        _bodyT = body;
    }

    public void ResetRange(float range)
    {
        _range = range;
    }

    public void CheckLoots(List<CargoLoot> loots_in_level)
    {
        foreach(CargoLoot loot in loots_in_level)
        {
            if(Vector3.Distance(_bodyT.position, loot.transform.position) < _range)
            {
                // _tractingLoots.Add(loot);
                loot.SetTraction(_bodyT, OnLootHandler);
            }
        }
    }
}
