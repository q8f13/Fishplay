using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissileConfig
{
	public float ExplositionRange;
	public float SpreadSpeed;
}

[System.Serializable]
public class SocketSet
{
	public SocketField Field = SocketField.Notset;
	public PluginConfig PluginOnSet = null;

	public static SocketSet CreateDummy()
	{
		SocketSet s = new SocketSet();
		s.Field = (SocketField)Random.Range(0, 5);
		s.PluginOnSet = null;
		return s;
	}
}

[System.Serializable]
public class PluginBase : IPlugin
{

}

[System.Serializable]
public class PluginConfig
{
	public string Name = "NoName";
	public string Description = "NoDescription";
	public SocketField FieldCompatable = SocketField.Notset;
	public PluginBase PluginSet;
}

public abstract class APerk
{
	private GameObject _owner;

	public APerk(GameObject owner)
	{
		_owner = owner;
	}

	protected abstract void OnAdded();
	protected abstract void OnRemoved();
	protected abstract void Activate();
	protected abstract void Deactivate();
	protected abstract void OnUpdate();
	protected abstract void OnEvent();
}

public interface IPlugin
{

}

public enum DmgType
{
	Kinetic,		// hign dd structure dmg, low dd shield dmg
	Electro,			// low dd structure dmg, high dd shield dmg
	Thermo,				// low dd structure dmg, low dd shield dmg, avg dot structure dmg
	Chaotic,			// high dd all dmg, unstable(dmg: 1 ~ dmg randomized) can be converted to full dot TODO: Need further design
}

public enum SocketField
{
	Notset = 0, 		// not valid
	Multiplier,			// RED
	Throttle,			// BLUE
	Recover,			// GREEN
	Multi,				// WHITE - all usable / rare / have to be gain by other ways
}

public enum StockIO
{
	Gold = 0,
	Scrap = 1,			// for item crafting, even trade
	Core = 2,			// for perk unlocking and filling
	Bounty = 4,			// increases with kill count, leads to more hunters and cops
}

public enum CargoType
{
	Unknown = 0,
	Weapon,
	Charged,
	Mod,
	Quest,
}