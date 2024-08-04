using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Machine
{
    public interface IPersist
    {
        public void Save();
        public void Load();
    }

}

public class DataPersistenceManager : MonoBehaviour
{
    private static string genericPath => Path.Combine(Application.persistentDataPath, "Data");
    public bool loadGame, saveGame;

    private void Awake()
    {
        if (loadGame)
            LoadAll();
    }

    private void OnApplicationQuit()
    {
        if (saveGame)
            SaveAll();
    }

    public void LoadAll()
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>(true).OfType<Machine.IPersist>())
        {
            item.Load();
        }
    }

    public void SaveAll()
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>(true).OfType<Machine.IPersist>())
        {
            item.Save();
        }
    }

    public static void Save<T>(string saveString, string ID = "ID")
    {
        string classPath = Path.Combine(genericPath, typeof(T).ToString());
        string fileName = ID + ".txt";
        Directory.CreateDirectory(classPath);
        StreamWriter writer = new(Path.Combine(classPath, fileName));
        writer.Write(saveString);
        writer.Close();
    }

    /// <summary>
    /// Returns a copy of the loaded class. Do not use with MonoBehaviour scripts, use the string overload instead.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static bool TryToLoad<T>(out T result, string ID = "ID")
    {
        result = default(T);
        string classPath = Path.Combine(genericPath, typeof(T).ToString());
        string fileName = ID + ".txt";
        string finalPath = Path.Combine(classPath, fileName);
        if (!File.Exists(finalPath))
        {
            return false;
        }

        StreamReader reader = new StreamReader(finalPath);
        result = JsonUtility.FromJson<T>(reader.ReadToEnd());
        reader.Close();
        return true;
    }

    /// <summary>
    /// Tries to load a string from a filepath. This string can be passed into JsonUtility.FromJsonOverwrite to apply it to the MonoBehaviour script. Alternatively you can retrieve custom data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static bool TryToLoad<T>(out string result, string ID = "ID") where T : MonoBehaviour
    {
        result = "";
        string classPath = Path.Combine(genericPath, typeof(T).ToString());
        string fileName = ID + ".txt";
        string finalPath = Path.Combine(classPath, fileName);
        if (!File.Exists(finalPath))
        {
            return false;
        }


        StreamReader reader = new StreamReader(finalPath);

        result = reader.ReadToEnd();

        reader.Close();
        return true;
    }

#if UNITY_EDITOR
    [MenuItem("Machine/Save Data/Delete All Save Data")]
#endif
    public static void DeleteAll()
    {
        if (Directory.Exists(genericPath))
        {
            DirectoryInfo dataDir = new DirectoryInfo(genericPath);
            dataDir.Delete(true);
        }

    }
}
