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

	public void DoUpdate(IConfig cfg)
	{
		_currentItemID = cfg == null ? null : cfg.GetID();

		if(cfg == null)
		{
			Icon.sprite = null;
			Caption.text = "";
		}
		else
		{
			Icon.sprite = cfg.Icon();
			Caption.text = cfg.GetName();
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