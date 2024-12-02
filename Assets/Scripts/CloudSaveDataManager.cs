using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;


public class CloudSaveDataManager : MonoBehaviour
{

    public async void SaveDataToCLoud(Dictionary<string, object> playerData)
    {
        var result = await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }

    public async void LoadDataFromCloud()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            "fireExtinguished",
            "timeTaken",
            "waterUsed",
        });

        if (playerData.TryGetValue("fireExtinguished", out var fireExtinguished))
        {
            Debug.Log($"fireExtinguished value: {fireExtinguished.Value.GetAs<string>()}");
        }

        if (playerData.TryGetValue("timeTaken", out var timeTaken))
        {
            Debug.Log($"timeTaken value: {timeTaken.Value.GetAs<int>()}");
        }

        if (playerData.TryGetValue("waterUsed", out var waterUsed))
        {
            Debug.Log($"waterUsed value: {waterUsed.Value.GetAs<int>()}");
        }
    }


  // https://docs.unity.com/ugs/manual/cloud-save/manual/tutorials/unity-sdk#Player_Files
  public async void SavePlayerFileToCloud(string fileName, string text)
  {
      byte[] file = System.Text.Encoding.UTF8.GetBytes(text);
      await CloudSaveService.Instance.Files.Player.SaveAsync(fileName, file);
      Debug.Log($"Saved file");
  }


  // this breaks the app
  // public string GetPlayerFileToStringFromCloud(string fileName)
  // {
  //     byte[] file = CloudSaveService.Instance.Files.Player.LoadBytesAsync(fileName).Result;
  //     string text = System.Text.Encoding.UTF8.GetString(file);
  //     Debug.Log($"Text from file from cloud: {text}");

  //     // return the text
  //     return text;

  // }

  // this works but doesnt return the text, only logs it. This code is copied in ExtinguishFire.cs
  // public async void GetPlayerFileToStringFromCloud(string fileName)
  // {
  //     byte[] file = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(fileName);
  //     string text = System.Text.Encoding.UTF8.GetString(file);
  //     Debug.Log($"Text from file from cloud: {text}");
  // }

  public void MakeLocalAndroidFile(string fileName, string text)
  {
      Debug.Log("Making local android file");
      string path = Application.persistentDataPath + "/" + fileName;
      System.IO.File.WriteAllText(path, text);
      Debug.Log("File create locally: " + path);
  }

  public string GetLocalAndroidFileToString(string fileName)
  {
      Debug.Log("Reading local android file");
      string path = Application.persistentDataPath + "/" + fileName;
      string text = System.IO.File.ReadAllText(path);
      Debug.Log("Reading Text from local file: " + text);
      return text;
  }

  // function to make local txt file with filename, text
  public void MakeLocalDesktopFile(string fileName, string text)
  {
      string path = fileName;
      System.IO.File.WriteAllText(path, text);
      Debug.Log("File create locally: " + path);

  }

  public string GetLocalDesktopFileToString(string fileName)
  {
      string text = System.IO.File.ReadAllText(fileName);
      Debug.Log("Reading Text from local file: " + text);
      return text;
  }
}