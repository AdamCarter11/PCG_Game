using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eggScript : MonoBehaviour
{
    private Vector2 targetScale; // The final scale you want to reach
    private float duration = 2f; // The duration in seconds to reach the target scale

    private Vector3 initialScale;
    private float elapsedTime = 0f;
    private bool isScaling = false;
    [SerializeField] GameObject enemyObj;

    private void Start()
    {
        // Store the initial scale of the object
        initialScale = transform.localScale;
        targetScale = initialScale * 4;
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value);
        StartScaling();
    }

    private void Update()
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
            if (t >= 1f && isScaling)
            {
                isScaling = false;
                GameObject tempEnemy = Instantiate(enemyObj, transform.position, Quaternion.identity);
                print("Spawned enemy");
                tempEnemy.transform.localScale /= 2;
                tempEnemy.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                tempEnemy.GetComponent<GoombaMove>().StartScaling();
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
