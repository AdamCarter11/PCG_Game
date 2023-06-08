using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class GoombaMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f; // The movement speed of the Goomba
    [SerializeField] float patrolDistance = 2f; // The distance the Goomba will patrol before turning back
    [SerializeField] float raycastDistance = 0.5f; // The distance of the raycast
    [SerializeField] LayerMask rayCastMask;
    [SerializeField] GameObject enemyObj;
    [SerializeField] float freezeTime;
    private bool isMovingRight = true; // Indicates if the Goomba is moving right or left
    private int moveDirection = 1;
    bool canSpawn = true;

    private Rigidbody2D rb;

    //scaling vars
    private Vector2 targetScale; // The final scale you want to reach
    private float duration = 2f; // The duration in seconds to reach the target scale

    private Vector3 initialScale;
    private float elapsedTime = 0f;
    private bool isScaling = false;
    bool canMove = true;
    Transform objectToFollow;
    Vector3 offset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        targetScale = new Vector3(.8f, .8f, 1);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            MoveFlipLogic();
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.MovePosition(objectToFollow.position + offset);
        }
            
    }

    private void MoveFlipLogic()
    {
        // Move the Goomba horizontally
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
        //print("velocity: " + rb.velocity);

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(moveDirection, 0), raycastDistance, rayCastMask);
        //print("hit: " + hit.collider);
        // Visualize the raycast
        Debug.DrawRay(transform.position, new Vector2(moveDirection, 0) * raycastDistance, Color.green);
        if (hit.collider != null)
        {
            print("hit: " + hit.collider);
            // If an obstacle is detected, flip the Goomba's direction
            Flip();
        }

        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, new Vector2(moveDirection, -1), raycastDistance * 2, rayCastMask);
        Debug.DrawRay(transform.position, new Vector2(moveDirection, -1) * raycastDistance * 2, Color.blue);
        if (groundCheck.collider == null)
        {
            print("ground: " + groundCheck.collider);
            Flip();
        }
        if (transform.position.y <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        ScalingLogic();
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        moveDirection *= -1; // Reverse the movement direction

        // Flip the sprite horizontally
        //rb.velocity = Vector2.zero;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // TO DO: figure out how to do the random evo stuff
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // TO DO: use a bool and switch it on/off on the other enemy
            //          then spawn an egg that doesn't have a collider that hatches after a while
            if(!Vector3.Equals(collision.transform.localScale, transform.localScale))
            {
                
                collision.gameObject.GetComponent<GoombaMove>().changeSpawnVal();
                EnemyMate(collision.gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            if(Mathf.Abs(collisionNormal.y) < 0.5f)
            {
                print("player goomba collision");
                objectToFollow = collision.transform;
                offset = transform.position - objectToFollow.position;
                StartCoroutine(freezeGoomba(collision.gameObject));
            }
            
        }
    }
    IEnumerator freezeGoomba(GameObject targetToFollow)
    {
        canMove = false;
        transform.SetParent(targetToFollow.transform);
        yield return new WaitForSeconds(freezeTime);
        transform.SetParent(null);
        canMove = true;
    }
    void EnemyMate(GameObject otherObj)
    {
        // TO DO: fix this, why is this running on a spawned enemy
        print("Goomba collision");
        if (canSpawn)
        {
            Destroy(this.gameObject);
            Destroy(otherObj);
            GameObject tempObj = Instantiate(enemyObj, transform.position, Quaternion.identity);
        }
        //tempObj.transform.localScale /= 2;
        //tempObj.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }
    public void changeSpawnVal()
    {
        canSpawn = false;
    }
    private void ScalingLogic()
    {
        if (isScaling)
        {
            // Increase the elapsed time since the scaling started
            elapsedTime += Time.deltaTime;

            // Calculate the interpolation factor based on the elapsed time and duration
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Interpolate the scale between the initial and target scale using Lerp
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            // Check if the scaling is complete
            if (t >= 1f)
            {
                isScaling = false;
                GameObject tempEnemy = Instantiate(enemyObj, transform.position, Quaternion.identity);
                tempEnemy.transform.localScale /= 2;
                tempEnemy.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                Destroy(this.gameObject);
            }
        }
    }
    public void StartScaling()
    {
        // Reset the elapsed time and enable scaling
        elapsedTime = 0f;
        isScaling = true;
    }
}
