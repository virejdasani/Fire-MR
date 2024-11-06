using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class ExtinguishFire : MonoBehaviour
{
    public ParticleSystem controllerWaterParticles;
    public ParticleSystem handWaterParticles;
    public ParticleSystem fireParticles;
    public int timeToExtinguish = 500;
    public AudioSource fireExtinguishingAudio;
    ParticleSystem currentWaterParticles;
    bool soundIsPlaying;

    public bool handTrackedMode = false;

    public struct userAttributes
    {
    }

    public struct appAttributes
    {
    }

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
        // initialize Unity's authentication and core services
        await InitializeRemoteConfigAsync();

        // Add a listener to apply settings when successfully retrieved:
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
        await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());

        RemoteConfigService.Instance.GetConfig("handTrackedMode");
    }

    void ApplyRemoteConfig(ConfigResponse configResponse)
    {
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session and no local cache file exists; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:
                Debug.Log("New settings loaded this session; update values accordingly.");
                Debug.Log("handTrackedMode: " + RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode"));
                break;
        }

        handTrackedMode = RemoteConfigService.Instance.appConfig.GetBool("handTrackedMode");
    }

    // Start is called before the first frame update
    void Start()
    {
      // set the current particle system based on the handWaterParticlesOn boolean
      if (handTrackedMode) {
          currentWaterParticles = handWaterParticles;
          controllerWaterParticles.Stop();
      } else {
          currentWaterParticles = controllerWaterParticles;
          handWaterParticles.Stop();
      }

      soundIsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {

        // if the left trigger is pressed, play the water particles from the hand
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f) {
            if (!soundIsPlaying)
            {
                fireExtinguishingAudio.Play();
                soundIsPlaying = true;
            }

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

                    Debug.Log("Time to extinguish: " + timeToExtinguish);

                    if (timeToExtinguish <= 0) {
                        fireParticles.Stop();
                    }
                } else {
                    timeToExtinguish = 500;
                    Debug.Log("Time to extinguish: " + timeToExtinguish);
                }
            }
        }
    }
}