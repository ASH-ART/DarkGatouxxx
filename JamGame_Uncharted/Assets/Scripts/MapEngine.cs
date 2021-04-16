using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEngine : MonoBehaviour
{

    public int mapW, mapH, chunckS;

    GameObject[,] chuncks;
    public Sprite[] tileSprites;
    byte[] chunckID;

    public float chunckPerUnit = 1;
    private float hw;
    private float hh;
    public Transform Center;


    void Start()
    {
        chunckID = new byte[mapH * mapW];
        chunckID[0] = 0b_0101_0101; //D R U L
        chunckID[1] = 0b_0100_0001;
        chunckID[2] = 0b_0101_0000;
        chunckID[3] = 0b_0100_0001;
        chunckID[4] = 0b_0100_0000;

        chunckID[5] = 0b_0000_0100;
        chunckID[6] = 0b_0001_0100;
        chunckID[7] = 0b_0000_0101;
        chunckID[8] = 0b_0001_0100;
        chunckID[9] = 0b_0000_0101;

        chunckID[20] = 0b_0101_0101; //D R U L

        hh = Camera.main.orthographicSize;
        hw = hh * Camera.main.aspect;
        chuncks = new GameObject[mapW, mapH];
        //load the map
        for (int c = 0; c < mapH*mapW; c++)
        {
            chunckID[c] = 0b_0101_0101; //D R U L
            ushort[] chunck = chunkId2map(chunckID[c]);
            chuncks[(c % mapW), (c / mapH)] = newGO(1 / chunckPerUnit * (c % mapW), -1 / chunckPerUnit * (c / mapH), 0, 1 / (float)chunckS, null, transform);
            Transform chunckTf = chuncks[(c % mapW), (c / mapH)].GetComponent<Transform>();
            for (int t = 0; t < chunck.Length; t++)
            {
                GameObject go = newGO(t % chunckS, -t / chunckS, 0, 1, tileSprites[chunck[t]], chunckTf);
                if (chunck[t] == 1)
                {
                    ((Rigidbody2D)go.AddComponent(typeof(Rigidbody2D))).bodyType = RigidbodyType2D.Static;
                    BoxCollider2D box = (BoxCollider2D)go.AddComponent(typeof(BoxCollider2D));
                    box.offset = new Vector2(-.003f, .003f);
                    box.size = new Vector2(.125f, .125f);
                }
            }
            chunckTf.position = new Vector3(400, 400, 0); //put it far away for now
        }
    }

    // Update is called once per frame
    void Update()
    {
        //scrolling
        transform.position = transform.position - Center.position;
        Center.position = Vector3.zero;

        /* TODO : looping
         * for that, we need to move chunck as to cover camera -> loop through chunck on screen
         * + (%) cam position as to avoid overflow
         * 
         */
        float cX = Camera.main.transform.position.x;
        float cY = Camera.main.transform.position.y;
        int X = (int)(cX / transform.localScale.x);
        int Y = -(int)(cY / transform.localScale.y);
        for (float sX = (-hw / transform.localScale.x) - 2; sX < (hw / transform.localScale.x)+2; sX++) for (float sY = (-hh / transform.localScale.y) - 2; sY < (hh / transform.localScale.y)+2; sY++)
            {
                int x = X + (int)sX;
                int y = Y + (int)sY;
                try
                {
                    chuncks[(int)(((x % mapW) + mapW)% mapW), (int)(((y % mapH) + mapH)% mapH)].GetComponent<Transform>().localPosition = new Vector3((int)x, -(int)y);
                }
                catch
                {
                    Debug.Log(((int)((x % mapW) + mapW) % mapW).ToString() + " ; " + ((int)((((y % mapH) + mapH) % mapH))).ToString() + " ; " + x.ToString() + " ; " + y.ToString());
                }
            }
        //Debug.Log(((int)(x % mapW)).ToString() + " ; " + ((int)(y % mapH)).ToString() + " ; " + x.ToString() + " ; " + y.ToString() );
        /*for (float sX = (-hw / transform.localScale.x)-1; sX < (hw / transform.localScale.x); sX++) for (float sY = (-hh / transform.localScale.y)-1; sY < (hh / transform.localScale.y); sY++)
            {
                float cX = Camera.main.transform.position.x;
                float cY = Camera.main.transform.position.y;
                int x = (int)(sX + cX / transform.localScale.x);
                x = x%mapW;
                int y = (int)(sY + cY / transform.localScale.y);
                y = ((y < 0) ? (y % mapH) + mapH : y % mapH);
                chuncks[(int)x, (int)y].GetComponent<Transform>().localPosition 
                    = new Vector3( (int)(sX + cX / transform.localScale.x), (int)(-sY + cY / transform.localScale.y));
            }*/

    }

    public GameObject newGO(float x, float y, float rotation, float scale, Sprite sprite, Transform parent) //0 empty, 1 is not
    {
        scale *= (sprite == null) ? 1 : (sprite.texture.width / 4); //assuming w = h
        GameObject g1 = new GameObject("(" + (x.ToString() + " ; " + (-y).ToString() + ")"), typeof(SpriteRenderer));
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

    ushort[] chunkId2map(byte chunck)
    {
        ushort[] _map = new ushort[chunckS * chunckS];
        for (int d = 0; d<4; d++) //trough directions : R, U, L, D
        {
            bool open  = (chunck & (1 << d*2)) != 0;
            int wallSize = (chunck & (2 << d*2)) == 0 ? 1 : (1+chunckS/4);
            for (int i = 0; i < chunckS; i++)
            {
                for (int j = 0; j < wallSize; j++)
                {
                    switch (d)
                    {
                        case 0: _map[i * chunckS + j] = (ushort)((open && i == (chunckS / 2)) ? 0 : 1); break; //R
                        case 1: _map[i + chunckS * j] = (ushort)((open && i == (chunckS / 2)) ? 0 : 1); break; //U
                        case 2: _map[i * chunckS + (chunckS - 1 - j)] = (ushort)((open && i == (chunckS / 2)) ? 0 : 1); break; //L
                        case 3: _map[i + chunckS * (chunckS - 1 - j)] = (ushort)((open && i == (chunckS / 2)) ? 0 : 1); break; //D
                    }
                }
            }
        }
        return _map;
    }
}
