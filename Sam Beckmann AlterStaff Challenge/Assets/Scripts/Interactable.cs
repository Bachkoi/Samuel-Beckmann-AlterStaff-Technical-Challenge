using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Establish necessary fields
    // Shared
    public bool isKey;
    

    // Key Related Fields
    public int rewardLevel;

    
    // Box/Door Related fields
    public int requiredAccessLevel; // Value needed to access the object
    public AnimationClip openClip;
    public AnimationClip closeClip;
    public bool isAnimPlaying = false; // Track this to ensure it does not spam animations.


    // As a heads up, I made the key model and the animations for the chest, but I did not make the chest.
    // Here is a link for where I got the chest from: "Treasure Chest Animation" (https://skfb.ly/o7vY7) by Matt Harris is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
