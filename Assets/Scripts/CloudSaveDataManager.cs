using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using System.IO;


public class CloudSaveDataManager : MonoBehaviour
{

    public TextMeshProUGUI text;

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
  public async void GetPlayerFileToStringFromCloud(string fileName)
  {
      byte[] file = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(fileName);
      string text = System.Text.Encoding.UTF8.GetString(file);
      Debug.Log($"Text from file from cloud: {text}");
  }

  public void MakeLocalAndroidFile(string fileName, string text)
  {
      Debug.Log("Making local android file");
      string path = Application.persistentDataPath + "/" + fileName;
      System.IO.File.WriteAllText(path, text);
      Debug.Log("File create locally: " + path);
  }

  

    // saves data to the cloud with public read access for all players
    public async void SavePublicData(string key, string data)
    {
        var dataDict = new Dictionary<string, object> { { key, data } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(dataDict, new SaveOptions(new PublicWriteAccessClassOptions()));
        Debug.Log("Saved public data");
        text.text += "\nSaved public data" + key + " : " + data;
    }

    // loads data from the cloud with public read access for a specific player
    public async void LoadPublicDataByPlayerId()
    {
        var playerId = "gtISJtVu72LykdN6ohBZgNMf2Mfy";

        Debug.Log("Loading public data by player id" + playerId);
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "999" }, new LoadOptions(new PublicReadAccessClassOptions(playerId)));
        if (playerData.TryGetValue("999", out var keyName))
        {
            Debug.Log($"999: {keyName.Value.GetAs<string>()}");
            text.text += "By player ID 999: " + keyName.Value.GetAs<string>();
        }
    }

    string[] allPlayerIDs = new string[] {
      "SWLFCT1V5db4H8AH4cAaXYnW1r67",
      "gtISJtVu72LykdN6ohBZgNMf2Mfy",
      "jz46kXj64jRxIAyxwRooEEhBhlAl",
      "z1MkqBhkOvDwIeIMl6LQ7eLT4ONW"
      };
    
    // loop through all player ids and load public data
    public async void LoadPublicDataByAllPlayerIds(string key)
    {
        for (int i = 0; i < allPlayerIDs.Length; i++)
        {
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { key }, new LoadOptions(new PublicReadAccessClassOptions(allPlayerIDs[i])));
            if (playerData.TryGetValue(key, out var keyName))
            {
                Debug.Log($"{key}: {keyName.Value.GetAs<string>()}");
                text.text += $"\nLoaded{key}: " + keyName.Value.GetAs<string>();
            }
        }
    }

    public async void ListKeys()
    {
        var keys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync(
          new ListAllKeysOptions(new PublicReadAccessClassOptions())
        );
        for (int i = 0; i < keys.Count; i++)
        {
            Debug.Log(keys[i].Key);
        }
    }

    public async void ListKeysForAllPlayerIds()
    {
        for (int i = 0; i < allPlayerIDs.Length; i++)
        {
            var keys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync(
              new ListAllKeysOptions(new PublicReadAccessClassOptions(allPlayerIDs[i]))
            );
            for (int j = 0; j < keys.Count; j++)
            {
                Debug.Log(keys[j].Key);
            }
        }
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

      public void WriteToFile(string fileName, string contents)
    {
        Debug.Log("Writing Text to local file = " + fileName);
        string path = Application.persistentDataPath + "/" + fileName;
        File.WriteAllText(path, contents);
    }

    public string ReadFileContents(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        string text = File.ReadAllText(path);
        Debug.Log("Reading Text from local file is = " + text);
        return text;
    }

    // function takes in a fileName and pushes the contents of the file to the cloud as a string using SaveDataToCloud with key as the FILECONTENTS and value as the contents of the file
    public void SaveFileToCloudAsPlayerData(string fileName)
    {
        string text = ReadFileContents(fileName);
        SavePublicData("FILECONTENTS", text);
    }
}