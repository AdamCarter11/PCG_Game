using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour
{
    // if you get sprite tearing or lines in between the tiles, make a sprite atlas

    [Header("Terrain Gen")]
    [SerializeField] int width, height;
    [SerializeField] float smoothness;
    [SerializeField] float seed;
    [SerializeField] int heightReduction = 2;

    [Header("Cave Gen")]
    [Range(0.01f, 1f)]
    [SerializeField] float modifier;

    [Header("Tiles")]
    [SerializeField] TileBase groundTile;   //  TileBase is a (Create ->2D ->Tiles ->RuleTile) that puts the correct sprite in based on surrounding sprites
    [SerializeField] TileBase caveTile;
    [SerializeField] Tilemap caveTileMap;
    [SerializeField] Tilemap groundTileMap;
    
    //  map to contain ints representing which tiles to be placed
    int[,] map;

    // attempt at making it generate in a row
    [Header("Row Gen")]
    [SerializeField] int numberOfChunks = 10; // Total number of chunks in the row
    [SerializeField] float chunkSpacing = 2f; // Spacing between chunks
    [SerializeField] float chunkSize = 2f; // Size of each chunk
    [SerializeField] float chaosFactor = 0.0f; // Controls the level of chaos
    int chunkStartingX = 0;

    [Header("Characters Spawning")]
    private GameObject gmObject;
    private Gamemanager gm;
    [SerializeField] GameObject boundryObj;
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject enemyObj;
    [SerializeField] GameObject shipObj;
    [SerializeField] int percentChanceForEnemy;
    bool spawnedPlayer = false;
    bool spawnedShip = false;
    int playerSpawnedX = -1;

    private void Start()
    {
        gmObject = GameObject.FindGameObjectWithTag("GameManager");
        if(gmObject != null)
        {
            gm = gmObject.GetComponent<Gamemanager>();
            seed = Gamemanager.instance.ReturnSeed();   // TO DO: make each chunk have a different seed (so they don't all match the first chunk
        }
        else
        {
            print("gamemanger is null. Are you testing?");
            seed = Time.time;
        }
        GameObject tempLeftBoundry = Instantiate(boundryObj, new Vector2(chunkStartingX, 0), Quaternion.identity);
        tempLeftBoundry.name = "LeftBoundry";

        for (int i = 0; i < numberOfChunks; i++)
        {
            if ((int)numberOfChunks / (i + 1) == 2 && !spawnedPlayer)
            {
                spawnedPlayer = true;
                playerObj.transform.position = new Vector2(chunkStartingX, height);
                playerSpawnedX = chunkStartingX;
            }
            Generation(chunkStartingX);
            /*
            if (chaosFactor < .1)
                chaosFactor += .01f;
            */
            chunkStartingX += width;
        }
        GameObject tempRightBoundry = Instantiate(boundryObj, new Vector2(chunkStartingX, 0), Quaternion.identity);
        tempRightBoundry.name = "RightBoundry";
    }
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            chunkStartingX += width;
            Generation(chunkStartingX);
            if(chaosFactor < .1)
                chaosFactor += .01f;
        }
        */
    }
    void Generation(int startingX)
    {
        // we use a seed currently in case we want to have the option of seeding the world and for testing
        //seed = Time.time;  
        // Clear tilemaps before generating new sprites on them

        //  IMPORTANT IF I WANT TO CLEAR MAP (1 chunk) AT A TIME
        //groundTileMap.ClearAllTiles();
        //caveTileMap.ClearAllTiles();

        // Generate the map (array) of size width x height that contains ints representing which tile to place (air, ground or cave)
        map = GenerateArray(width, height, true, startingX);
        map = TerrainGeneration(map, startingX);
        // Places the sprites down based on the generated map
        RenderMap(map, groundTileMap, caveTileMap, groundTile, caveTile, startingX);
    }

    private int[,] GenerateArray(int width, int height, bool empty, int startingX)
    {
        int[,] map = new int[width + startingX, height];
        for(int x = 0 + startingX; x < width + startingX; x++)
        {
            for(int y = 0; y < height; y++)
            {
                /*
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
                */
                //this line of code is the equivalent of the if else above
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }

    private int[,] TerrainGeneration(int[,] map , int startingX)
    {
        int perlinHeight;
        for (int x = 0 + startingX; x < width + startingX; x++)
        {
            // smoothness value controls how steep mountains can be, heightReduction controls total height
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smoothness, seed) * height / heightReduction);
            if(heightReduction > 1)
            {
                perlinHeight += height / 2;
            } 
            for (int y = 0; y < perlinHeight; y++)
            {
                //map[x, y] = 1;
                int caveVal = Mathf.RoundToInt(Mathf.PerlinNoise((x * (modifier + chaosFactor)) + seed, (y * (modifier + chaosFactor)) + seed));
                // if caveVal is 1 then put a 2 in the map (which represents the cave sprite), if not, a 1 is placed to represent the ground sprite
                map[x, y] = (caveVal == 1) ? 2 : 1;
            }
        }
        return map;
    }

    private void RenderMap(int[,] map, Tilemap groundTileMap, Tilemap caveTileMap, TileBase groundTileBase, TileBase caveTile, int startingX)
    {
        for (int x = 0 + startingX; x < width + startingX; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x,y] == 1)
                {
                    // sets tile at the position of (x,y) with the specified tile. This works because the tiles are sized to match the in game grid
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
                    if (map[x,y+1] == 0)
                    {
                        if (!spawnedShip && x == playerSpawnedX)
                        {
                            spawnedShip = true;
                            shipObj.transform.position = new Vector2(playerSpawnedX, y + 2);
                        }
                        if (Random.Range(0,100) <= percentChanceForEnemy)
                        {
                            Instantiate(enemyObj, new Vector2(x, y+2), Quaternion.identity);
                        }
                    }
                    
                }
                else if (map[x,y] == 2)
                {
                    caveTileMap.SetTile(new Vector3Int(x, y, 0), caveTile);
                }
            }
        }
    }


}
