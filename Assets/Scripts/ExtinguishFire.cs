using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;
using TMPro;
using Unity.Services.CloudSave;




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

    public bool handTrackedMode = true;

    public struct userAttributes { }

    public struct appAttributes { }

    // for hand squeeze tracking
    public GameObject target1;
    public GameObject target2;
    public float distanceX;
    public float distanceY;
    public float distanceZ;
    public float distanceTotal;


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

        // RemoteConfigService.Instance.GetConfig("handTrackedMode", true);
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
        if (handTrackedMode) {
            currentWaterParticles = handWaterParticles;
            controllerWaterParticles.Stop();
        } else {
            currentWaterParticles = controllerWaterParticles;
            handWaterParticles.Stop();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        controllerWaterParticles.Stop();
        handWaterParticles.Stop();

        if (handTrackedMode) {
            currentWaterParticles = handWaterParticles;
        } else {
            currentWaterParticles = controllerWaterParticles;
        }

        serverConfigStatusText.text += "\ncurrent water particles: " + currentWaterParticles;

        soundIsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {

        // for hand squeeze tracking
        Vector3 delta = target2.transform.position - target1.transform.position;
        distanceX = delta.x;
        distanceY = delta.y;
        distanceZ = delta.z;
        distanceTotal = delta.magnitude * 100;

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
        if (fireParticles.IsAlive()) {
            timeSinceFireStart += 1;
        }

        // if the left trigger is pressed, play the water particles from the hand and increment the water used and play the sound
        if ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f) || (distanceTotal < 4)) {
            if (!soundIsPlaying)
            {
                fireExtinguishingAudio.Play();
                soundIsPlaying = true;
            }

            amtWaterUsed += 1;
            currentWaterParticles.Play();
            
        } else {
            if (soundIsPlaying)
            {
                fireExtinguishingAudio.Stop();
                soundIsPlaying = false;
            }

            currentWaterParticles.Stop();
        }


        // check if the water particles are colliding with the fire particles for more than 3 seconds
        if (currentWaterParticles && fireParticles) {
            if (currentWaterParticles.IsAlive() && fireParticles.IsAlive()) {
                // get the bounds of the water particles
                Bounds waterBounds = currentWaterParticles.GetComponent<Renderer>().bounds;

                // get the bounds of the fire particles
                Bounds fireBounds = fireParticles.GetComponent<Renderer>().bounds;

                // check if the water particles are colliding with the fire particles
                if (waterBounds.Intersects(fireBounds)) {
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


                } else {
                    timeToExtinguish = 300;
                    Debug.Log("Time to extinguish: " + timeToExtinguish);
                }
            }
        }
    }
}