using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    #region Attributes

    public float health = 100f;
    private float current_health;

    [Header("Text to show health :")]
    public Text healthText;

    #endregion

    #region Unity Methods

    void Start()
    {
        current_health = health;
        UpdateHealthText();
    }

    #endregion

    #region Health Methods

    private void UpdateHealthText()
    {
        healthText.text = "HEALTH: " + current_health;
    }

    public void ReceiveDamage(float damage)
    {
        current_health -= damage;
        
        if(current_health <= 0){
            FindObjectOfType<RailMovementController>().EndGame();
        }
        else{
            UpdateHealthText();
        }
    }

    #endregion
}
