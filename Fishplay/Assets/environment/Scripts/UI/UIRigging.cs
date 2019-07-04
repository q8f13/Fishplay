using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRigging : MonoBehaviour {
	private static UIRigging _instance;
	public static UIRigging Instance{get{return _instance;}}

	[SerializeField]
	private Sprite[] DebugIcons;

	public List<ItemBlock> Weapons;
	public List<ItemBlock> Consumables;
	public List<ItemBlock> Mods;

	public List<ItemBlock> Loots;
	// public UIConsumableSocket[] Consumables;
	// public UIEmbedSocket[] Mods;

	public ItemBlock LootItemPrefab;

	[SerializeField]
	private Transform _lootContainer;

	[SerializeField]
	private Transform _weaponContainer;
	[SerializeField]
	private Transform _modContainer;
	[SerializeField]
	private Transform _consumableContainer;

	[SerializeField]
	private Transform _weaponSwitchSlot;
	[SerializeField]
	private Transform _consumableSwitchSlot;
	[SerializeField]
	private Transform _modSwitchSlot;

	[SerializeField]
	private GameObject _switchPanel; 
	[SerializeField]
	private GameObject _switchPanelMask;

	[SerializeField]
	private UIWeaponStat _floatingPanel;
	private RectTransform _floatingPanelRt;

	private string _currentId = null;

	private Dictionary<string, IConfig> _itemRegister = new Dictionary<string, IConfig>();


	public IConfig GetItem(string id)
	{
		return _itemRegister[id];
	}

	public Sprite DummyIcon{get{return DebugIcons[Random.Range(0,10)];}}

	private void Awake()
	{
		_instance = this;
	}

	private void ExtractItem(string id, ItemBlock item)
	{
		// IConfig data = GetItem(id);
		BlockType type = item.Type;
		List<ItemBlock> from = null;
		switch(type)
		{
			case BlockType.Weapon:
				from = Weapons;
				break;
			case BlockType.Consumable:
				from = Consumables;
				break;
			case BlockType.Mods:
				from = Mods;
				break;
		}
		// item.DoUpdate(data);
		from.Remove(item);
		Loots.Add(item);
		item.transform.SetParent(_lootContainer);
		item.OnItemClickedAction = TryEquipItem;

		LayoutRebuilder.ForceRebuildLayoutImmediate(_lootContainer.GetComponent<RectTransform>());
	}

	private void Start() {
		// init blocks
		foreach(ItemBlock b in Weapons)
		{
			b.OnItemHoverAction = ShowFloatingPanel;
			b.OnItemClickedAction = ExtractItem;
		}
		foreach(ItemBlock b in Consumables)
		{
			b.OnItemHoverAction = ShowFloatingPanel;
			b.OnItemClickedAction = ExtractItem;
		}
		foreach(ItemBlock b in Mods)
		{
			b.OnItemHoverAction = ShowFloatingPanel;
			b.OnItemClickedAction = ExtractItem;
		}

		// create and import debug data 
		int rnd_count = Random.Range(6,20);
		while(rnd_count > 0)
		{
			IConfig cc = null;
			if(Random.value > 0.5f)
				cc = WeaponConfig.CreateDummy();
			else
				cc = ModConfig.CreateDummy();
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
		{
			b.OnItemHoverAction = ShowFloatingPanel;
			b.OnItemClickedAction = TryEquipItem;
		}

		LootItemPrefab.gameObject.SetActive(false);
		_floatingPanel.UpdateStat(null);
		_floatingPanelRt = _floatingPanel.GetComponent<RectTransform>();

		_switchPanelMask.AddComponent<Button>().onClick.AddListener(()=>
		{
			_switchPanel.SetActive(false);
		});
	}

	private void TryEquipItem(string id, ItemBlock item)
	{
		// IConfig data = GetItem(id);
		List<ItemBlock> tar_arr = null;
		int count_limit = -1;
		Transform slot = null;
		Transform switcher_slot = null;
		switch(item.Type)
		{
			case BlockType.Weapon:
				tar_arr = Weapons;
				count_limit = 2;
				slot = _weaponContainer;
				switcher_slot = _weaponSwitchSlot;
				break;
			case BlockType.Mods:
				tar_arr = Mods;
				count_limit = 12;
				slot = _modContainer;
				switcher_slot = _modSwitchSlot;
				break;
			case BlockType.Consumable:
				tar_arr = Consumables;
				count_limit = 4;
				slot = _consumableContainer;
				switcher_slot = _consumableSwitchSlot;
				break;
			default:
				throw new System.Exception(string.Format("invalid item type: {0}",item.Type));
		}

		// equip on directly
		if(tar_arr.Count < count_limit)
		{
			// item.DoUpdate(data);
			tar_arr.Add(item);
			Loots.Remove(item);
			item.transform.SetParent(slot);
			item.OnItemClickedAction = ExtractItem;

			LayoutRebuilder.ForceRebuildLayoutImmediate(slot.GetComponent<RectTransform>());
		}
		// show item switch panel
		else
		{
			GenerateContainerSnapshoto(item, tar_arr, slot, switcher_slot);
			Debug.Log("show switch panel");
		}
	}

	private void Update() {
		if(!string.IsNullOrEmpty(_currentId))
			_floatingPanelRt.anchoredPosition = Input.mousePosition;
	}

	public static void ClearAllChilds(Transform t)
	{
		int failsafe = 999;
		while(t.childCount > 0)
		{
			Transform child = t.GetChild(0);
			child.SetParent(null);
			Destroy(child.gameObject);
			failsafe--;
			if(failsafe < 0)
			{
				throw new System.Exception("failsafe triggered");
			}
		}
	}

	public static Button ButtonComponent(GameObject g)
	{
		Button b = g.GetComponent<Button>();
		if(b == null)
			b = g.AddComponent<Button>();
		return b;
	}

	private void ShowSwitchSubPanel(Transform targetSlot)
	{
		_weaponSwitchSlot.parent.gameObject.SetActive(_weaponSwitchSlot == targetSlot);
		_modSwitchSlot.parent.gameObject.SetActive(_modSwitchSlot == targetSlot);
		_consumableSwitchSlot.parent.gameObject.SetActive(_consumableSwitchSlot == targetSlot);
	}

	private void GenerateContainerSnapshoto(ItemBlock candidate, List<ItemBlock> blocks, Transform blocks_slot, Transform switcher_slot)
	{
		_switchPanel.gameObject.SetActive(true);
		ShowSwitchSubPanel(switcher_slot);
		ClearAllChilds(switcher_slot);
		foreach(ItemBlock block in blocks)
		{
			GameObject snapshot = Instantiate(block.gameObject);
			snapshot.transform.SetParent(switcher_slot);
			ItemBlock ib = snapshot.GetComponent<ItemBlock>();
			ib.OnItemClickedAction = (id,item) => 
			{
				blocks.Remove(block);
				Loots.Add(block);
				blocks.Add(candidate);
				Loots.Remove(candidate);
				block.transform.SetParent(_lootContainer);
				candidate.transform.SetParent(blocks_slot);
				switcher_slot.parent.gameObject.SetActive(false);
				_switchPanel.gameObject.SetActive(false);
				block.OnItemClickedAction = TryEquipItem;
				candidate.OnItemClickedAction = ExtractItem;
				LayoutRebuilder.ForceRebuildLayoutImmediate(_lootContainer.GetComponent<RectTransform>());
				LayoutRebuilder.ForceRebuildLayoutImmediate(blocks_slot.GetComponent<RectTransform>());
			};
		}
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

    public static string GenerateName(int len)
    { 
        System.Random r = new System.Random();
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }
        return Name;
    }
}

public interface IConfig {
	string GetID();
	string TextOutput();
	string GetName();
	Sprite Icon();
}
