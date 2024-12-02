using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using TMPro;
using UnityEngine.UI;

public class DashboardManager : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    CloudSaveDataManager cloudSaveDataManager;

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    async Task Awake() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        byte[] file = await CloudSaveService.Instance.Files.Player.LoadBytesAsync("virejfilee.csv");
        string text = System.Text.Encoding.UTF8.GetString(file);
        Debug.Log($"0999 Text from file from cloud: {text}");
        uiText.text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("DashboardManager Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
