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
    private bool isMovingRight = true; // Indicates if the Goomba is moving right or left
    private int moveDirection = 1;

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
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
        if(groundCheck.collider == null)
        {
            print("ground: " + groundCheck.collider);
            Flip();
        }
        if(transform.position.y <= 0)
        {
            Destroy(this.gameObject);
        }
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
            EnemyMate(collision.gameObject);
        }
    }
    void EnemyMate(GameObject otherObj)
    {
        int rando = Random.Range(0, 2);
        if (rando == 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(otherObj.gameObject);
        }
        GameObject tempObj = Instantiate(enemyObj, new Vector3(transform.position.x + 2, transform.position.y, transform.position.z), Quaternion.identity);
        tempObj.transform.localScale /= 2;
        tempObj.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }
}
