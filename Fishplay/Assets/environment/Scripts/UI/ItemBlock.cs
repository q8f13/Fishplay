using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemBlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public BlockType Type;
	public Image Icon;
	public Text Caption;

	private string _currentItemID = null;
	private bool _onHover = false;
	public System.Action<string, bool> OnItemHoverAction;
	public System.Action<string, ItemBlock> OnItemClickedAction;

	private void Start()
	{
		Button b = gameObject.GetComponent<Button>();
		if(b == null)
			b = gameObject.AddComponent<Button>();

		b.onClick.AddListener(
			() =>
			{
				OnPointerExitExec();
				if (OnItemClickedAction != null)
					OnItemClickedAction(_currentItemID, this);
			}
		);
	}

	public void DoUpdate(IConfig cfg)
	{
		_currentItemID = cfg == null ? null : cfg.GetID();

		if(cfg == null)
		{
			Icon.sprite = null;
			Caption.text = "";
			Type = BlockType.NotSet;
		}
		else
		{
			Icon.sprite = cfg.Icon();
			Caption.text = cfg.GetName();
			if(cfg is WeaponConfig)
				Type = BlockType.Weapon;
			else if(cfg is ModConfig)
				Type = BlockType.Mods;
			// else if(cfg is ConsumableConfig)
			// 	Type = BlockType.Consumable;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_onHover = true;
		if(OnItemHoverAction != null)
			OnItemHoverAction(_currentItemID, _onHover);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnPointerExitExec();
	}

	void OnPointerExitExec()
	{
		_onHover = false;
		if(OnItemHoverAction != null)
			OnItemHoverAction(_currentItemID, _onHover);
	}
}

public enum BlockType
{
	NotSet = 0,
	Weapon,
	Consumable,
	Mods,
}