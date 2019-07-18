using UnityEngine;

public class UIDmgNumberText : UnityEngine.UI.Text,  IPoolable
{
    private RectTransform  _rt;
    private float _timer = 0.0f;
    private bool _timerOn = false;
    private const float DURATION = 1.0f;

    public System.Action<UIDmgNumberText> OnHideAction;

    private Vector3 _startWorldPos;
    private Vector3 _currPos;
    private Vector3 _worldDir;
    private Camera _cam;

    private void Awake() {
        if(_rt == null)
            _rt = GetComponent<RectTransform>();

        if(_cam == null)
            _cam = Camera.main;
    }

	public void Active()
	{
        this.enabled = true;
        _timer = 0.0f;
        _timerOn = true;
	}

    private void LateUpdate()
    {
        _currPos += (_worldDir + _cam.transform.up) * Time.deltaTime * 10;

        _rt.anchoredPosition = _cam.WorldToScreenPoint(_currPos);

        if(_timerOn)
        {
            _timer += Time.deltaTime;
            if(_timer > DURATION)
            {
                if(OnHideAction != null)
                    OnHideAction(this);
            }
        }
    }

	public void Deactive()
	{
        _timerOn = false;
        this.enabled = false;
	}

    public void SetPoint(Vector3 worldPos, Vector3 world_dir, string s, bool crit)
    {
        this.text = s;
        _startWorldPos = worldPos;
        _currPos = _startWorldPos;
        _worldDir = world_dir;
    }
}
