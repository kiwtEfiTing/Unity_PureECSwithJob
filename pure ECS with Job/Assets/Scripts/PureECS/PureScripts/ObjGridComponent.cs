using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

[Serializable]
public struct ObjGrid : ISharedComponentData
{
    public GameObject prefab;
    public int gridSizeX;
    public int gridSizeY;
    public int gridGap;

    public Mesh myMesh;
    public Material myMat;

    public float freeTime;
    public float driftTime;

    public float forceMass;
    public float boidMass;


}


public class ObjGridComponent : SharedComponentDataWrapper<ObjGrid> { }
