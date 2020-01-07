using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!  Gun attributes. 
/*!
    This class will define the range, damage and the firing position of the bullet.
*/
public class Gun : MonoBehaviour
{
    #region Gun Atributes

    //! Transform placed in the barrel of the object. Decides the origin position of the raycast.
    public Transform gunBarrel;

    //! Range of the gun.
    public float fireLength = 100f;

    //! Damage of each bullet of the gun.
    public float damage = 25;

    //! Muzzle flash when firing.
    public ParticleSystem muzzleFlash;

    //! Gunshot audiosource when firing.
    public AudioSource gunshotAudio;

    #endregion
}
