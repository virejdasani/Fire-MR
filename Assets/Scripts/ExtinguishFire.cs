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
    bool soundIsPlaying;

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
                if (!soundIsPlaying)
                {
                    fireExtinguishingAudio.Play();
                    soundIsPlaying = true;
                }

                currentWaterParticles.Play();

            }
            else {
                if (soundIsPlaying)
                {
                    fireExtinguishingAudio.Stop();
                    soundIsPlaying = false;
                }

                currentWaterParticles.Stop();

            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            fireExtinguishingAudio.Play();
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            fireExtinguishingAudio.Stop();
        }

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