using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModConfig : IConfig
{
	public string Name = "NoName";
	public string Description = "NoDescription";
	public string ID = null;

	public SocketSet SocketSuit = null;
	private Sprite _currIcon;

	public static ModConfig CreateDummy()
	{
		ModConfig cfg = new ModConfig();
		cfg.Name = UIRigging.GenerateName(Random.Range(5,12));
		cfg.Description = "It's a weapon";
		cfg.ID = Random.Range(10000,100000).ToString();
		// List<SocketSet> list = new List<SocketSet>();
		SocketSet ss = new SocketSet();
		cfg.SocketSuit = ss;
		return cfg;
	}


	public Sprite Icon()
	{
		if(_currIcon == null)
			_currIcon = UIRigging.Instance.DummyIcon;
		return _currIcon;
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

		string data_raw = string.Format("Name: {0}\n{1}\nClass: {2}\nSocketType: {3}"
			, Name
			, Description
			, class_raw
			, (SocketField)(Random.Range(1,5))
		);
		
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

