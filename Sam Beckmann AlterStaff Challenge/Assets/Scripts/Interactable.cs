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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
