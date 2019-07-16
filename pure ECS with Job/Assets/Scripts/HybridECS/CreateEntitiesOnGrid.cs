using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEntitiesOnGrid : MonoBehaviour {

    public int gridSizeX;
    public int gridSizeY;
    public int gridGap;
    public GameObject prefab;
    GameObject[] cubes;
    Vector3[] initialPositions;

    // Use this for initialization
    void Start()
    {
        var count = (gridSizeX + 1) * (gridSizeY + 1);
        initialPositions = new Vector3[count];
        cubes = new GameObject[count];
        Generate();

    }


    void Generate()
    {
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        for (int i = 0, y = 0; y <= gridSizeY * gridGap; y += gridGap)
        {
            for (int x = 0; x <= gridSizeX * gridGap; x += gridGap, i++)
            {
                initialPositions[i] = new Vector3(x, y, 0);
                cubes[i] = Instantiate(prefab, initialPositions[i], rot) as GameObject;
            }
        }
    }
}
