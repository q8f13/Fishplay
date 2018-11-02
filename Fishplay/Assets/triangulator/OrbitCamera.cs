//Filename: maxCamera.cs
//
// --01-18-2010 - create temporary target, if none supplied at start

using UnityEngine;
using System.Collections;
[AddComponentMenu("FlyCamera_Control/OrbitCamera")]
public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;//控制旋转速率  相机旋转快慢
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;//y轴上视野的限度
    public int yMaxLimit = 80;
    public int zoomRate = 40;//相机向上移动速率
    public float panSpeed = 0.3f;//镜头摇动速度 鼠标中间 移动镜头
    public float zoomDampening = 5.0f;//向上移动阻尼

    private float xDeg = 0.0f;//Deg 程度
    private float yDeg = 0.0f;
    private float currentDistance;//最近距离
    private float desiredDistance;//渴望达到的距离
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 CurrentPosition;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    #region TouchRelated
    private float _distanceTouchDown;
    private float _distanceTouchCurrent;
    private float _cameraDistanceCurrent;
    private Vector3 _twoPointVectorTouchDown;
    private Vector3 _twoPointVectorTouchMoved;
    private float _currentAngleDelta;
    private float _lastAngleDegree;
    private Vector3 _lastCenterPoint;
    #endregion

    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);//虚拟目标点位置:相机位置+相机移动距离
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);//算两物体间的距离
        currentDistance = distance;
        desiredDistance = distance;
                
        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
        
        xDeg = Vector3.Angle(Vector3.right, transform.right );//算旋转角度
        yDeg = Vector3.Angle(Vector3.up, transform.up );

        #region 新添加为了限制相机高度

        CurrentPosition = target.position;

        #endregion
    }

    private void OnGUI() {
        GUI.Label(new Rect(0,0,300,60), string.Format("angle delta: {0:F1}", _lastAngleDegree));
        GUI.Label(new Rect(0,60,300,60), string.Format("2 point distance: {0:F1} / {1:F1}", _distanceTouchCurrent, _distanceTouchDown));
    }

    void MouseInput()
    {
        // 鼠标hover在ui上则无视更新
        if(UnityEngine.EventSystems.EventSystem.current == null ||  UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        #region 新添加为了限制相机高度

        //if (target.position.y <= CurrentPosition.y)
        //{
        //    target.position = new Vector3(target.position.x, Mathf.Lerp(target.position.y, CurrentPosition.y, Time.deltaTime * 10), target.position.z);
        //} 

        #endregion

        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(1) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if (Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.1f;//控制旋转速率 快慢成都
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.1f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);//锁定y轴向上下移范围
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if (Input.GetMouseButton(1))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
            target.Translate(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }

    void TouchInput()
    {
        // 鼠标hover在ui上则无视更新
        if(UnityEngine.EventSystems.EventSystem.current == null ||  UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        #region 新添加为了限制相机高度

        //if (target.position.y <= CurrentPosition.y)
        //{
        //    target.position = new Vector3(target.position.x, Mathf.Lerp(target.position.y, CurrentPosition.y, Time.deltaTime * 10), target.position.z);
        //} 

        #endregion

        if(Input.touchCount == 2)
        {
            // 缩放
            Touch[] ts = Input.touches;
            // touch begin
            if(ts[0].phase == TouchPhase.Began || ts[1].phase == TouchPhase.Began)
            {
                _distanceTouchDown = (ts[0].position - ts[1].position).sqrMagnitude;
                _cameraDistanceCurrent = currentDistance;

                _twoPointVectorTouchDown = ts[1].position - ts[0].position;
                _twoPointVectorTouchMoved = _twoPointVectorTouchDown;
                _lastAngleDegree = 0;
                _lastCenterPoint = Vector3.Lerp(ts[0].position, ts[1].position, 0.5f);
            }

            // touch end
            if(ts[0].phase == TouchPhase.Ended
                || ts[0].phase == TouchPhase.Canceled
                || ts[1].phase == TouchPhase.Canceled
                || ts[1].phase == TouchPhase.Ended)
            {
            //    _distanceTouchDown = -1.0f;
            }

            // moved
            if(ts[0].phase == TouchPhase.Moved || ts[1].phase == TouchPhase.Moved)
            {
                float distance = (ts[0].position - ts[1].position).sqrMagnitude;
                float t = distance / _distanceTouchDown - 0.5f;
                _distanceTouchCurrent = distance;
                float offset = maxDistance * (zoomRate / 100.0f);
				desiredDistance = Mathf.Lerp(_cameraDistanceCurrent + offset
                    , _cameraDistanceCurrent - offset
                    , t);

                _twoPointVectorTouchMoved = ts[1].position -ts[0].position;
            }


            // 旋转
            float angleDeg = Vector3.SignedAngle(_twoPointVectorTouchDown, _twoPointVectorTouchMoved, -Vector3.forward);
            float deltaDeg = angleDeg - _lastAngleDegree;

            Vector3 centerPoint = Vector3.Lerp(ts[0].position, ts[1].position, 0.5f);
            float delta_y_center = centerPoint.y - _lastCenterPoint.y;

            _lastAngleDegree = angleDeg;
            _currentAngleDelta = deltaDeg;

            xDeg += -deltaDeg * xSpeed* Time.deltaTime;
            yDeg += -delta_y_center * ySpeed * Time.deltaTime;

            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);//锁定y轴向上下移范围

            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            _lastCenterPoint = centerPoint;
            
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        else if(Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            target.Translate(Vector3.right * -t.deltaPosition.x * Time.deltaTime);
            target.Translate(Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * -t.deltaPosition.y * Time.deltaTime, Space.World);
        }

        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }
    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate()
    {
        if(Application.isMobilePlatform)
            TouchInput();
        else
            MouseInput();
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

