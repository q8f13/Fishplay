using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponStat : MonoBehaviour {

	public Text WeaponStatPad;

	public WeaponConfig DebugOnlyDataAsset;

	private string _dataRaw;
	private bool _visible = false;

	private void Start()
	{
	}

	void Toggle(bool on)
	{
		gameObject.SetActive(on);
	}

	public void UpdateStat(IConfig data)
	{
		if(data == null)
		{
			Toggle(false);
			return;
		}

		Toggle(true);

/* 		string class_raw = "";
		for(int i=0;i<data.CompatibleTypes.Length;i++)
		{
			if (i > 0)
				class_raw += ",";
			class_raw += data.CompatibleTypes[i].ToString();
		}

		_dataRaw = string.Format("Name: {0}\n{1}\nClass: {2}\nRate: {3}/min\nCost: {4}/shot\n"
			, data.Name
			, data.Description
			, class_raw
			, GetFireRate(data.FireInterval)
			, data.EnegryDrain); */
		
		WeaponStatPad.text = data.TextOutput();
	}

	private void Update() {
		if(!_visible)
			return;

		Vector3 mp = Input.mousePosition;
	}

	public static int GetFireRate(float fire_interval)
	{
		int fr = Mathf.FloorToInt(60.0f / fire_interval);
		return fr;
	}

	#region Test case
	[ContextMenu("DebugOnly_ShowAssetStat")]
	private void DebugOnly_ShowAssetStat()
	{
		UpdateStat(DebugOnlyDataAsset);
	}
	#endregion
}
