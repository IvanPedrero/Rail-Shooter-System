using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneDamageReceiver : MonoBehaviour
{
    #region Public Attributes

    [Header("Enemy object to pass the damage : ")]
    public Enemy enemy;

    [Header("Multiplier of damage depending on the part shooted : ")]
    public float damageMultiplier = 1.5f;

    #endregion

    #region Damage Methods

    public void ReceiveDamage(int damage)
    {
        enemy.ReceiveGeneralDamage(damage * damageMultiplier);
    }

    #endregion
}
