using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabTile : MonoBehaviour 
{
    public NetData mNetData = new NetData();

    public delegate void OnTileClick(GameObject obj);
    public OnTileClick onTileClick;

    public delegate void OnTileUpdate(GameObject obj);
    public OnTileUpdate onTileUpdate;

    private float mDownTime = 0f;
    private Vector3 mDownPos = Vector3.zero; 

    public TextMesh mTextMesh = null;

    public void SetData(NetData netData)
    {
        mNetData.mX = netData.mX;
        mNetData.mY = netData.mY;
        mNetData.mCreateTime = netData.mCreateTime;
        mNetData.mTileName = netData.mTileName;
        mTextMesh.text = "[" + mNetData.mX + "," + mNetData.mY +  "]";

        /*200秒后自己刷新自己.为了方便用的协成，实际项目中最好用 Invoke*/
        StopCoroutine("TimerUpdate");
        StartCoroutine (TimerUpdate(200)); /*测试的话这里可以设置的数值小一点*/
    }

    public IEnumerator TimerUpdate(float time)
    {
        yield return new WaitForSeconds (time);

        if (onTileUpdate != null)
            onTileUpdate(gameObject);
    }

    void OnDestroy()
    {
        onTileClick = null;
        mTextMesh = null;
    }

    void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hitInfo;
        if (GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            PrefabTile prefabTile = hitInfo.transform.GetComponent<PrefabTile>();
            if (prefabTile.mNetData.mX == mNetData.mX && mNetData.mY == prefabTile.mNetData.mY)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mDownTime = Time.realtimeSinceStartup;
                    mDownPos = Input.mousePosition;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    float distance = (Input.mousePosition - mDownPos).magnitude;
                    if (Time.realtimeSinceStartup - mDownTime > 0.04f && distance < 0.5f) /*点击的时候触发*/
                    {
                        if (onTileClick != null)
                            onTileClick(gameObject);
                    }
                }
            }
        }
    }
}
