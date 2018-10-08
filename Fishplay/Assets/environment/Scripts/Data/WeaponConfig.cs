using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponConfig : ScriptableObject
{
	public string Name = "NoName";
	public string Description = "NoDescription";
	public DmgType[] CompatibleTypes = new DmgType[]{DmgType.Kinetic, DmgType.Thermo};
	public AmmoConfig CurrentAmmo = null;

	public float FireInterval = 0.1f;
	public float ProjectileSpeed = 120.0f;
	public int EnegryDrain = 5;

	public MissileConfig MissileSet = null;			// not null if it's a launcher

	public SocketSet[] Sockets = null;
}

