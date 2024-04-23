using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // Establish necessary fields

    // Visual-Movement fields
    public Rigidbody rb; // Player's own rigidbody for collisions and positioning
    public Transform playerTransform;
    public float mouseSenseX;
    public float mouseSenseY;
    private float yawRotation;
    private float pitchRotation;
    public Camera playerCam;
    public Transform cameraLocTransform;

    // Text fields to communicate with the player
    public Text speedText;
    public Text keyText;
    public Text helpText;


    // Jump Related Fields
    public bool isGrounded;
    public float groundDrag;
    public LayerMask groundLayer;
    float playerHeight = 2f;
    float jumpTime = 0.25f;
    public float airDrag = 0.4f;
    public bool playerCanJump = true;


    // Fields related to the overall movement of the player
    float horizontalInput;
    float verticalInput;
    public float moveSpeed;
    Vector3 direction;



    public int keyLevel = 0; // This value will be storing the overall level of access the player has.


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        playerTransform = rb.GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        rb.freezeRotation = true;

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true; // Ensure the cursor is visible so players know where to click


        // Do a raycast check to see if the player is actually on the ground to determine drag and if the player can jump
        isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // Upon space input, check to see if the player is grounded and if they can jump, from there Jump and then invoke the rest function.
        if (Input.GetKeyDown(KeyCode.Space) && playerCanJump && isGrounded)
        {
            Jump();
            Invoke(nameof(ResetJump), jumpTime);
        }

        // Tracking Player Input
        if(Input.GetMouseButtonDown(0))
        {
            ClickInteractions();
        }
        LookUpdate();
        
    }


    private void FixedUpdate()
    {
        // To ensure that we are doing this the same regardless of performance.
        PlayerMovement();
        SpeedCorrection();
        speedText.text = "Speed: " + rb.velocity.magnitude;

    }
    void LookUpdate()
    {
        // Get the current value of how far the mouse hase moved from the axes multiplied against the dTime and sensitivity.
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSenseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSenseY;

        // Subtract the overall movement in the y-axis from the camera's current yaw rotation.
        // Clamp it between -45 and 45 to ensure they are not going too far.
        yawRotation -= mouseY;
        yawRotation = Mathf.Clamp(yawRotation, -45f, 45f);


        // Add the overall movement in the x-axis to the camera's current pitch rotation.
        pitchRotation += mouseX;

        // Apply the rotations we calculated back to the player, transform, and the player camera.
        playerTransform.rotation = Quaternion.Euler(yawRotation, pitchRotation, 0);
        transform.rotation = Quaternion.Euler(yawRotation, pitchRotation, 0);
        playerCam.transform.rotation = playerTransform.rotation;
    }

    void PlayerMovement()
    {

        // Get the necessary axes
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Determine the overall direction of the player's movement, but disregard the y value to ensure 
        // it is only interacted with jumping.
        direction = playerTransform.forward * verticalInput + playerTransform.right * horizontalInput;
        direction.y = 0f;

        // Check to see if the player is grounded, if so then do the normal calculation
        // If not, then apply the air drag multiplier.
        if(isGrounded)
        {
            rb.AddForce(direction.normalized * moveSpeed * 10f, ForceMode.Force);

        }
        else
        {
            rb.AddForce(direction.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);

        }

        // Update the cameras position to ensure it is never left behind.
        playerCam.transform.position = cameraLocTransform.position;


    }

    void SpeedCorrection()
    {
        // Speed Check to ensure the player does not go faster than the max speed amount.
        Vector3 vel2D = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (vel2D.magnitude > moveSpeed)
        {
            Vector3 newVelo = vel2D.normalized * moveSpeed;
            rb.velocity = new Vector3(newVelo.x, rb.velocity.y, newVelo.z);
        }
    }

    void Jump()
    {
        // Take the current velocity of the player, but disregard the y value, as that is what we are going to calculate
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add an impulse to the player by multiplying 10 against the up-vector of the player
        rb.AddForce(transform.up * 10f, ForceMode.Impulse);

        // Disable the jump variable so it can be manipulated by the reset function.
        playerCanJump = false;
    }

    // Reset functions that will allow us to invoke them to reset a variable, text, or animation.
    private void ResetJump()
    {
        playerCanJump = true;
    }

    private void ResetHelpText()
    {
        helpText.text = "";
    }

    private IEnumerator ResetChestAnimation(Animator pAnimator, float delayAmt)
    {
        yield return new WaitForSeconds(delayAmt);
        Interactable hitInteractable = pAnimator.transform.GetComponentInParent<Interactable>();
        pAnimator.Play(hitInteractable.closeClip.name);
        hitInteractable.isAnimPlaying = false;
    }

    void ClickInteractions()
    {
        // Cast a raycast from the mouses position out into the world to hit objects within 5 units
        RaycastHit hitObj;
        Ray sptrRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(sptrRay, out hitObj, 5f);

        // Ensure there is actually an object that is hit
        if(hitObj.collider != null)
        {
            // Then check to see if the hit object has the interactable script
            if (hitObj.transform.gameObject.GetComponentInParent<Interactable>() != null)
            {
                Interactable hitInteractable = hitObj.transform.GetComponentInParent<Interactable>();
                if (hitInteractable.isKey == true) // Check to see if it is a key
                {
                    // Check to ensure that the key we are attempting to pick up is actually a higher value key than the one we already have.
                    // If so, upgrade the players key value, if not then let them know they have a better key
                    if (keyLevel >= hitInteractable.rewardLevel)
                    {
                        helpText.text = "You already have a key of equal or greater value than this.";
                        Invoke(nameof(ResetHelpText), 2.5f);
                    }
                    else
                    {
                        keyLevel = hitInteractable.rewardLevel;
                        keyText.text = "Key Level: " + keyLevel;
                        helpText.text = "You already have aquired a key of " + hitInteractable.rewardLevel + " strength.";
                        Invoke(nameof(ResetHelpText), 2.5f);
                    }
                }
                else
                {
                    // With the Doors/Boxes check to ensure our key has a greater or equal strength to the requirement of the box/door.
                    // If so, the door or box can open. If not, then the door or box will let the player know they don't have a strong enough key.
                    if (keyLevel >= hitInteractable.requiredAccessLevel)
                    {
                        if(hitInteractable.isAnimPlaying == false)
                        {
                            hitInteractable.isAnimPlaying = true;
                            Animator chestAnimator = hitObj.transform.GetComponentInParent<Animator>();
                            chestAnimator.Play(hitInteractable.openClip.name);
                            StartCoroutine(ResetChestAnimation(chestAnimator, 6f));
                        }

                    }
                    else
                    {
                        helpText.text = "You do not have a strong enough key to open this. Increase your key level.";
                        Invoke(nameof(ResetHelpText), 2.5f);
                    }
                }
            }
        }
        
    }

}
