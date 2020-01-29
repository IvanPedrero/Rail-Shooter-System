using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

//!  Main rail controller script. 
/*!
  This controller manages the movement of the player based on the nodes configurations when the player reached them.
  It serves as an interface between the nodes and the FirstPerson script from the standard assets of Unity.
*/
public class RailMovementController : MonoBehaviour
{
    #region Player(FPS) attributes

    //! Player game object. 
    /*!
      It is used to fetch the NavMeshAgent and FirstPersonController components.
    */
    public GameObject player;

    //! AI Agent component.
    /*!
      Agent used to move the player as a rail through a baked navigation area.
    */
    private NavMeshAgent navMeshAgent;

    //! Unity Standard Assets first person controller.
    /*!
      Modified version of the FirstPersonController with disabled movement and implemented interface to work as a rail component.
    */
    private FirstPersonController firstPersonController;

    #endregion

    #region Node Attributes

    [Header("Transform containing the nodes as children :")]
    //! Parent of the nodes. 
    /*!
      Transform used to get child Transform objects which will be the nodes.
    */
    public Transform nodesParent;

    [HideInInspector]
    //! List of the nodes. 
    /*!
      Each node of the list which will be the path that the player (using nav mesh agent) will follow to move.
      The nodes will have properties used to decide the behaviour of the player when moving.
    */
    public List<Transform> nodes;

    //! Pointer to the current node in the list. 
    private int nodeIndex = 0;

    #endregion

    #region Gameplay Attributes

    [HideInInspector]
    //! Flag that decides if the player will run or walk during transition to the next node.
    public bool isWalking = true;

    [Header("Walking speed of the character :")]
    [Range(3, 6)]
    //! Walk speed in m/s.
    public float walkingSpeed = 5;

    [Header("Running speed of the character :")]
    [Range(7, 12)]
    //! Running speed in m/s.
    public float runningSpeed = 10;

    //! Rotation speed for the player when looking to another node.
    private float rotationDuration = 1f;

    #endregion

    #region Unity Methods

    /**
     * Start method will fetch the agent and fps components from the player, get the nodes from the parent and start movement of the player.
     * @see Update()
     */
    void Start()
    {
        // Fetch the player components.
        navMeshAgent = player.GetComponent<NavMeshAgent>();
        firstPersonController = player.GetComponent<FirstPersonController>();

        // Get the nodes.
        GetNodes();

        // Start moving.
        MoveToDestination();
    }

    /**
     * Will manage the speed of the player and constantly check if the player already reached the objective node.
     */
    void Update()
    {
        SetMovementSpeed();
        CheckIfNodeReached();
    }

    #endregion

    #region Node Methods

    /**
     * Iterate over all the childs of the parent transform to get each child node into a list (which will be the path).
     */
    private void GetNodes()
    {
        foreach (Transform child in nodesParent)
            nodes.Add(child);
    }

    /**
     * The method will set the next node in the list as the next target for the nav mesh agent and will set the behaviour of the gamme based on the node configurations.
     * @see CheckIfNodeReached()
     * @see RotateTowardsTarget()
     */
    private void SetNewNode()
    {
        // Set the new node as the next in the list.
        nodeIndex++;

        // Fetch the neew node.
        Node newNode = nodes[nodeIndex - 1].GetComponent<Node>();

        // Shooting area settings:
        if (newNode.shootingArea)
        {
            StopMovement();

            if (newNode.enemyController)
            {
                newNode.enemyController.ActivateEnemies();
            }
            else
            {
                Debug.LogError("You need to assign a enemy controller to this node!");
            }

        }


        // Running area settings:
        isWalking = !newNode.runningArea;

        if (newNode.runningArea)
            navMeshAgent.speed = runningSpeed;
        else
            navMeshAgent.speed = walkingSpeed;

        // Rotation settings:
        if (!newNode.ignoreRotation)
        {
            StartCoroutine(RotateTowardsTarget());
        }

    }

    /**
     * The method will check if the player already reached the current objective node and set a new one. 
     * It will also end the game if the node reached is the last of the list.
     * @see SetNewNode()
     */
    private void CheckIfNodeReached()
    {
        // Check if the agent reached it's current destination.
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // If the current node is not the last...   
                    if (nodeIndex < nodes.Count - 1)
                    {
                        // Select next node and apply the node parameters.
                        SetNewNode();

                        // Start moving to the new destination.
                        MoveToDestination();
                    }
                    else
                    {
                        // If reached the last node, end the game.
                        EndGame();
                    }

                }
            }
        }
    }

    /**
     * This method reloads the current scene when all the nodes in the list have been visited.
     * You should change this for your own custom behaviour.
     */
    public void EndGame()
    {
        // TODO: Change this for your end-game code!
        Debug.LogWarning("Change the code HERE for you own custom end-game code!");

        // This will load the current scene again.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region Movement/Rotation Methods

    /**
     * Set the destination for the nav mesh agent of the player.
     */
    private void MoveToDestination()
    {
        navMeshAgent.destination = nodes[nodeIndex].position;
    }

    /**
     * Decides the speed of the agent and fps effect (running or walking) based on the node walking configuration.
     */
    private void SetMovementSpeed()
    {
        firstPersonController.m_IsWalking = isWalking;
        if (isWalking)
            navMeshAgent.speed = walkingSpeed;
        else
            navMeshAgent.speed = runningSpeed;
    }

    /**
    * Stops the nav mesh agent.
    */
    public void StopMovement()
    {
        navMeshAgent.isStopped = true;
    }

    /**
    * Resumes the nav mesh agent.
    */
    public void ResumeMovement()
    {
        navMeshAgent.isStopped = false;
    }

    /**
    * Rotates the FirstPersonController towards the new objective node.
    */
    IEnumerator RotateTowardsTarget()
    {
        // Disables the rotation with the mouse.
        firstPersonController.canRotate = false;

        float t = 0f;

        Quaternion targetRotation = new Quaternion();

        while (t < rotationDuration)
        {
            t += Time.deltaTime;

            //Rotation factor.
            float factor = t / rotationDuration;

            // Get the rotation to the new node.
            targetRotation = Quaternion.LookRotation(nodes[nodeIndex].position - player.transform.position).normalized;

            // Smoothly rotate towards the target point.
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, factor);

            // Rotate the camera.
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.identity, factor);

            // Wait for the next frame.
            yield return null;

        }

        // Reset the rotation and mouse look of the player.
        player.transform.rotation = targetRotation;
        firstPersonController.ReInitMouseLook();

        // Allow mouse rotation
        firstPersonController.canRotate = true;

    }

    #endregion

    #region Visualization

    /**
     * Node path visualization within Unity's editor.
     */
    void OnDrawGizmos()
    {
        if(nodes.Count == 0){
            return;
        }
        foreach (Transform node in nodes)
        {
            int index = nodes.FindIndex(d => d == node);

            if (index < nodes.Count - 1)
            {
                if (node.GetComponent<Node>().runningArea)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.blue;

                Gizmos.DrawLine(node.position, nodes[index + 1].position);
            }
                
        }
    }

    #endregion
}
