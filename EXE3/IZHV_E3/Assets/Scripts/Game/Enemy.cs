using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

/// <summary>
/// Main Enemy behavior script.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Current health of the Enemy.
    /// </summary>
    [ Header("Gameplay") ]
    public float health = 10.0f;

    /// <summary>
    /// Current speed of the Enemy.
    /// </summary>
    public float speed = 1.0f;
    
    /// <summary>
    /// Rigid body of the Enemy.
    /// </summary>
    private Rigidbody mRigidBody;
    
    /// <summary>
    /// Trigger of the Enemy.
    /// </summary>
    private BoxCollider mBoxTrigger;
    
    /// <summary>
    /// Collider of the Enemy.
    /// </summary>
    private BoxCollider mBoxCollider;

    /// <summary>
    /// Called when the script instance is first loaded.
    /// </summary>
    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        
        // Get the collider and the trigger, making sure we got the correct one.
        var colliders = GetComponents<BoxCollider>();
        mBoxTrigger = colliders[0]; Assert.IsTrue(mBoxTrigger.isTrigger);
        mBoxCollider = colliders[1];
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { SetupCollider(); }

    /// <summary>
    /// Update called at fixed time delta.
    /// </summary>
    void FixedUpdate()
    {
        /*
         * Task #1B: Implement the enemy functionality
         * Useful functions and variables:
         *  - Transform of the currently controlled enemy: transform
         *    Contains its position and rotation
         *  - Get nearest player to given position: GameManager.Instance.NearestPlayer(position)
         *    Can return null, which should be checked for!
         *    Use player's transform to determine their position.
         *    Rotation can be calculated using Quaternion.LookRotation(direction, Vector3.forward).
         *      The "Vector3.forward" specifies the upward direction. For us this is (0, 0, 1).
         *  - Physical body of this enemy: mRigidBody
         *    Can control rotation and movement
         *    Use mRigidBody.MovePosition to move the enemy
         * Implement a simple AI, which will head towards the closest player and follow them.
         */

        GameObject nearestplayer = GameManager.Instance.NearestPlayer(transform.position);
        if (nearestplayer != null)
        {
            MoveEnemy(nearestplayer.transform.position);
            RotateEnemy(nearestplayer.transform.position);
        }
    }
    private void MoveEnemy(Vector3 targetPos)
    {
        Vector3 moveDirection = targetPos - transform.position;  //vector from enemy to nearest player
        moveDirection = moveDirection.normalized * speed * Time.deltaTime;  //move delta

        //mRigidBody.MovePosition(transform.position+ moveDirection);
        //print("Moving");
        transform.position += moveDirection;
    }
    private void RotateEnemy(Vector3 targetPos)
    {
        Vector3 rotateDirection = targetPos - transform.position;
        rotateDirection.Normalize();
        Quaternion lookRotation = Quaternion.LookRotation(rotateDirection);

        Vector3 q  = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100).eulerAngles;
        q.z = 90f;
        gameObject.transform.rotation = Quaternion.Euler(q);
    }


    /// <summary>
    /// Triggered when a collision is detected.
    /// </summary>
    /// <param name="other">The other collidable.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        { DestroyEnemy(); }
    }

    /// <summary>
    /// Destroy this Enemy.
    /// </summary>
    public void DestroyEnemy()
    {
        Destroy(gameObject); 
    }

    /// <summary>
    /// Setup collider for better collision detection.
    /// </summary>
    private void SetupCollider()
    {
        // Modify the rigid body to allow collisions on the y axis.
        var colliderCenter = mBoxCollider.center;
        mBoxCollider.center = new Vector3 {
            x = colliderCenter.x,
            y = colliderCenter.y, 
            z = transform.localScale.y, 
        };
        var colliderSize = mBoxCollider.size;
        mBoxCollider.size = new Vector3 {
            x = colliderSize.x,
            y = colliderSize.y, 
            //z = 1.0f / transform.localScale.y, 
            z = colliderSize.z, 
        };
    }
}
