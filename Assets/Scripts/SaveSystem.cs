using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    //Basic character info and location
    public int health;
    public float mana;
    public Vector3 currentLocation;
    public string currentScene;

    //Unlocked/locked abilities
    public bool canJump;
    public bool canDoubleJump;
    public bool canDash;
    public bool canHeal;
}

public static class SaveSystem
{
    public static string filePath = Application.persistentDataPath + "/playerData.json";

    public static void SaveData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("playerData.json saved at " + filePath);
    }

    public static PlayerData LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found");
            return null;
        }
    }
}
