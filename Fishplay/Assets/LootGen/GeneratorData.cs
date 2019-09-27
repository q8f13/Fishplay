using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gear generator base data
/// source:
/// https://docs.google.com/spreadsheets/d/1Hpb_wZ19MHJ-S5A3fYRPLnEdTjczm7xhtn8ojbEh8G0/edit#gid=138319449
/// </summary>

public class GeneratorData 
{
    public int ID;
    public string NameId;
    public GearEffect Effect;
    public Oper Operation;
    public ValueType Type;
    public GearSlot Slot;
    public float Min;
    public float Max;
    public string Requirement;
    public string Description;
    public string Remarks;
}

[System.Serializable]
public class GeneratorDataRaw
{
    public int id;
    public string name;
    public string type;
    public string oper;
    public string num_type;
    public float min_val;
    public float max_val;
    public float min_pct;
    public float max_pct;
    public string requirement;
    public string desc;
    public string remarks;
}

public enum GearSlot
{
    NotSet = 0,
    Weapon,
    Tech,
    Engine,
    Core,
    Armor,
    Sensor,
    Shield,
}

public enum ValueType
{
    Value = 0,
    Percent,
}

public enum Oper
{
    AddVal = 0,
    AddPercent,
}

public enum GearEffect
{
    NotSet = 0,
    // weapon
    Damage,
    AtkRate,
    DmgRegenEnergy,
    KillRegenEnergy,
    AddSocket,
    DmgVsInorganic,
    DmgVsOrganic,
    DmgVsShield,
    PenetrationRate,
    FreeAmmoRate,
    // tech
    SensorRadius,
    TechEffect,
    Cooldown,
    EnergyCost,
    HitRate,
    // engine
    MaxSpeed,
    ThrustMultiplier,
    ThrustEnergyCost,
    // Core
    EnergyCapacity,
    EnergyRegeneration,
    HullAddPoint,
    ResistToProjectile,
    ResistToExplosive,
    ResistToEnergy,
    ResistToIon,
    ResistToDecay,
    ResistToAll,
    DmgProjectile,
    DmgExplosive,
    DmgIon,
    DmgDecay,
    DmgAll,
    // Armor
    HullAddPercent,
    // sensor
    ResistToDisrupt,
    // shield
}