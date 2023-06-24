using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    //���е�lua�ļ�
    public List<string> LuaNames = new List<string>();
    
    //����lua�ű�����
    public Dictionary<string, byte[]> m_LuaScripts;

    public LuaEnv LuaEnv;

    public void Init()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
        m_LuaScripts = new Dictionary<string, byte[]>();
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
            EditorLoadLuaScript();
        else
#endif
            LoadLuaScript();
    }

    public void StartLua(string name)
    {
        Debug.Log(string.Format("require '{0}'", name));
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }

    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    public byte[] GetLuaScript(string name)
    {
        name = name.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(name);

        byte[] luaScript = null;
        if (!m_LuaScripts.TryGetValue(fileName, out luaScript))
            Debug.LogError("lua script is not exist:" + fileName);
        return luaScript;
    }

    void LoadLuaScript()
    {
        foreach (string name in LuaNames)
        {
            Manager.Resource.LoadLua(name, (UnityEngine.Object obj) =>
            {
                AddLuaScripts(name, (obj as TextAsset).bytes);
                if (m_LuaScripts.Count >= LuaNames.Count)
                {
                    //����lua�������ʱִ��
                    Manager.Event.Fire(10000);
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }

    public void AddLuaScripts(string assetsName, byte[] luaScript)
    {
        //��ֹ�ظ���ӣ�ʹ�ø��ǵķ�ʽ���
        m_LuaScripts[assetsName] = luaScript;
    }

#if UNITY_EDITOR
    void EditorLoadLuaScript()
    {
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < luaFiles.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScripts(PathUtil.GetUnityPath(fileName), file);
        }
        Manager.Event.Fire(10000);
    }
#endif

    //lua�ڴ����
    private void Update()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    private void OnDestroy()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}
