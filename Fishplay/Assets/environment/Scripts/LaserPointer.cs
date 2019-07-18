using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    public float Width;
    public Color LineColor;
    public Material LineMat;
    public float Distance = 100.0f;

    LineRenderer _lr;

    RaycastHit _hit;
    Ray _r;
    Camera _cam;

    private Vector3 _p;
    public bool bHit;
    public Vector3 GetPos{get{return _p;}}

    private bool _casting = false;

    private const float _intervalCooldown = 0.05f;
    private float _intervalCounter = 0.0f;

    private void Start() 
    {
        _lr = GetComponent<LineRenderer>();
        _lr.startWidth = _lr.endWidth = Width;
        _lr.sharedMaterial = LineMat;
        _cam = Camera.main;
        _lr.positionCount = 2;
    }

    public void SetMousePoint(bool on, Vector3 mousePoint)
    {
        _r = _cam.ScreenPointToRay(mousePoint);
        _casting = on && _intervalCounter <= 0;

        if(_intervalCounter <= 0)
            _intervalCounter = _intervalCooldown;
    }

    private void LateUpdate()
    {
        _lr.enabled = _casting;

        if(_intervalCounter > 0.0f)
            _intervalCounter -= Time.fixedDeltaTime;

        if(_casting)
        {
            _p = _r.origin + _r.direction * Distance;
            if(Physics.Raycast(_r, out _hit, Distance))
            {
                _p = _hit.point;
                bHit = true;
            }
            else
            {
                bHit = false;
            }
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, _p);
        }
        else
        {
            bHit = false;
        }
    }
}
