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
    public TextMeshProUGUI serverConfigStatusText;

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
            serverConfigStatusText.text += "\ntime: " + timeTaken.Value.GetAs<int>() + "\n";

        }

        if (playerData.TryGetValue("waterUsed", out var waterUsed))
        {
            Debug.Log($"waterUsed value: {waterUsed.Value.GetAs<int>()}");
        }
    }

    public async void SavePlayerFileToCloud(string fileName, string text)
    {
        byte[] file = System.Text.Encoding.UTF8.GetBytes(text);
        await CloudSaveService.Instance.Files.Player.SaveAsync(fileName, file);
        Debug.Log($"Saved file");
    }

    // function to make local txt file with filename, text
    public void MakeLocalFile(string fileName, string text)
    {
        string path = fileName;
        System.IO.File.WriteAllText(path, text);
        Debug.Log("File created");
    }

    public string ReadLocalFile(string fileName)
    {
        string text = System.IO.File.ReadAllText(fileName);
        Debug.Log("Text from file: " + text);
        return text;
    }

}
