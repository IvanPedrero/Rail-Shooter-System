using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public List<Enemy> enemies;

    private bool alreadyResumedMovement = false;

    private void GetEnemies()
    {
        foreach (Transform child in this.gameObject.transform)
            enemies.Add(child.GetComponent<Enemy>());
    }

    void Start()
    {
        GetEnemies();
    }

    public void ActivateEnemies()
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.SendMessage("StartChasingPlayer");
        }
    }

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
