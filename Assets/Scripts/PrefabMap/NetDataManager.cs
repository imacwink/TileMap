using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetDataManager
{
    /*注：这里最好用Array，效率会高一些，或者用Dic存储，用_X_Y_做KEY，用NetData做Value，测试Demo为了方便用的List*/
    static List<NetData> mNetDataList = new List<NetData>();
    static int s_count = 50; 

    /*初始化数据，模拟网络数据
     *注：这里真实项目的话要用单例制作这个 Manager 或者 采用 ECS 的 System来实现；
    */
    public static void InitNetData()
    {
        for(int i = -s_count; i < 0; i++)
        {
            for(int j = -s_count; j < 0; j++)
            {
                NetData netData = new NetData();
                netData.mX = (float)i;
                netData.mY = (float)j;
                netData.mTileName = "Tile";
                netData.mCreateTime = Time.realtimeSinceStartup;

                mNetDataList.Add(netData);   
            }
        }


        for(int i = -s_count; i < 0; i++)
        {
            for(int j = 0; j < s_count; j++)
            {
                NetData netData = new NetData();
                netData.mX = (float)i;
                netData.mY = (float)j;
                netData.mTileName = "Tile";
                netData.mCreateTime = Time.realtimeSinceStartup;

                mNetDataList.Add(netData);   
            }
        }

        for(int i = 0; i < s_count; i++)
        {
            for(int j = 0; j < s_count; j++)
            {
                NetData netData = new NetData();
                netData.mX = (float)i;
                netData.mY = (float)j;
                netData.mTileName = "Tile";
                netData.mCreateTime = Time.realtimeSinceStartup;

                mNetDataList.Add(netData);   
            }
        }

        for(int i = 0; i < s_count; i++)
        {
            for(int j = -s_count; j < 0; j++)
            {
                NetData netData = new NetData();
                netData.mX = (float)i;
                netData.mY = (float)j;
                netData.mTileName = "Tile";
                netData.mCreateTime = Time.realtimeSinceStartup;

                mNetDataList.Add(netData);   
            }
        }

        /*
        for(int i = 0; i < mNetDataList.Count; i++)
        {
            Debug.Log(mNetDataList[i].mX + "|" + mNetDataList[i].mY);
        }*/
    }

    /*检查时间过期, 随意设置了一个时间阀值*/
    public static bool CheckTimeExpire(NetData data)
    {
        return (Time.realtimeSinceStartup - data.mCreateTime) > 200*1000 ? true : false;
    }

    #region 下面的接口可以看作是模拟的网络接口（这里没有模拟异步接口，直接用的同步，节省时间，当然也可以写一个虚拟网络层，通过delegate的方式模拟）
    /*查找对应范围的NetData*/
    public static List<NetData> GetNetDataList(float fOriginX, float fOriginY, float fScreenW, float fScreenH)
    {
        List<NetData> netDataList = new List<NetData>();
        for (int i = 0; i < mNetDataList.Count; i++)
        {
            if ((mNetDataList[i].mX >= fOriginX && mNetDataList[i].mX <= fOriginX + fScreenW)
                && (mNetDataList[i].mY >= fOriginY && mNetDataList[i].mY <= fOriginY + fScreenH))
            {
                netDataList.Add(mNetDataList[i]);
            }
        }
        return netDataList;
    }

    /*指定数据的更新*/
    public static NetData GetNetData(float fX, float fY)
    {
        NetData netData = new NetData();
        netData.mX = fX;
        netData.mY = fY;
        netData.mTileName = "Tile";
        netData.mCreateTime = Time.realtimeSinceStartup;
        return netData;
    }
    #endregion
}
