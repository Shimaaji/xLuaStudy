using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //�Զ��ͷ�ʱ��  ��
    protected float m_ReleaseTime;

    //�ϴ��ͷ���Դ��ʱ�� ��΢�� 
    protected long m_LastReleaseTime = 0;

    //ʵ�ʶ����,�����ŵ�һ����  û������ʹ��  �Ķ���
    protected List<PoolObject> m_Objects;

    public void Start()
    {
        m_LastReleaseTime = System.DateTime.Now.Ticks;
    }

    //��ʼ�������
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects = new List<PoolObject>();
    }

    //ȡ������
    public virtual Object Spwan(string name)
    {
        foreach (PoolObject po in m_Objects)
        {
            if (po.Name == name)
            {
                m_Objects.Remove(po);
                return po.Object;
            }
        }

        return null;
    }

    //���ն���
    public virtual void UnSpwan(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);
        m_Objects.Add(po);
    }

    //�ͷŶ��󣬸����в���Ҫ���в���
    public virtual void Release()
    {

    }

    private void Update()
    {
        if (System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            m_LastReleaseTime = System.DateTime.Now.Ticks;
            Release();
        }
    }
}
