using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public class FileUtil
{
    //验证文件是否存在
    public static bool IsExists(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

    public static void WirteFile(string path, byte[] data)
    {
        //获取标准路径
        path = PathUtil.GetStandardPath(path);
        //获取文件夹路径
        string dir = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            file.Delete();
        }
        try
        {
            using (FileStream fs = new FileStream(path,FileMode.Create,FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch (IOException e)
        {

            Debug.LogError(e.Message);
        }
    }
}
