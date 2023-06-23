using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //ResourceManager.LoadAsset("Assets/BuildResource/UI/Prefabs/TestUI.prefab", OnComplete);
        
        
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
