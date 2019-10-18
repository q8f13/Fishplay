using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSets : MonoBehaviour
{
    [SerializeField]
    private TextAsset _weapon;

    // Start is called before the first frame update
    void Start()
    {
        GeneratorDataRaw[] raw = JsonHelper.getJsonArray<GeneratorDataRaw>(_weapon.text);
        GeneratorData[] data = new GeneratorData[raw.Length];
        for(int i=0;i<data.Length;i++)
        {
            data[i] = GeneratorData.Parse(raw[i]);
        }
        Debug.Log("parse done");
    }

    [ContextMenu("RandomGenGear")]
    void RandomGen()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
