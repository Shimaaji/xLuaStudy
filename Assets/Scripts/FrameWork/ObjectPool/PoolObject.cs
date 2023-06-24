using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    public Object Object;

    public string Name;

    public System.DateTime LastUseTime;

    public PoolObject(string name, Object obj)
    {
        Name = name;
        Object = obj;
        LastUseTime = System.DateTime.Now;
    }
}
