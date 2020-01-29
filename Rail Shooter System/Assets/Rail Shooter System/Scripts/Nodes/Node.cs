using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!  Node visualization within Unity's editor using the editor. 
/*!
    Running node     =   Yellow
    Shooting node    =   Red
    Normal node      =   Cyan
*/
public class Node : MonoBehaviour
{
    #region Node Attributes

    [Header("Player will run towards next node : ")]
    //! Decides if the player will run after reaching the node.
    public bool runningArea;

    [Header("Player will stop and shoot enemies :")]
    //! Decides if the player stops and start shooting after reaching the node.
    public bool shootingArea;

    [Header("Enemies if it's a shooting area :")]
    //! Enemy controller needed to activate and start attack phase for all the enemies attached to it.
    public EnemyController enemyController;

    [Header("Ignore rotation when reaching the node :")]
    //! Decides if the player will ignore rotation after reaching the node.
    public bool ignoreRotation;

    #endregion

    #region Unity Methods

    void Start(){
        FixYPosition();
    }

    /**
     * This method will draw on the editor's gizmos the spheres and the colors according the node's parameters.
     */
    private void OnDrawGizmos()
    {
        // Change color to differentiate the nodes.
        if (runningArea)
            Gizmos.color = Color.yellow;
        else if (shootingArea)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.cyan;

        // Draw an sphere on the node position.
        Gizmos.DrawSphere(this.transform.position, 1f);
    }

    #endregion

    #region Node Method

    /**
    * This method will find the player object and will copy to itself it's 'y' position.
    */
    void FixYPosition(){
        float newPos = GameObject.FindGameObjectWithTag("Player").transform.position.y;
        this.transform.position = new Vector3(this.transform.position.x, newPos, this.transform.position.z);
    }

    #endregion
}
