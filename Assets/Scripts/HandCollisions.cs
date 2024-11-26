using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisions : MonoBehaviour
{
    public AudioSource fireAlarmAudio;

    bool soundIsPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("touched sum");

        if (collision.gameObject.tag == "fireAlarm")
        {
            Debug.Log("touched alarm");

            if (!soundIsPlaying)
            {
                soundIsPlaying = true;
                fireAlarmAudio.Play();
            }
            else
            {
                soundIsPlaying = false;
                fireAlarmAudio.Stop();
            }
        }
    }
}
