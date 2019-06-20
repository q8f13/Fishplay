using System.Collections.Generic;
using UnityEngine;

public class UIRigging : MonoBehaviour {
	public ItemBlock[] Weapons;
	public ItemBlock[] Consumables;
	public ItemBlock[] Mods;
	public List<ItemBlock> Loots;
	// public UIConsumableSocket[] Consumables;
	// public UIEmbedSocket[] Mods;

	public ItemBlock LootItemPrefab;

	[SerializeField]
	private Transform _lootContainer;

	[SerializeField]
	private UIWeaponStat _floatingPanel;
	private RectTransform _floatingPanelRt;

	private string _currentId = null;

	private Dictionary<string, IConfig> _itemRegister = new Dictionary<string, IConfig>();
	public IConfig GetItem(string id)
	{
		return _itemRegister[id];
	}

	private void Start() {
		// init blocks
		foreach(ItemBlock b in Weapons)
			b.OnItemHoverAction = ShowFloatingPanel;
		foreach(ItemBlock b in Consumables)
			b.OnItemHoverAction = ShowFloatingPanel;
		foreach(ItemBlock b in Mods)
			b.OnItemHoverAction = ShowFloatingPanel;

		// create and import debug data 
		int rnd_count = Random.Range(1,11);
		while(rnd_count > 0)
		{
			IConfig cc = null;
			cc = WeaponConfig.CreateDummy();
/* 			if(Random.value > 0.5f)
			{
			}
			else
			{
				cc = PluginConfig.CreateDummy();
			} */
			GameObject rig_unit = Instantiate(LootItemPrefab.gameObject);
			rig_unit.gameObject.SetActive(true);
			ItemBlock ib = rig_unit.GetComponent<ItemBlock>();
			ib.DoUpdate(cc);

			Loots.Add(ib);
			_itemRegister.Add(cc.GetID(), cc);
			ib.transform.SetParent(_lootContainer);

			rnd_count--;
		}

		foreach(ItemBlock b in Loots)
			b.OnItemHoverAction = ShowFloatingPanel;

		LootItemPrefab.gameObject.SetActive(false);
		_floatingPanel.UpdateStat(null);
		_floatingPanelRt = _floatingPanel.GetComponent<RectTransform>();
	}

	private void Update() {
		if(!string.IsNullOrEmpty(_currentId))
			_floatingPanelRt.anchoredPosition = Input.mousePosition;
	}

	void ShowFloatingPanel(string id, bool onHover)
	{
		if(!onHover)
		{
			_floatingPanel.UpdateStat(null);
			_currentId = null;
			return;
		}
		_currentId = id;
		if(string.IsNullOrEmpty(id))
		{
			_floatingPanel.UpdateStat(null);
		}
		else
		{
			_floatingPanel.UpdateStat(GetItem(id));
		}
	}
}

public interface IConfig {
	string GetID();
	string TextOutput();
	string GetName();
	Sprite Icon();
}
