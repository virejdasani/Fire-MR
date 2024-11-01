using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguishFire : MonoBehaviour
{
    public ParticleSystem controllerWaterParticles;
    public ParticleSystem handWaterParticles;
    public bool handTrackedMode;
    public ParticleSystem fireParticles;
    public int timeToExtinguish = 500;
    public AudioSource fireExtinguishingAudio;

    ParticleSystem currentWaterParticles;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (handTrackedMode) {
            // while the w key is held down, play the water particles
            if (Input.GetKey(KeyCode.W)) {
                currentWaterParticles.Play();
            } else {
                currentWaterParticles.Stop();
            }
        } else {
            // if waterParticles is not null and the right trigger is pressed, play the water particles
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f) {
                currentWaterParticles.Play();
            } else {
                currentWaterParticles.Stop();
            }
        }

        // while the trigger is held down, play the sound effect
        // if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f) {
        //     Debug.Log("Playing fire extinguisher sound effect");
        //     fireExtinguishingAudio.Play();
        // } else {
        //     Debug.Log("Stopping fire extinguisher sound effect");
        //     fireExtinguishingAudio.Stop();
        // }

        // check if the water particles are colliding with the fire particles for more than 3 seconds
        if (controllerWaterParticles && fireParticles) {
            if (controllerWaterParticles.IsAlive() && fireParticles.IsAlive()) {
                // get the bounds of the water particles
                Bounds waterBounds = controllerWaterParticles.GetComponent<Renderer>().bounds;

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