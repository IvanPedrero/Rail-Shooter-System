using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    //! Enemy child list.
    public List<Enemy> enemies;

    //! Flag to check if the player has already resumed the movement.
    private bool alreadyResumedMovement = false;

    /**
     * This method will assign each enemy that is child of the enemy parent transform and assign it to the enemy child list.
     * @see enemies
     */
    private void GetEnemies()
    {
        foreach (Transform child in this.gameObject.transform)
            enemies.Add(child.GetComponent<Enemy>());
    }

    void Start()
    {
        GetEnemies();
    }

    /**
     * Sends a message to all the enemies assigned to start attacking the player.
     */
    public void ActivateEnemies()
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.SendMessage("StartChasingPlayer");
        }
    }

    /**
     * This method removes the enemy child from the enemy list.
     */
    public void RemoveChildFromList(Enemy child)
    {
        enemies.Remove(child);
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count <= 0 && !alreadyResumedMovement)
        {
            FindObjectOfType<RailMovementController>().SendMessage("ResumeMovement");
            alreadyResumedMovement = true;
        }
    }
}
