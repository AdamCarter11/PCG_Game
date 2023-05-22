using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int worldWidth = 10;                      // Width of the world in tiles
    public int worldHeight = 10;                     // Height of the world in tiles
    public GameObject[] tilePrefabs;                 // Array of tile prefabs to use for generation
    public float fillPercentage = 0.45f;             // Initial fill percentage of the cave

    private int[,] worldMap;

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        worldMap = new int[worldWidth, worldHeight];

        // Generate initial random cave
        GenerateRandomCave();

        // Smooth the cave using cellular automata
        for (int i = 0; i < 5; i++)
        {
            SmoothCave();
        }

        // Spawn tiles based on the cave data
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                Vector2 spawnPosition = new Vector2(x, y);
                //GameObject tilePrefab = worldMap[x, y] == 1 ? tilePrefabs[0] : tilePrefabs[1]; // Assumes tilePrefabs[0] is the ground and tilePrefabs[1] is the wall
                //Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                if (worldMap[x, y] == 1)
                {
                    Instantiate(tilePrefabs[1], spawnPosition, Quaternion.identity);
                }
                else
                {
                    Instantiate(tilePrefabs[0], spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    private void GenerateRandomCave()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (x == 0 || x == worldWidth - 1 || y == 0 || y == worldHeight - 1)
                {
                    worldMap[x, y] = 1; // Set the borders as walls
                }
                else
                {
                    worldMap[x, y] = Random.value < fillPercentage ? 0 : 1;
                }
            }
        }
    }

    private void SmoothCave()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                int adjacentWalls = GetAdjacentWallCount(x, y);

                if (adjacentWalls > 4)
                {
                    worldMap[x, y] = 1;
                }
                else if (adjacentWalls < 4)
                {
                    worldMap[x, y] = 0;
                }
            }
        }
    }
    private int GetAdjacentWallCount(int x, int y)
    {
        int wallCount = 0;

        for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
        {
            for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < worldWidth && neighborY >= 0 && neighborY < worldHeight)
                {
                    if (neighborX != x || neighborY != y)
                    {
                        wallCount += worldMap[neighborX, neighborY];
                    }
                }
                else
                {
                    wallCount++; // Consider out-of-bounds positions as walls
                }
            }
        }

        return wallCount;
    }
}
