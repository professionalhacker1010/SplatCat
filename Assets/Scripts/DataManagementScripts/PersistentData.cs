using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    public static bool MenuShown { get; set; }
    public static bool PauseShown { get; set; }
    public static int CurrLevel { get; set; }

/*    public static void LoadSaveData()
    {
        PlayerData playerData = SaveSystem.LoadData();
        if (playerData == null)
        {
            CurrLevel = 1;
            SaveSystem.SaveData(CurrLevel);
        }
        else
        {
            CurrLevel = playerData.level;
        }
    }*/
}
