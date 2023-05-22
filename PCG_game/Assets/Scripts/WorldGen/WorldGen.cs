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

    private void Start()
    {
        Generation();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generation();
        }
    }
    void Generation()
    {
        // we use a seed currently in case we want to have the option of seeding the world and for testing
        seed = Time.time;  
        // Clear tilemaps before generating new sprites on them
        groundTileMap.ClearAllTiles();
        caveTileMap.ClearAllTiles();
        // Generate the map (array) of size width x height that contains ints representing which tile to place (air, ground or cave)
        map = GenerateArray(width, height, true);
        map = TerrainGeneration(map);
        // Places the sprites down based on the generated map
        RenderMap(map, groundTileMap, caveTileMap, groundTile, caveTile);
    }

    private int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for(int x = 0; x < width; x++)
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

    private int[,] TerrainGeneration(int[,] map)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
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
                int caveVal = Mathf.RoundToInt(Mathf.PerlinNoise((x * modifier) + seed, (y * modifier) + seed));
                // if caveVal is 1 then put a 2 in the map (which represents the cave sprite), if not, a 1 is placed to represent the ground sprite
                map[x, y] = (caveVal == 1) ? 2 : 1;
            }
        }
        return map;
    }

    private void RenderMap(int[,] map, Tilemap groundTileMap, Tilemap caveTileMap, TileBase groundTileBase, TileBase caveTile)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x,y] == 1)
                {
                    // sets tile at the position of (x,y) with the specified tile. This works because the tiles are sized to match the in game grid
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
                }
                else if (map[x,y] == 2)
                {
                    caveTileMap.SetTile(new Vector3Int(x, y, 0), caveTile);
                }
            }
        }
    }


}
