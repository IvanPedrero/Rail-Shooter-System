using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!  Damage controller that applies a multiplier depending on the part shooted.
public class BoneDamageReceiver : MonoBehaviour
{
    #region Public Attributes

    [Header("Enemy object to pass the damage : ")]
    //!  Enemy to apply the damage. 
    public Enemy enemy;

    [Header("Multiplier of damage depending on the part shooted : ")]
    //!  Multiplier of the damage to apply to the total damage.
    public float damageMultiplier = 1.5f;

    #endregion

    #region Damage Methods

    /**
     * This method sends the damage with the counter applied to the general's health.
     */
    public void ReceiveDamage(int damage)
    {
        enemy.ReceiveGeneralDamage(damage * damageMultiplier);
    }

    #endregion
}
