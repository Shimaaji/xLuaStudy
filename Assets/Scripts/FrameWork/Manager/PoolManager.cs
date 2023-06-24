using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //对象池中的对象的父节点
    Transform m_PoolParent;

    //对象池字典
    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        m_PoolParent = this.transform.parent.Find("Pool");
    }

    //注册对象池
    private void CreatePool<T>(string poolName, float releaseTime) where T:PoolBase
    {
        if (!m_Pools.TryGetValue(poolName, out PoolBase pool))
        {
            GameObject go = new GameObject(poolName);
            go.transform.SetParent(m_PoolParent);
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            m_Pools.Add(poolName, pool);
        }
    }

    //创建物体对象池
    public void CreateGameObjectTool(string poolName, float releaseTime)
    {
        CreatePool<GameObjectPool>(poolName, releaseTime);
    }

    //创建资源对象池
    public void CreateAssetPool(string poolName, float releaseTime)
    {
        CreatePool<AssetPool>(poolName, releaseTime);
    }

    //取出对象
    public Object Spawn(string poolName, string assetName)
    {
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
            return pool.Spwan(assetName);

        return null;
    }

    //回收对象
    public void UnSpawn(string poolName, string assetName, Object asset)
    {
        if (m_Pools.TryGetValue(poolName, out PoolBase pool))
            pool.UnSpwan(assetName, asset);
    }

}
