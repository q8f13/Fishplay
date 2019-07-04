using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : IConfig
{
	public string Name = "NoName";
	public string Description = "NoDescription";
	public DmgType[] CompatibleTypes = new DmgType[]{DmgType.Kinetic, DmgType.Thermo};
	public AmmoConfig CurrentAmmo = null;
	public string ID = null;

	public float FireInterval = 0.1f;
	public float ProjectileSpeed = 120.0f;
	public int EnegryDrain = 5;

	public MissileConfig MissileSet = null;			// not null if it's a launcher

	public SocketSet[] Sockets = null;

	public static WeaponConfig CreateDummy()
	{
		WeaponConfig cfg = new WeaponConfig();
		cfg.Name = UIRigging.GenerateName(Random.Range(5,12));
		cfg.Description = "It's a weapon";
		cfg.ID = Random.Range(10000,100000).ToString();
		int socket_count = Random.Range(0, 4);
		List<SocketSet> list = new List<SocketSet>();
		while(socket_count > 0)
		{
			list.Add(SocketSet.CreateDummy());
			socket_count--;
		}
		cfg.Sockets = list.ToArray();
		return cfg;
	}

	public Sprite Icon()
	{
		return UIRigging.Instance.DummyIcon;
	}

	public string TextOutput()
	{
		string class_raw = "";
		// for(int i=0;i<data.CompatibleTypes.Length;i++)
		// {
		// 	if (i > 0)
		// 		class_raw += ",";
		// 	class_raw += data.CompatibleTypes[i].ToString();
		// }

		string data_raw = string.Format("Name: {0}\n{1}\nClass: {2}\nRate: {3}/min\nCost: {4}/shot\n"
			, Name
			, Description
			, class_raw
			, UIWeaponStat.GetFireRate(FireInterval)
			, EnegryDrain);
		
		return data_raw;
	}

	public string GetName()
	{
		return Name;
	}

	public string GetID()
	{
		return ID;
	}
}

