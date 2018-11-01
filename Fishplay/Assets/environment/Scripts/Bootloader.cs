using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootloader : MonoBehaviour {

	public FacilityBase Building;

	public FacilityDataSetConfig BuildingSet;

	// Use this for initialization
	void Start () {
		Building.Setup(BuildingSet.DataSet, Building.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
