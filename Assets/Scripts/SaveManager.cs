using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    const int SAVE_VERSION = 1;
    public static SaveManager instance = null;
    void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple SaveManager instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void SaveGame(int slot)
    {
        try
        {
            MemoryStream memStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memStream);

            writer.Write(SAVE_VERSION);

            // Save game state
            writer.Write(GameManager.Instance.twoPlayer);
            GameManager.Instance.gameSession.Save(writer);
            GameManager.Instance.playerDatas[0].Save(writer);
            if (GameManager.Instance.twoPlayer) GameManager.Instance.playerDatas[1].Save(writer);



            string savePath = Application.persistentDataPath + "/slot" + slot + ".dat";
            Debug.Log("Saving game to " + savePath);

            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            memStream.WriteTo(fileStream);
            fileStream.Close();


            writer.Close();
            memStream.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to save: " + e.Message);
            return;
        }
    }
    public void LoadGame(int slot)
    {
        try
        {
            string savePath = Application.persistentDataPath + "/slot" + slot + ".dat";
            Debug.Log("Loading game from " + savePath);

            MemoryStream memStream = new MemoryStream();

            FileStream fileStream = new FileStream(savePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(memStream);
            fileStream.CopyTo(memStream);
            memStream.Position = 0;

            int version = reader.ReadInt32();
            if (version == SAVE_VERSION)
            {
                GameManager.Instance.twoPlayer = reader.ReadBoolean();
                GameManager.Instance.gameSession.Load(reader);
                GameManager.Instance.playerDatas[0].Load(reader);
                if (GameManager.Instance.twoPlayer) GameManager.Instance.playerDatas[1].Load(reader);
            }
            else Debug.Log("Save version mismatch: " + version + " (expected " + SAVE_VERSION + ")");

            reader.Close();
            fileStream.Close();
            memStream.Close();

            GameManager.Instance.resumeGameFromLoad();
        }
        catch (Exception e)
        {
            Debug.Log("Failed to load save: " + e.Message);
            return;
        }
    }
    public void CopySaveToSlot(int slot)
    {
        Debug.Assert(slot > 0);

        try
        {
            string loadPath = Application.persistentDataPath + "/slot0.dat";
            string destPath = Application.persistentDataPath + "/slot" + slot + ".dat";
            File.Copy(loadPath, destPath, true);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to copy save: " + e.Message);
            return;
        }
    }
    public bool LoadExists(int slot)
    {
        string loadPath = Application.persistentDataPath + "/slot" + slot + ".dat";
        return File.Exists(loadPath);
    }

}
