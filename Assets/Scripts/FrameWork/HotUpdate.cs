using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;
    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }

    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(DownFileInfo info, Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError("下载文件出错:" + info.url);
            yield break;
        }
        info.fileData = webRequest.downloadHandler;
        Complete?.Invoke(info);
        webRequest.Dispose();
    }
    /// <summary>
    /// 下载多个文件  
    /// </summary>
    /// <param name="infos"></param>
    /// <param name="Complete"></param>
    /// <param name="DownLoadAllComplete"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete,Action DownLoadAllComplete)
    {
        foreach (DownFileInfo info in infos)
        {
            yield return DownLoadFile(info, Complete);
        }
        DownLoadAllComplete?.Invoke();
    }

    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }
        return downFileInfos;
    }

    private void Start()
    {
        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstInstall()
    {
        //判断只读目录是否存在版本文件
        bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));

        //判断可读写目录是否存在版本文件
        bool isExistsReadWritePath = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isExistsReadPath && !isExistsReadWritePath;
    }

    private void ReleaseResources()
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileComplete));
    }

    private void OnDownLoadReadPathFileComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownLoadFile(fileInfos,OnReleaseFileComplete,OnReleaseAllFileComplete));
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WirteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }

    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("OnReleaseFileComplete:" + fileInfo.url);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.fileName);
        FileUtil.WirteFile(writeFile, fileInfo.fileData.data);
    }

    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourceUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        m_ServerFileListData = file.fileData.data;
        //服务器资源列表
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResourceUrl);
        //需要下载的资源
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        //遍历服务器资源列表，记录每个文件的路径，如果本地无此资源路径，那么将此资源加入下载列表
        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourceUrl, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
        }
        else
        {
            EnterGame();
        }
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WirteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo file)
    {
        Debug.Log("OnUpdateFileComplete:" + file.url);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, file.fileName);
        FileUtil.WirteFile(writeFile, file.fileData.data);
    }

    private void EnterGame()
    {
        Manager.Resource.ParseVersionFile();
        Manager.Resource.LoadUI("Login/LoginUI", OnComplete);
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
