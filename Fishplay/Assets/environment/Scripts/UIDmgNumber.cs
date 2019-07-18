using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDmgNumber : MonoBehaviour, ISpawner
{
    private UIDmgNumberText _template;

    private Pooling _pool;
    private Camera _cam;

    private void Start()
    {
        _template = transform.GetChild(0).GetComponent<UIDmgNumberText>();
        _pool = new Pooling(30, this, true);
        _cam = Camera.main;
    }

    public void ShowDmgAtPos(Vector3 worldPos, int dmg, bool crit = false)
    {
        UIDmgNumberText t = _pool.PoolOut() as UIDmgNumberText;
        if(t.OnHideAction == null)
            t.OnHideAction = (u) => _pool.PoolIn(u);

        t.SetPoint(
            worldPos
            , Vector3.ProjectOnPlane((worldPos - _cam.transform.position).normalized, _cam.transform.forward)
            , _cam
            , dmg.ToString()
            , crit);
    }

	public UIDmgNumberText SpawnNew<UIDmgNumberText>()
	{
        GameObject g = GameObject.Instantiate(_template.gameObject);
        UIDmgNumberText tt = g.GetComponent<UIDmgNumberText>();
        g.transform.SetParent(transform);
        return tt;
	}
}