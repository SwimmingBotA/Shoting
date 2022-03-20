using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void SaveJsonData(string saveFileName, object data)
    {
        var path = Path.Combine(Application.dataPath, saveFileName);
        try
        {
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);

#if UNITY_EDITOR
            Debug.Log($"Successfuly saved data{path}");
#endif
        }
        catch(System.Exception)
        {
            Debug.Log("have Problem");
        }
    }

    public static T LoadJsonData<T>(string loadFileName)
    {
        var path = Path.Combine(Application.dataPath, loadFileName);
        try
        {
            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }
        catch (System.Exception)
        {
            Debug.Log("No Find");
            return default;
        }
    }

    public static void DeleteFile(string fileName)
    {
        var path = Path.Combine(Application.dataPath, fileName);
        try
        {
            File.Delete(path);
        }
        catch (System.Exception)
        {
            Debug.Log("No Find");
        }
    }

    public static bool SaveFileExit(string fileName)
    {
        var path = Path.Combine(Application.dataPath, fileName);
        return File.Exists(path);
    }
}
