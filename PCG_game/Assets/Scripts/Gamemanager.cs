using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WorldStruct
{
    public float seed;
    public float size;
    public int creatureAmount;
    public List<string> creatures; //  TO DO: make the string an actual creature class
}

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    List<WorldStruct> worldsList;
    [SerializeField] int amountOfWorlds;
    int counter = 0;

    [Header("Planet Spawning Vars")]
    [SerializeField] GameObject objectPrefab;
    //[SerializeField] int numberOfObjects;
    [SerializeField] float spawnRadius;
    [SerializeField] float centerWeight;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    [HideInInspector] public static int whichPlanetToLoad;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateWorlds();
        //OutputWorldsData();
        SpawnObjects();
    }
    public float ReturnSeed()
    {
        return worldsList[whichPlanetToLoad].seed;
    }

    private void GenerateWorlds()
    {
        worldsList = new List<WorldStruct>();
        for (int i = 0; i < amountOfWorlds; i++)
        {
            WorldStruct worldStruct = new WorldStruct();
            worldStruct.seed = GenerateUniqueSeed() + counter * 0.001f;
            counter++;

            // used to create a new seed so size doesn't match
            System.Random random = new System.Random();

            // Generate a random seed
            int seed = random.Next();

            // Set the seed for Unity's random number generator
            UnityEngine.Random.InitState(seed);
            worldStruct.size = UnityEngine.Random.Range(1f, 3f);
            worldStruct.creatureAmount = UnityEngine.Random.Range(1, 5);
            worldStruct.creatures = new List<string>();
            // TO DO: Add creatures to the worldStruct.creatures list

            worldsList.Add(worldStruct);
        }
    }
    private void OutputWorldsData()
    {
        foreach (WorldStruct worldStruct in worldsList)
        {
            Debug.Log("Seed: " + worldStruct.seed);
            Debug.Log("Size: " + worldStruct.size);
            Debug.Log("Creature Amount: " + worldStruct.creatureAmount);
            // TO DO: Access and use worldStruct.creatures list
        }
    }
    private float GenerateUniqueSeed()
    {
        DateTime now = DateTime.Now;
        int randomSeed = now.Millisecond + now.Second * 1000 + now.Minute * 60000 + now.Hour * 3600000;
        UnityEngine.Random.InitState(randomSeed);
        float seed = UnityEngine.Random.Range(0f, 10000f);
        return seed;
    }

    #region SpawningPlanets
    private void SpawnObjects()
    {
        List<Vector2> spawnPositions = new List<Vector2>();
        float angleIncrement = 360f / amountOfWorlds;
        float currentAngle = 0f;

        for (int i = 0; i < amountOfWorlds; i++)
        {
            float randomRadius = UnityEngine.Random.Range(0f, spawnRadius);
            float weightedRadius = Mathf.Lerp(randomRadius, spawnRadius, centerWeight);
            Vector2 spawnPosition = GetPolarToCartesian(currentAngle, weightedRadius);

            // Check for overlapping
            float worldSize = worldsList[i].size;
            bool isOverlapping = IsOverlapping(spawnPosition, spawnPositions, worldSize);
            if (isOverlapping)
            {
                i--;
                continue;
            }
            //print("OVERLAPPING: " + isOverlapping);
            // Spawn object at the calculated position
            GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            spawnedObject.name = "Planet: " + i;
            spawnedObject.transform.localScale *= worldSize;
            spawnedObject.GetComponent<PlanetScript>().planetData = worldsList[i];
            // Add any additional logic for configuring the spawned object

            spawnPositions.Add(spawnPosition);
            currentAngle += angleIncrement;
        }
    }

    private Vector2 GetPolarToCartesian(float angle, float radius)
    {
        float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }

    private bool IsOverlapping(Vector2 position, List<Vector2> existingPositions, float distanceBetween)
    {
        float minDistance = 1f * distanceBetween; // Minimum distance between objects to consider them not overlapping

        foreach (Vector2 existingPosition in existingPositions)
        {
            float distance = Vector2.Distance(position, existingPosition);
            if (distance < minDistance)
                return true;
        }

        return false;
    }
    
    #endregion
}
