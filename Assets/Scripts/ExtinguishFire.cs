using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using TMPro;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public class ExtinguishFire : MonoBehaviour
{
    CloudSaveDataManager cloudSaveDataManager;

    public GameObject firePrefab;

    public ParticleSystem controllerWaterParticles;
    public ParticleSystem handWaterParticles;
    public ParticleSystem fireParticles;
    public int timeToExtinguish = 400;
    public AudioSource fireExtinguishingAudio;
    ParticleSystem currentWaterParticles;
    bool soundIsPlaying;
    int amtWaterUsed = 0;
    int timeSinceFireStart = 0;

    public TextMeshProUGUI serverConfigStatusText;

    public bool handTrackedMode = true; // false means controller trigger

    public bool leftHandSqueezeTrigger = true; // false means left hand squeeze wont trigger water particles

    public struct userAttributes { }

    public struct appAttributes { }

    // for hand squeeze tracking
    public GameObject palmCenterCollider;
    public GameObject middleFingerCollider;
    float distanceX;
    float distanceY;
    float distanceZ;
    public float distanceBetweenFingerAndPalm;

    private TcpListener tcpListener;

    async Task InitializeRemoteConfigAsync()
    {
        // initialize handlers for unity game services
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    async Task Awake()
    {
        cloudSaveDataManager = GameObject.FindGameObjectWithTag("RHController").GetComponent<CloudSaveDataManager>();

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        cloudSaveDataManager.LoadDataFromCloud();
        cloudSaveDataManager.SavePlayerFileToCloud("virejfilee.csv", "test,1,3,test");

        // local file testing
        // cloudSaveDataManager.MakeLocalFile("filee.csv", "VIREJV,IRE,J");
        // Debug.Log(cloudSaveDataManager.ReadLocalFile("filee.csv"));

        // initialize Unity's authentication and core services
        await InitializeRemoteConfigAsync();

        // Add a listener to apply settings when successfully retrieved:
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());

        // Start the TCP server
        StartServer();
    }

    void ApplyRemoteConfig(ConfigResponse configResponse)
    {
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session and no local cache file exists; using default values.");
                serverConfigStatusText.text += "No settings loaded this session and no local cache file exists; using default values.";
                serverConfigStatusText.text += "\nhandTrackedMode: " + RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                serverConfigStatusText.text += "No settings loaded this session; using cached values from a previous session.";
                serverConfigStatusText.text += "\nhandTrackedMode: " + RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("New settings loaded this session; updated values accordingly.");
                Debug.Log("handTrackedMode: " + RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode"));
                serverConfigStatusText.text += "New settings loaded this session; update values accordingly.";
                serverConfigStatusText.text += "\nhandTrackedMode: " + RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode");
                break;
        }

        // TODO: set default val for when quest not on wifi
        handTrackedMode = RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode", true);

        // set the current particle system based on the handWaterParticlesOn boolean
        if (handTrackedMode)
        {
            currentWaterParticles = handWaterParticles;
            controllerWaterParticles.Stop();
        }
        else
        {
            currentWaterParticles = controllerWaterParticles;
            handWaterParticles.Stop();
        }
    }

    private string GetLocalIPv4Address()
    {
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicastIPAddressInformation.Address.ToString();
                    }
                }
            }
        }
        return "No network adapters with an IPv4 address in the system!";
    }

    // Start is called before the first frame update
    void Start()
    {
        controllerWaterParticles.Stop();
        handWaterParticles.Stop();

        if (handTrackedMode)
        {
            currentWaterParticles = handWaterParticles;
        }
        else
        {
            currentWaterParticles = controllerWaterParticles;
        }

        soundIsPlaying = false;

        string localIPAddress = GetLocalIPv4Address();
        Debug.Log("Local IPv4 Address: " + localIPAddress);
        serverConfigStatusText.text += "\nLocal IPv4 Address: " + localIPAddress;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandSqueezeTrigger) {
            // for hand squeeze tracking
            Vector3 delta = middleFingerCollider.transform.position - palmCenterCollider.transform.position;
            distanceX = delta.x;
            distanceY = delta.y;
            distanceZ = delta.z;
            distanceBetweenFingerAndPalm = delta.magnitude * 100;
        }

        // if the left trigger is pressed, play the water particles from the hand and increment the water used and play the sound
        if ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f)||
            (leftHandSqueezeTrigger && (distanceBetweenFingerAndPalm < 4)))
        {
            if (!soundIsPlaying)
            {
                fireExtinguishingAudio.Play();
                soundIsPlaying = true;
            }

            amtWaterUsed += 1;
            currentWaterParticles.Play();

        }
        else
        {
            if (soundIsPlaying)
            {
                fireExtinguishingAudio.Stop();
                soundIsPlaying = false;
            }

            currentWaterParticles.Stop();
        }

        // ifthe right trigger is pressed, instantiate the fire particles at the hand position (y value set to 0)
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            // get the transform position in a varable and set the y value to 0
            Vector3 spawnPos = transform.position;
            spawnPos.y = 0;

            GameObject spawnedFire = Instantiate(firePrefab, spawnPos, Quaternion.identity);

            // set the fire particles to the instantiated fire particles child
            fireParticles = spawnedFire.transform.GetChild(0).GetComponent<ParticleSystem>();
        }

        // if the fire particles are alive, increment the time since fire start
        if (fireParticles.IsAlive())
        {
            timeSinceFireStart += 1;
        }

        // check if the water particles are colliding with the fire particles for more than 3 seconds
        if (currentWaterParticles && fireParticles)
        {
            if (currentWaterParticles.IsAlive() && fireParticles.IsAlive())
            {
                // get the bounds of the water particles
                Bounds waterBounds = currentWaterParticles.GetComponent<Renderer>().bounds;

                // get the bounds of the fire particles
                Bounds fireBounds = fireParticles.GetComponent<Renderer>().bounds;

                // check if the water particles are colliding with the fire particles
                if (waterBounds.Intersects(fireBounds))
                {
                    timeToExtinguish -= 1;
                    serverConfigStatusText.text = "Time to extinguish: " + timeToExtinguish;
                    serverConfigStatusText.text += "\nTime since fire start: " + timeSinceFireStart;
                    serverConfigStatusText.text += "\nWater used: " + amtWaterUsed;

                    Debug.Log("Time to extinguish: " + timeToExtinguish);

                    if (timeToExtinguish <= 0)
                    {
                        fireParticles.Stop();

                        var playerData = new Dictionary<string, object>{
                            {"fireExtinguished", true},
                            {"timeTaken", timeSinceFireStart},
                            {"waterUsed", amtWaterUsed},
                        };

                        cloudSaveDataManager.SaveDataToCLoud(playerData);
                    }

                }
                else
                {
                    timeToExtinguish = 300;
                    Debug.Log("Time to extinguish: " + timeToExtinguish);
                }
            }
        }
    }

    private async void StartServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, 41196);
        tcpListener.Start();
        Debug.Log("Server started.");
        serverConfigStatusText.text += "\nServer started!!";


        while (true)
        {
            TcpClient client = await tcpListener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);  // Fire and forget
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[2048];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;  // Client disconnected

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + message);
                serverConfigStatusText.text = "\nReceived: " + message;

                if (int.TryParse(message, out int value))
                {
                    if (value < 50000)
                    {
                        // todo: try uncommenting these
                        //if (!currentWaterParticles.isPlaying)
                        //{
                            currentWaterParticles.Play();
                            Debug.Log("Particles started.");
                            serverConfigStatusText.text += "\nParticles started.";
                        //}
                    }
                    else
                    {
                        //if (currentWaterParticles.isPlaying)
                        //{
                            currentWaterParticles.Stop();
                            Debug.Log("Particles stopped.");
                            serverConfigStatusText.text += "\nParticles stopped.";
                        //}
                    }
                }
                else
                {
                    Debug.LogError("Received invalid number: " + message);
                    serverConfigStatusText.text += "\nReceived invalid number: " + message;
                }

                byte[] response = Encoding.UTF8.GetBytes("Echo: " + message);
                await stream.WriteAsync(response, 0, response.Length);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
            serverConfigStatusText.text += "Exception: " + ex.Message;
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }

    private void OnApplicationQuit()
    {
        tcpListener.Stop();
        Debug.Log("Server stopped.");
        serverConfigStatusText.text += "server stopped";

    }
}
