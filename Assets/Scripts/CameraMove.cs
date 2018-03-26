using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMove : MonoBehaviour
{
    /*当前主相机*/
    private Camera mMainCamera = null;

    /*相机宽度高度和比例*/
    [HideInInspector]
    public float mCameraHeight = 1.0F;
    [HideInInspector]
    public float mCameraWidth = 1.0F;
    private float mCameraAspect = 1.0F;

    /*移动相机，测试代码，暂时没有添加边界*/
    private Vector2 mMomentum = Vector2.zero;  /*偏移*/
    private float mMomentumTime = 0.0F;
    private float mMomentumDistance = 0.0F;
    private bool mCanMoveMomentum = false;
    public float mMomentumAmount = .1f; /*加速度*/
    private Vector3 mPreMousePos = Vector3.zero;

    /*处理动态加载地图的delegate*/
    public enum SwipeType{ None, Left, Right, Up, Down, Other};
    public SwipeType mSwipeType = SwipeType.Other;

    public delegate void OnDragCameraStart(GameObject obj);
    public OnDragCameraStart onDragCameraStart;

    public delegate void OnDragCamera(GameObject obj);
    public OnDragCamera onDragCamera;

    public delegate void OnDragCameraEnd(GameObject obj);
    public OnDragCameraEnd onDragCameraEnd;

    void OnDestroy()
    {
        onDragCamera = null;
    }

    void Awake () 
    {
        mMainCamera = GetComponent<Camera>();
        mCameraHeight = mMainCamera.orthographicSize * 2;
        mCameraAspect = mMainCamera.aspect;
        mCameraWidth = mCameraHeight * mCameraAspect;
	}
	
    /*处理事件*/
	void Update () 
    {
        /*注:如果考虑在手机屏幕滑动，需要使用touch或者第三方插件都可以,这里只是为了简单快速实现功能故使用的Mouse*/
        if (Input.GetMouseButtonDown(0))
        {
            mMomentum = Vector2.zero;
            mMomentumTime = Time.realtimeSinceStartup;
            mCanMoveMomentum = false;
            mMomentumDistance = 0.0F;
            mPreMousePos = Input.mousePosition;

            if (onDragCameraStart != null)
                onDragCameraStart(gameObject);
        }
        else if(Input.GetMouseButton(0)) 
        {
            UserDrag(Input.mousePosition - mPreMousePos);

            mSwipeType = GetSwipe(mPreMousePos, Input.mousePosition);

            mPreMousePos = Input.mousePosition;

            if (onDragCamera != null)
                onDragCamera(gameObject);
        }
        else if(Input.GetMouseButtonUp(0)) 
        {
            mMomentumTime = Time.realtimeSinceStartup - mMomentumTime;
            mCanMoveMomentum = true;
            mPreMousePos = Input.mousePosition;

            if (onDragCameraEnd != null)
                onDragCameraEnd(gameObject);
        }
	}

    /*用户平滑滑动相机*/
    private void UserDrag(Vector2 delta)
    {
        Vector3 screenPos = mMainCamera.WorldToScreenPoint(transform.position);
        mMomentumDistance += screenPos.magnitude;
        screenPos -= new Vector3(delta.x, delta.y, 0);
        Vector3 position = mMainCamera.ScreenToWorldPoint(screenPos);
        Vector3 dstPosition = transform.position;
        dstPosition.x = position.x;
        dstPosition.y = position.y;
        transform.position = new Vector3(dstPosition.x, dstPosition.y, transform.position.z); 
        mMomentum = Vector2.Lerp(mMomentum, mMomentum + delta * (0.01f * mMomentumAmount), 0.67f);
    }

    private SwipeType GetSwipe(Vector2 start, Vector2 end)
    {
        Vector2 linear;
        linear = (end - start).normalized;

        if (Mathf.Abs(linear.y) > Mathf.Abs(linear.x))
        {
            if ( Vector2.Dot(linear, Vector2.up) >= 0.85f)
                return SwipeType.Up;

            if ( Vector2.Dot(linear, -Vector2.up) >= 0.85f)
                return SwipeType.Down;      
        }
        else
        {
            if ( Vector2.Dot(linear, Vector2.right) >= 0.85f)
                return SwipeType.Right;

            if ( Vector2.Dot(linear, -Vector2.right) >= 0.85f)
                return SwipeType.Left;
        }                   

        return SwipeType.Other;         
    }

}
