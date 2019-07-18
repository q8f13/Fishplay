using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIWarningMsg : MonoBehaviour , ISpawner
{
	[SerializeField]
	private UIWarningMsgItem _warningMsgItemPrefab;

	private Pooling _warningPool;

	private void Start() 
	{
		_warningPool = new Pooling(5, this, true);
	}

	public void ShowMsg(string msg, float duration = 2.0f)
	{
		UIWarningMsgItem item = _warningPool.PoolOut() as UIWarningMsgItem;
		item.SetMsg(msg, duration, (i) => _warningPool.PoolIn(i));
	}

	public UIWarningMsgItem SpawnNew<UIWarningMsgItem>()
	{
		GameObject instance = GameObject.Instantiate(_warningMsgItemPrefab.gameObject);
		UIWarningMsgItem item = instance.GetComponent<UIWarningMsgItem>();
		instance.transform.parent = this.transform;

		return item;
	}
}