using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region Public Attributes

    [Header("Heath of the enemy")]
    public float health = 100;

    [Header("Distance between enemy and player to stop in meters : ")]
    public float stoppingDistance = 1f;

    [Header("Time in seconds to attack player : ")]
    public float timeToAttack = 3f;

    [Header("How much damage the enemy will deal : ")]
    public float damageDealed = 15f;

    [Header("How much score the enemy will give : ")]
    public int scoreGiven = 100;

    [Header("Stagger the enemy when damaged?")]
    public bool staggerEnemy = true;

    [Header("Time that the enemy will reamin staggered : ")]
    [Range(2, 4)]
    public float staggerTime = 2.5f;

    public enum enemyState
    {
        idle,
        walking,
        attacking,
        staggered,
        dead
    };
    [HideInInspector]
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

    // Accessed by node of the parent when reached.
    public void StartChasingPlayer()
    {
        // Set the destination of the enemy to the player.
        navMeshAgent.destination = player.transform.position;

        // Set the stopping distance of the enemy.
        navMeshAgent.stoppingDistance = stoppingDistance;

        // Start walking animation.
        state = enemyState.walking;
    }

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

    public void DealDamage()
    {
        // Send damage to the player.
        player.SendMessage("ReceiveDamage", damageDealed);
    }

    #region Damage and Dead Methods

    private void RemoveFromList()
    {
        // Remove this enemy from the list of the parent.
        this.transform.parent.SendMessage("RemoveChildFromList", this);
    }

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

    private IEnumerator Stagger()
    {
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

    #endregion
}
