using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class FacilityBase : MonoBehaviour, IFacilityTrigger
{
	private GameObject _buildingInstance = null;
	public delegate void OnFacilityActive(FacilityDataSet data);
	public OnFacilityActive OnFacilityActiveEvent;

	private FacilityDataSet _data;
	private Material _entranceMatInstance;
	private Color _debug_entrance_start_color;

	public Transform Setup(FacilityDataSet dataset, Transform targetSlot)
	{
		_data = dataset;

		_buildingInstance = Instantiate(dataset.BuildingPrefab
			, dataset.GetPosition()
			, dataset.GetRotation()
			, targetSlot);

		_buildingInstance.transform.localScale = dataset.GetScale();

		// _entranceMatInstance = this.GetEntrance.GetComponent<MeshRenderer>().material;
		_entranceMatInstance = _buildingInstance.transform.GetChild(0).GetComponent<MeshRenderer>().material;
		_debug_entrance_start_color = _entranceMatInstance.GetColor("_Color");

		return _buildingInstance.transform;
	}

	public Transform GetEntrance
	{
		get
		{
			return _buildingInstance.transform.GetChild(0);
		}
	}

	public void TriggerDispatch()
	{
		// _entranceMatInstance = GetEntrance.GetComponent<MeshRenderer>().material;
		// _entranceMatInstance.color = Color.red;
		_entranceMatInstance.SetColor("_Color", Color.red);
		StartCoroutine(PlayTriggerFx_Co(2.0f));

		// throw new System.NotImplementedException();
		if(OnFacilityActiveEvent != null)
			OnFacilityActiveEvent(_data);
	}

	IEnumerator PlayTriggerFx_Co(float delay)
	{
		yield return new WaitForSeconds(delay);
		_entranceMatInstance.color = _debug_entrance_start_color;
		// _entranceMatInstance.SetColor("_Color", _debug_entrance_start_color);
	}
}

public interface IFacilityTrigger
{
	Transform GetEntrance{get;}
	void TriggerDispatch();
}

[System.Serializable]
public class FacilityDataSet
{
	public string Name = "NoName";
	public string Description = "NoDesc";
	public GameObject BuildingPrefab;
	public FacilityType Category;

	public Vector3 GetPosition(){
		// throw new System.NotImplementedException();
		return Random.insideUnitSphere * 5.0f;
	}

	public Vector3 GetScale(){
		// throw new System.NotImplementedException();
		return Random.value * 5.0f * Vector3.one;
	}

	public Quaternion GetRotation()
	{
		// throw new System.NotImplementedException();
		return Quaternion.Euler(new Vector3(Random.Range(-15.0f, 15.0f), Random.Range(0, 180), Random.Range(-15.0f, 15.0f)));
	}
}

public enum FacilityType
{
	NotSet = 0,
	Stargate,
	POS,
}