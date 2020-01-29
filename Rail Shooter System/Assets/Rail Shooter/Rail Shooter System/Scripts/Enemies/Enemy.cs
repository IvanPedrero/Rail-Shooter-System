using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region Public Attributes

    [Header("Heath of the enemy")]
    //! Initial enemy health value.
    public float health = 100;

    [Header("Distance between enemy and player to stop in meters : ")]
    //! Stoping distance in meters for the nav mesh agent of the enemy to stop.
    public float stoppingDistance = 1f;

    [Header("Time in seconds to attack player : ")]
    //! Time in seconds to attack the player after stopping.
    public float timeToAttack = 3f;

    [Header("How much damage the enemy will deal : ")]
    //! Damage to apply to the player's health for each hit.
    public float damageDealed = 15f;

    [Header("How much score the enemy will give : ")]
    //! How many points the enemy will give after killed.
    public int scoreGiven = 100;

    [Header("Stagger the enemy when damaged?")]
    //! Boolean that decides if the enemy can be staggered after landed a hit.
    public bool staggerEnemy = true;

    [Header("Time that the enemy will reamin staggered : ")]
    [Range(2, 4)]
    //! Time in seconds to be staggered before start moving again.
    public float staggerTime = 2.5f;

    //! Posible states for the enemy.
    public enum enemyState
    {
        idle,
        walking,
        attacking,
        staggered,
        dead
    };
    [HideInInspector]
    //! Variable that defines the state.
    public enemyState state = enemyState.idle;

    #endregion

    #region Private Attributes

    private float current_health = 0;

    private Animator anim;

    private NavMeshAgent navMeshAgent;

    private GameObject player;

    private float timer = 0;

    #endregion

    private void Awake()
    {
        // Fetch the necessary components.
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        // Initialize the health and time to attack.
        current_health = health;
        timer = timeToAttack;
    }

    private void Update()
    {
        // Manage each state of the enemy.
        ManageStates();
    }
    
    /**
     * This method will decide the enemy state depending on the nav mesh attributes and the current state on each frame.
     */
    private void ManageStates()
    {
        // Do not play walk animation if idle or staggered.
        if (state != enemyState.idle || state != enemyState.staggered)
        {
            anim.SetFloat("walkSpeed", navMeshAgent.velocity.magnitude);
        }

        if (state == enemyState.walking)
        {
            // If the enemy has reached the player (stopped)...
            if (navMeshAgent.velocity.magnitude <= 0)
            {
                // Start attacking.
                state = enemyState.attacking;
            }
        }
        if (state == enemyState.attacking)
        {
            // Safety meassure. When starting, the magnitude is always 0.
            if (navMeshAgent.velocity.magnitude > 0)
            {
                state = enemyState.walking;
            }

            // Start attack animation and timer.
            Attack();
        }
    }

    /**
    * This method is accessed by the parent node when player already reached it. It signals to each enemy to start attacking.
    */
    public void StartChasingPlayer()
    {
        // Set the destination of the enemy to the player.
        navMeshAgent.destination = player.transform.position;

        // Set the stopping distance of the enemy.
        navMeshAgent.stoppingDistance = stoppingDistance;

        // Start walking animation.
        state = enemyState.walking;
    }

    /**
    * Sets the animation to attack and counts 'n' time to start attacking.
    */
    private void Attack()
    {
        // Do not count time if the enemy is still attacking.
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return;
        }

        //After the previously defined time passed, attack.
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            // Play attack animation.
            anim.SetTrigger("attack");

            //Reset the timer.
            timer = timeToAttack;
        }
    }

    /**
    * Functions that deals the damage to the player.
    * @see ReceiveDamage() on HealthController.cs
    */
    public void DealDamage()
    {
        // Send damage to the player.
        player.SendMessage("ReceiveDamage", damageDealed);
    }

    #region Damage and Dead Methods

    /**
    * Remove the enemy from the parent list when killed.
    */
    private void RemoveFromList()
    {
        // Remove this enemy from the list of the parent.
        this.transform.parent.SendMessage("RemoveChildFromList", this);
    }

    /**
    * Receive damage (with modified applied) and substract it from the health. Check if the health has reached zero to set the state to 'dead'.
    */
    public void ReceiveGeneralDamage(float damage)
    {
        // Remove health.
        current_health -= damage;

        // Check if enemy already killed.
        if (current_health <= 0 && state != enemyState.dead)
        {
            // Set new state.
            state = enemyState.dead;

            // Animate the dead.
            anim.SetBool("dead", true);

            //Stop the agent from moving.
            navMeshAgent.enabled = false;

            // Update player score.
            player.SendMessage("UpdateScore", scoreGiven);

            // Remove enemy from the list of the enemy controller.
            RemoveFromList();
        }

        // Stagger enemy.
        if (staggerEnemy && state != enemyState.staggered)
            StartCoroutine(Stagger());
    }

    /**
    * This method stagger the enemy for 'n' seconds before start walking again.!
    * @see staggerTime
    */
    private IEnumerator Stagger()
    {
        if(state != enemyState.dead){

            state = enemyState.staggered;

            anim.SetTrigger("damage");

            navMeshAgent.isStopped = true;

            yield return new WaitForSeconds(staggerTime);

            if(state != enemyState.dead)
            {
                state = enemyState.walking;

                navMeshAgent.isStopped = false;
            }
        }
        else{
            yield return null;
        }
    }

    #endregion
}
