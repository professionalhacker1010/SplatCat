using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    static int defaultLevel = 1;
    public static PlayerData SaveData(int level)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(level);

        formatter.Serialize(stream, data);
        stream.Close();

        return data;
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/player.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            if (data.level < 1 || data.level > 9)
            {
                Debug.LogWarning("Save file corrupted, creating new save file");
                return SaveData(defaultLevel);
            }

            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path + " Creating new save file");
            return SaveData(defaultLevel);
        }
    }
}
