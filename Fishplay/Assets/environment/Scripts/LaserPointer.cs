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

    private bool _casting = false;

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
        _casting = on;
    }

    private void LateUpdate()
    {
        _lr.enabled = _casting;

        if(_casting)
        {
            Vector3 p = _r.origin + _r.direction * Distance;
            if(Physics.Raycast(_r, out _hit, Distance))
            {
                p = _hit.point;
            }
            _lr.SetPosition(0, transform.position);
            _lr.SetPosition(1, p);
        }
    }
}
