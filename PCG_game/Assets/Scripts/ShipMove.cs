using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipMove : MonoBehaviour
{
    public float acceleration = 5f;
    public float deceleration = 10f;
    public float maxSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    bool onPlanet = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ShipMovement();
        if(Input.GetKeyDown(KeyCode.Space) && onPlanet)
        {
            SceneManager.LoadScene("PlanetScene");
        }
    }

    void ShipMovement()
    {
        // get user input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // convert input to a vector and apply movespeed
        Vector2 desiredVelocity = new Vector2(moveHorizontal, moveVertical) * maxSpeed;

        // Calculate acceleration and deceleration
        Vector2 accelerationVector = (desiredVelocity - currentVelocity).normalized * acceleration;
        Vector2 decelerationVector = -currentVelocity.normalized * deceleration;

        // Apply acceleration and deceleration
        currentVelocity += accelerationVector * Time.deltaTime;
        currentVelocity = Vector2.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity += decelerationVector * Time.deltaTime;

        // prevents stuttering when the acceleration and decceleration are small values
        if (currentVelocity.x < .1f && currentVelocity.y < .1f && moveHorizontal == 0 && moveVertical == 0)
            currentVelocity = Vector2.zero;

        rb.velocity = currentVelocity;

        // Rotate the spaceship based on the movement direction
        if (currentVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            string planetName = collision.gameObject.name;
            int intPlanet = int.Parse(planetName.Substring(planetName.IndexOf(" ")));
            print("which planet: " + intPlanet);
            Gamemanager.whichPlanetToLoad = intPlanet;
            onPlanet = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Planet"))
        {
            onPlanet = false;
        }
    }

}
