using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AmmoConfig : ScriptableObject
{
	public DmgType DamageType = DmgType.Kinetic;
	public int DmgVal = 1;
	public int Count;
}
