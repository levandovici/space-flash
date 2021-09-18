using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

public static class SaveLoadManager
{
#if UNITY_EDITOR
    private static string _Path = Path.Combine(Application.dataPath, "Data");
#else
    private static string _Path = Path.Combine(Application.persistentDataPath, "Data");
#endif



    private static PlayerData _currentData = null;
    private static bool _isNewPlayerData = false;



    public static PlayerData CurrentData
    {
        get
        {
            if (_currentData == null)
                Load();

            return _currentData;
        }
    } 

    public static bool IsNewPlayerData => _isNewPlayerData;



    public static void Save()
    {
        File.WriteAllText(_Path, Encript(CurrentData));
    }

    public async static void SaveAsync()
    {
        using (StreamWriter writer = File.CreateText(_Path))
        {
            await writer.WriteAsync(Encript(CurrentData));
        }
    }

    public static void Load()
    {
        if(File.Exists(_Path))
        {
            _currentData = Decript(File.ReadAllText(_Path));
        }
        else
        {
            _isNewPlayerData = true;
            _currentData = new PlayerData();
        }
    }


    private static string Encript(PlayerData data)
    {
        SaveData saveData = new SaveData(data);


        return JsonUtility.ToJson(saveData);
    }

    private static PlayerData Decript(string data)
    {
        SaveData saveData = JsonUtility.FromJson<SaveData>(data);


        return saveData.PlayerData();
    }
}