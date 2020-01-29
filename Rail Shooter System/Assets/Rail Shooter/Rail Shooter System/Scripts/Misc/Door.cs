using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!  Door opening controller. 
/*!
  Door script which detects the player and opens.
  It can be used to only open when reached or it can stop the player movement and resume it after opening.
*/
public class Door : MonoBehaviour
{
    #region Public Attributes

    [Header("Door objects : ")]
    public GameObject rightDoor;
    public GameObject leftDoor;

    [Header("Movement speed : ")]
    [Range(0.05f, 0.2f)]
    //! Speed factor that decides the speed of the gate when opening.
    public float speed;

    [Header("Stop player movement (useful for shooting areas) : ")]
    //! Decides if the player will stop after reaching the door.
    public bool stopPlayerMovement = true;

    #endregion

    #region Private Attributes

    private float startTime;
    //! Factor needed to calculate the distance of the door to the sliding objective.
    private float distanceLength;
    //! The right door will slide to the right side to open, this is the reference to said position.
    private Vector3 rightDoorObjective;
    //! The left door will slide to the left side to open, this is the reference to said position.
    private Vector3 leftDoorObjective;
    private bool openDoor;
    //! Object needed to stop/resume player's movement.
    private RailMovementController railMovementController;

    #endregion

    #region Unity Methods

    //! Used to get the rail main controller.
    void Start()
    {
        // Fetch the rail controller.
        railMovementController = FindObjectOfType<RailMovementController>();    
    }

    //! If the player reaches the door, this method will start the door movement and resume player's movement when opened (if enabled).
    void Update()
    {
        // If the player reached the door...
        if (openDoor)
        {
            // Distance covered.
            float distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed.
            float fractionOfJourney = distCovered / distanceLength;

            // Update door positions.
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, rightDoorObjective, fractionOfJourney);
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, leftDoorObjective, fractionOfJourney);

            // If doors have already reached an acceptable position...
            if(rightDoor.transform.localPosition.x >= rightDoorObjective.x - 0.15f)
            {
                // If stopped previously...
                if (stopPlayerMovement)
                    // Advance.
                    railMovementController.ResumeMovement();

                // Stop the loop.
                openDoor = false;
            }
        }
    }

    //! Check if the player reached the door trough a trigger in the collider.
    private void OnTriggerEnter(Collider other)
    {
        // If the player reached the door..
        if(other.tag == "Player")
        {
            StartCoroutine(DelayOpening());
        }
    }

    #endregion

    #region Door Methods

    /**
     * This method will initialize the timer, the door sliding objectives and the lenght between the doors and objectives.
     * @see DelayOpening()
     */
    private void SetDoorMovement()
    {
        // Track the starting time.
        startTime = Time.time;

        // Set the dessired positions of the doors (that wil ONLY move in the x axis). 
        rightDoorObjective = new Vector3(rightDoor.transform.localPosition.x + 2, rightDoor.transform.localPosition.y, rightDoor.transform.localPosition.z);
        leftDoorObjective = new Vector3(leftDoor.transform.localPosition.x - 2, leftDoor.transform.localPosition.y, leftDoor.transform.localPosition.z);

        // Calculate the journey length.
        distanceLength = Vector3.Distance(rightDoor.transform.localPosition, rightDoorObjective);
    }

    /**
     * This IEnumeraator will stop the movement, set the initial values of each door and start the opening.
     * It will wait for 1 second before opening the door.
     * @see OnTriggerEnter()
     * @see SetDoorMovement()
     */
    private IEnumerator DelayOpening()
    {
        // Stop the movement.
        if(stopPlayerMovement)
            railMovementController.StopMovement();

        // Set initial values.
        SetDoorMovement();

        // Small delay of one second.
        yield return new WaitForSeconds(1f);

        // Start opening the door.
        openDoor = true;
    }

    #endregion

}
