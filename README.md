# Fire-MR: Mixed-Reality Fire Safety Training Simulation

![trainerPanelImage](https://github.com/user-attachments/assets/824d8ca2-01ff-4ecf-b2df-1ed2df764340)


Running the app on a Quest

- The app identifier is called ARFTPT (stands for augmented reality fire training with passthrough)
- Open this app and you will see a fire in front of you in the world space
- Put both the controllers down (can be on the floor next to your feet or on a table/chair) where they are in range of the headset but not being used in hand
- Now there will be a gray overlay hand model on your left and right hand - this means hand tracking is working, and the controllers aren't being detected (this is good)
- User should put RIGHT HAND in a fist (and can be given to hold the nozzle of he fire extinguisher)
- As the user puts the left hand in a fist, the water will start to shoot out from the right fist.

(FYI: the way this works is I have put virtual trackers on the left palm and left middle finger. When the distance between these 2 is lesser than x value, I make it detect a fist made - and this shoots the water)

- Fire can be extinguished by aiming water at its base for a few seconds (works like you'd expect)

- The "B" button on the right controller can be pressed to spawn a working fire alarm. This will spawn exactly at the tip of your right controller
- Fire alarms can be spawned as many times as we want
- They work by (like you'd expect) just hitting them with your hand or with controllers

- The right controller joystick can be pressed down to spawn a fire at the tip of the controller (note: if you want it to spawn on the ground level, you will have to bend down and touch the right controller to the ground as you press the joystick. This is intentional so you can spawn fires at non-ground levels like on top of a table or microwave or something)

- Have only 1 active fire at any time in the scene. (having more tanks frame rate)

In a demo with multiple people trying the headset, it is easier to just restart the app before handing the headset to the next person instead of going and physically putting down a fire somewhere each time it is extinguished - but either works.

Data collection

- If the quest was connected to wifi/hotspot when the fire training was done, it will automatically send data to the cloud with insights like the time taken to extinguish fire, amount of water used, and we will add more - I have made the workflow easy to add more data to.
- This data goes to this website (we will change the domain in the future): virejfypfiretraining.netlify.app
- If you are demoing the app with the website, it's a good idea to first go to the website and where it says `Click here to set the current trainee name before starting the training`, ask the user for their name and enter it there. This is not required to demo the website because I have put enough real and dummy data to give anyone a gist of what its capable of doing.
