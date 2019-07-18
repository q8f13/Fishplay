using UnityEngine;
using UnityEngine.UI;

public class UIWarningMsgItem : MonoBehaviour , IPoolable
{
	private float _timer = 0.0f;
	private float _duration;

	private System.Action<UIWarningMsgItem> _onDisappearAction;

	[SerializeField]
	private Image _imgUp;
	[SerializeField]
	private Image _imgDown;
	[SerializeField]
	private Text _text;

	private Color _currColor;

	private void Start() 
	{
		_currColor = _imgUp.color;
	}

	public void Active()
	{
		gameObject.SetActive(true);
		_currColor.a = 1.0f;
	}

	public void Deactive()
	{
		gameObject.SetActive(false);
	}

	public void SetMsg(string msg, float duration, System.Action<UIWarningMsgItem> handler)
	{
		_duration = duration;
		_timer = _duration;
		_onDisappearAction = handler;

		_text.text = msg;
	}

	private void LateUpdate()
	{
		if(_timer <= 0.0f)
			return;

		if(_timer > 0.0f)
			_timer-= Time.deltaTime;

		float fade = Mathf.Sin(_timer);
		_currColor.a = fade;

		_imgUp.color = _currColor;
		_imgDown.color = _currColor;
		_text.color = _currColor;

		if(_timer <= 0.0f)
		{
			_onDisappearAction(this);
		}
	}
}