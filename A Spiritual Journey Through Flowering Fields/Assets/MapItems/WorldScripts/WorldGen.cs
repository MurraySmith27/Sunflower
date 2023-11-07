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
            colorMap.Add(map.col.r * 1000000 + map.col.g * 1000 + map.col.b, map.tile);
        }
    }

    void Awake()
    {
        addColorMapping();
    }


    public void GenerateMap(int worldIndex)
    {
        Vector2 worldSize = new Vector2(1, 1);
        Vector3 roomSize = new Vector2(rooms[worldIndex].width, rooms[worldIndex].height) * tileSize;
        

        Vector3 pos = Vector3.zero;

        Texture2D tile = rooms[worldIndex];
        generateRoom(tile, pos, tileSize);
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
            
            int key = pixels[i].r * 1000000 + pixels[i].g * 1000 + pixels[i].b;
            if (colorMap.ContainsKey(key)) {
                Vector3 tilePos = row * tileSize * dir1 + col * tileSize * dir2 + position;
                Instantiate(colorMap[key], tilePos, Quaternion.identity);

            } else
            {
                Debug.LogError($"Invalid Key: {key}");
            }
        }
    }

}
