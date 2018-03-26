using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabMap : MonoBehaviour 
{    
    public CameraMove mCameraMove = null; /*可拖动相机*/

    private List<NetData> mCurNetDataList = new List<NetData>(); /*当前需要显示的网格数据*/
    private List<PrefabTile> mMapGameObjectList = new List<PrefabTile>();
    private Vector3 mDragStartPos = Vector3.zero; /*拖动的起始位置*/
    private Vector3 mDragMovePos = Vector3.zero; /*拖动的起始位置*/
    private float mOriginX = 0f;
    private float mOriginY = 0f;

    /*注：这里为了方便直接耦合了UI，这里应该通过delegate或者notify 通知UI刷新*/
    [SerializeField]private Text mTextPosShow = null;

    void OnDestroy() 
    {
        mTextPosShow = null;

        mCameraMove.onDragCameraStart -= HandleDragCameraStartEvent;
        mCameraMove.onDragCamera -= HandleDragCameraEvent;
        mCameraMove.onDragCameraEnd -= HandleDragCameraEndEvent;
        mCameraMove = null;

        for (int i = 0; i < mMapGameObjectList.Count; i++)
        {
            mMapGameObjectList[i].onTileUpdate -= HandleTileUpdate;
            mMapGameObjectList[i].onTileClick -= HandleTileClick;
        }
    }
        
	void Start () 
    {
        /*初始化后台网格数据*/
        NetDataManager.InitNetData(); 

        /*检查相机脚本*/
        if (mCameraMove == null)
            Debug.LogError("您忘记挂在相机脚本了，请在Inspector窗口挂载CameraMove脚本！！！！");

        /*相机回调注册*/
        mCameraMove.onDragCameraStart += HandleDragCameraStartEvent;
        mCameraMove.onDragCamera += HandleDragCameraEvent;
        mCameraMove.onDragCameraEnd += HandleDragCameraEndEvent;

        /*获取制定坐标的网格数据*/
        float fTempW = 0f;
        mOriginX = -GetRealFloat(mCameraMove.mCameraWidth / 2);
        if (Mathf.Abs(mOriginX) > Mathf.Abs(mCameraMove.mCameraWidth / 2))
            fTempW = 1.0f;

        float fTempH = 0f;
        mOriginY = -GetRealFloat(mCameraMove.mCameraHeight / 2);
        if (Mathf.Abs(mOriginY) > Mathf.Abs(mCameraMove.mCameraHeight / 2))
            fTempH = 1.0f;

        List<NetData> netDataList = NetDataManager.GetNetDataList(mOriginX, mOriginY, mCameraMove.mCameraWidth + fTempW, mCameraMove.mCameraHeight + fTempH);
        if(netDataList != null)
            mCurNetDataList.AddRange(netDataList);

        /*绘制地图*/
        DrawMap(netDataList);
	}

    /*处理屏幕问题*/
    private static float GetRealFloat(float fValue)
    {
        float f4t05Value = Mathf.Round(fValue); /*4舍5入的值*/
        if (fValue > f4t05Value)
            return f4t05Value + 1f;
        else
            return f4t05Value;
    }
     
    /*注： 这里可以采用对象池的方式处理，没必要每次都删除，另外这里也可以考虑在协成里面处理，如果是单纯数据的话，另启动一个线程处理数据也是可以的*/
    private void DrawMap(List<NetData> netDataList)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            if (obj != null)
            {
                PrefabTile prefabTile = obj.GetComponent<PrefabTile>();
                if (prefabTile != null)
                {
                    prefabTile.onTileClick -= HandleTileClick;
                    prefabTile.onTileUpdate -= HandleTileUpdate;
                }
                GameObject.Destroy(obj);
            }
        }

        mMapGameObjectList.Clear();

        for(int i = 0; i < netDataList.Count; i++)
        {
            /*注：这里在真实项目中需要用对象池做处理*/
            GameObject prefabObj = Resources.Load(netDataList[i].mTileName) as GameObject;
            if (prefabObj != null)
            {
                GameObject obj = GameObject.Instantiate(prefabObj);
                if (obj != null)
                {
                    obj.transform.parent = transform;
                    obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
                    obj.transform.position = new Vector3(netDataList[i].mX, netDataList[i].mY);
                    obj.name = "[" + netDataList[i].mX + "," + netDataList[i].mY + "]";

                    PrefabTile prefabTile = obj.GetComponent<PrefabTile>();
                    if(prefabTile != null)
                    {
                        prefabTile.SetData(netDataList[i]);
                        prefabTile.onTileClick += HandleTileClick;
                        prefabTile.onTileUpdate += HandleTileUpdate;
                        mMapGameObjectList.Add(prefabTile);
                    }
                }
            }
            else
            {
                Debug.LogError(netDataList[i].mTileName + " 不存在！！！！");
            }
        }
    }
        
    private void HandleDragCameraStartEvent(GameObject obj)
    {
        mDragStartPos = obj.transform.position;
        mDragMovePos = obj.transform.position;
    }

    private void HandleDragCameraEvent(GameObject obj)
    {
        /*注释的方案是通过偏移来计算的*/
//        Vector3 temp = obj.transform.position - mDragMovePos;
//
//        /*获取制定坐标的网格数据. 这里可以自己添加famecount 来控制帧率*/
//        mOriginX += temp.x;
//        mOriginY += temp.y;
//
//        /*整理数据*/
//        mCurNetDataList.Clear();
//        List<NetData> netDataList = NetDataManager.GetNetDataList(mOriginX, mOriginY, mCameraMove.mCameraWidth, mCameraMove.mCameraHeight);
//        if (netDataList != null)
//            mCurNetDataList.AddRange(netDataList);
//
//        DrawMap(netDataList);
//
//        /*记录上一次的位置*/
//        mDragMovePos = obj.transform.position;

        /*通过估算设置*/
        float fTempW = 0f;
        mOriginX = -GetRealFloat(mCameraMove.mCameraWidth / 2 - mCameraMove.transform.position.x);
        if (Mathf.Abs(mOriginX) > Mathf.Abs(mCameraMove.mCameraWidth / 2 - mCameraMove.transform.position.x))
            fTempW = 1.0f;

        float fTempH = 0f;
        mOriginY = -GetRealFloat(mCameraMove.mCameraHeight / 2 - mCameraMove.transform.position.y);
        if (Mathf.Abs(mOriginY) > Mathf.Abs(mCameraMove.mCameraHeight / 2 - mCameraMove.transform.position.y))
            fTempH = 1.0f;

        /*整理数据*/
        mCurNetDataList.Clear();
        List<NetData> netDataList = NetDataManager.GetNetDataList(mOriginX, mOriginY, mCameraMove.mCameraWidth + fTempW, mCameraMove.mCameraHeight + fTempH);
        if(netDataList != null)
            mCurNetDataList.AddRange(netDataList);

        DrawMap(netDataList);
    }

    private void HandleDragCameraEndEvent(GameObject obj)
    {
        // TOOD::
    }

    private void HandleTileClick (GameObject obj)
    {
        if (mTextPosShow != null)
            mTextPosShow.text = "Click "+ obj.name;
    }

    private void HandleTileUpdate(GameObject obj)
    {
        /*注释：的详细操作是，替换资源，重置创建时间。重新开始倒计时*/
        if (mTextPosShow != null)
            mTextPosShow.text += "Update "+ obj.name;
    }
}
