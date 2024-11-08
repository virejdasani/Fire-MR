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

    public async void SaveDataToCLoud()
    {
        var playerData = new Dictionary<string, object>{
          {"fireExtinguished", true},
          {"timeTaken", 220},
          {"waterUsed", 9999},
        };

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
}
