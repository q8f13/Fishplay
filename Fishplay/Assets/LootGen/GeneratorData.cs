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
    public string Name;
    public GearEffect Effect;
    public Oper Operation;
    public ValueType Type;
    public GearSlot Slot;
    public float Min;
    public float Max;
    public string Requirement;
    public string Description;
    public string Remarks;

    public static GeneratorData Parse(GeneratorDataRaw raw)
    {
        GeneratorData data = new GeneratorData();
        data.ID = raw.id;
        data.Name = raw.name;
        switch(raw.type)
        {
            case "dmg":
                data.Effect = GearEffect.Damage;
                break;
            case "atk_rate":
                data.Effect = GearEffect.AtkRate;
                break;
            case "en_cost":
                data.Effect = GearEffect.EnergyCost;
                break;
            case "dmg_regen_en":
                data.Effect = GearEffect.DmgRegenEnergy;
                break;
            case "kill_regen_en":
                data.Effect = GearEffect.KillRegenEnergy;
                break;
            case "add_socket":
                data.Effect = GearEffect.AddSocket;
                break;
            case "dmg_vs_inorganic":
                data.Effect = GearEffect.DmgVsInorganic;
                break;
            case "dmg_vs_organic":
                data.Effect = GearEffect.DmgVsOrganic;
                break;
            case "dmg_vs_shield":
                data.Effect = GearEffect.DmgVsShield;
                break;
            case "thrust_rate":
                data.Effect = GearEffect.PenetrationRate;
                break;
            case "free_ammo_cost":
                data.Effect = GearEffect.FreeAmmoRate;
                break;
        }

        if(raw.num_type == "pct")
        {
            data.Operation = Oper.AddPercent;
            data.Type = ValueType.Percent;
            data.Min = raw.min_pct / 100.0f;
            data.Max = raw.max_pct / 100.0f;
        }
        else if(raw.num_type == "val")
        {
            data.Operation = Oper.AddVal;
            data.Type = ValueType.Value;
            data.Min = (int)raw.min_val;
            data.Max = (int)raw.max_val;
        }
        else
            throw new System.NotImplementedException();

        data.Description = raw.desc;
        data.Remarks = raw.remarks;

        return data;
    }
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