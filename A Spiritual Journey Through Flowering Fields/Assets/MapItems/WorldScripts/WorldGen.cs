using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldGen : MonoBehaviour
{

    [Serializable]
    public struct ColorMapping
    {
        public Color32 col;
        public GameObject tile;
    }

    public List<ColorMapping> mappings;
    public List<Texture2D> rooms;
    public float tileSize = 10;

    Dictionary<int, GameObject> colorMap = new Dictionary<int, GameObject>();

    void addColorMapping()
    {
        foreach(ColorMapping map in mappings)
        {
            colorMap.Add(map.col.r, map.tile);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        addColorMapping();
        generateWorld();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void generateWorld()
    {
        Vector2 worldSize = new Vector2(1, 1);
        Vector3 roomSize = new Vector2(rooms[0].width, rooms[0].height) * tileSize;
        

        for (int r = 0; r < worldSize.x; r++)
        {
            for(int c = 0; c < worldSize.y; c++)
            {
                Vector3 pos = new Vector3(r * roomSize.x, 0, c * roomSize.y);
                int tileIndex = UnityEngine.Random.Range(0, rooms.Count);
                Texture2D tile = rooms[tileIndex];
                generateRoom(tile, pos, tileSize);
            }
        }
    }
    

    void generateRoom(Texture2D tile, Vector3 position, float tileSize)
    {

        Color32[] pixels = tile.GetPixels32();

        Vector3 dir1 = new Vector3(2.33f, 0, -0.2f);
        Vector3 dir2 = new Vector3(-0.2f, 0, 2.33f);

        for(int i  = 0; i  < pixels.Length; i++)
        {
            int row = i / tile.width;
            int col = i % tile.height;

            //int nearestFactor = 5;
            //int redVal = (int)(Mathf.Round(pixels[i].r / nearestFactor)) * nearestFactor;
            int redVal = pixels[i].r;

            if (colorMap.ContainsKey(redVal)) {
                Vector3 tilePos = row * tileSize * dir1 + col * tileSize * dir2 + position;
                Instantiate(colorMap[redVal], tilePos, Quaternion.identity);

            } else
            {
                Debug.Log(redVal);
            }
        }
    }

}
