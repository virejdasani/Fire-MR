using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // time to extinguish fire
    // amount of water used
    // time taken to discover fire
    // time since fire started
    // fire extinguished bool

    public TextMeshProUGUI serverConfigStatusText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getFinalScore(
        bool fireExtinguished,
        int timeToExtinguishFire,
        int waterUsed,
        int timeSinceFireStart
        // int timeTakenToDiscoverFire
    )
    {
        int score = 0;

        if (fireExtinguished)
        {
            score += 100;
        }

        if (timeToExtinguishFire < 10)
        {
            score += 100;
        }
        else if (timeToExtinguishFire < 20)
        {
            score += 50;
        }
        else if (timeToExtinguishFire < 30)
        {
            score += 25;
        }

        if (waterUsed < 10)
        {
            score += 100;
        }
        else if (waterUsed < 20)
        {
            score += 50;
        }
        else if (waterUsed < 30)
        {
            score += 25;
        }

        if (timeSinceFireStart < 10)
        {
            score += 100;
        }
        else if (timeSinceFireStart < 20)
        {
            score += 50;
        }
        else if (timeSinceFireStart < 30)
        {
            score += 25;
        }

        int newScore = timeToExtinguishFire + waterUsed + timeSinceFireStart;

        serverConfigStatusText.text += "\nFinal score: " + score + "\n";
        // time to extinguish fire goes negative - this is the amount of wasted extinguisher liquid
        serverConfigStatusText.text += "Time to extinguish fire: " + timeToExtinguishFire + "\n";
        serverConfigStatusText.text += "Water used: " + waterUsed + "\n";
        serverConfigStatusText.text += "Time since fire started " + timeSinceFireStart + "\n";
        
        // right now the lower the new score the better
        serverConfigStatusText.text += "New score: " + newScore + "\n";

    }
}
