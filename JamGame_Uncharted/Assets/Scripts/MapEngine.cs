using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEngine : MonoBehaviour
{
    ushort[,] tiles; // chunck ; tile
    public int mapW, mapH, chunckW, chunckH;

    ArrayList tilesGameobjects = new ArrayList();
    public Sprite[] tileSprites;

    public float chunckPerUnit = 1;
    private float hw;
    private float hh;
    public Transform Center;


    void Start()
    {
        hh = Camera.main.orthographicSize;
        hw = hh * Camera.main.aspect;
        tiles = new ushort[mapH * mapW, chunckW * chunckH];
        //bullshit filling
        for (int c = 0; c < tiles.GetLength(0); c++)
        {
            for (int t = 0; t < tiles.GetLength(1); t++)
            {
                tiles[c, t] = (ushort)(c%2);
            }
        }
        //load the map
        for (int c = 0; c < tiles.GetLength(0); c++)
        {
            Transform chunckTf = newGO(1/chunckPerUnit * ( c % mapW), -1/chunckPerUnit * ( c / mapH ), 0, 1/(float)chunckW, null, transform).GetComponent<Transform>();
            for (int t = 0; t < tiles.GetLength(1); t++)
            {
                newGO(t % chunckW, -t / chunckH, 0, 1, tileSprites[tiles[c, t]], chunckTf);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //scrolling
        transform.position = transform.position - Center.position;
        Center.position = Vector3.zero;
    }

    public GameObject newGO(float x, float y, float rotation, float scale, Sprite sprite, Transform parent)
    {
        scale *= (sprite == null) ? 1 : (sprite.texture.width/4); //assuming w = h
        GameObject g1 = new GameObject("(" + (x.ToString() + " ; " + y.ToString() + ")"), typeof(SpriteRenderer));
        SpriteRenderer sprRenderer = g1.GetComponent<SpriteRenderer>();
        sprRenderer.drawMode = SpriteDrawMode.Tiled;
        sprRenderer.sortingLayerID = SortingLayer.NameToID("tile/chunck");
        g1.GetComponent<Transform>().SetParent(parent);
        g1.GetComponent<Transform>().localPosition = new Vector3(x, y, 0);
        g1.GetComponent<Transform>().localScale = new Vector3(scale, scale, 0);
        g1.GetComponent<Transform>().Rotate(0, 0, rotation);
        sprRenderer.sprite = sprite;

        return g1;
    }

}
