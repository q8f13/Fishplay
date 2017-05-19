using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Offscreen target indicator
/// use it with a preseted prefab via ugui
/// </summary>
public class OffscreenIndicator : MonoBehaviour
{
	public bool HideInSight = true;

	private Canvas _canvas;
	private Image[] _indicators;

	private const int _indicatorCount = 10;

	private GameObject[] _targets;
	private Image[] _indicatorsOn;

	// Use this for initialization
	void Start ()
	{
		_indicators = InitIndicators(_indicatorCount);
		_indicatorsOn = new Image[_indicators.Length];
	}

	[ContextMenu("RefreshTargets")]
	public void RefreshTarget()
	{
		GetAllTargetByTag("target");
	}

	private Image[] InitIndicators(int count)
	{
		int num = count;
		Image[] indicators = new Image[num];
		num--;
		Transform slot = transform.FindChild("indicators");
		GameObject fst = slot.GetChild(0).gameObject;
		indicators[0] = fst.GetComponent<Image>();
		while (num > 0)
		{
			indicators[num] = (Instantiate(fst, slot)).GetComponent<Image>();
			indicators[num].enabled = false;
			num--;
		}

		indicators[num].enabled = false;

		return indicators;
	}

	/// UNDONE: 目前是追踪所有tag为target的gameObject
	/// 之后考虑把targetArray的动态调整补上保护措施暴露出接口
	void GetAllTargetByTag(string tag)
	{
		_targets = GameObject.FindGameObjectsWithTag(tag);
	}

	void LateUpdate()
	{
		if (_targets == null || _targets.Length == 0)
			return;

		// 遍历所有当前追踪的target
		// 让indicator与target 1-1 配对
		// 屏幕外目标不显示indicator
		for (int i = 0; i < _targets.Length; i++)
		{
			// 目标销毁后隐藏
			if (_targets[i] == null)
			{
				if (_indicatorsOn[i] != null)
				{
					_indicatorsOn[i].enabled = false;
					_indicatorsOn[i] = null;
				}
			}
			// 目标存在，根据数组下标来追踪
			else
			{
				if (_indicatorsOn[i] == null)
				{
					_indicatorsOn[i] = _indicators[i];
					_indicatorsOn[i].enabled = true;
				}

				// 先获取屏幕空间坐标
				Vector3 pos_on_screen = GetScreenPos(_targets[i].transform);

				// 如果屏幕空间坐标发现目标在相机背后，那么计算目标到近裁减面的投影
				// 用结果修正屏幕空间坐标
				if (pos_on_screen.z < 0.0f)
				{
					Vector3 target2Cam = _targets[i].transform.position - Camera.main.transform.position;
					Vector3 dir_proj = Vector3.ProjectOnPlane(target2Cam, Camera.main.transform.forward);
					pos_on_screen.x = Vector3.Dot(dir_proj, Camera.main.transform.right)*Screen.width;
					pos_on_screen.y = Vector3.Dot(dir_proj, Camera.main.transform.up)*Screen.height;
				}

				pos_on_screen.x = Mathf.Clamp(pos_on_screen.x, 0.0f, Screen.width);
				pos_on_screen.y = Mathf.Clamp(pos_on_screen.y, 0.0f, Screen.height);

				_indicatorsOn[i].GetComponent<RectTransform>().position = pos_on_screen;

				// 相机视野内则隐藏
				bool inSight = (pos_on_screen.x < Screen.width && pos_on_screen.x > 0.0f)
				               && (pos_on_screen.y < Screen.height && pos_on_screen.y > 0.0f);

				_indicatorsOn[i].enabled = !HideInSight || (HideInSight && !inSight);
			}
		}
	}

	Vector3 GetScreenPos(Transform target)
	{
		return Camera.main.WorldToScreenPoint(target.position);
	}
}
