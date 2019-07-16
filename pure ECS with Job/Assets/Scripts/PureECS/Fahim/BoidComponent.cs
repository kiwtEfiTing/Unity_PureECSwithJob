//Author: Fahim Ahmed

using Unity.Mathematics;
using Unity.Entities;
using System;


//Pure ECS always deals with struct, not class.
//IComponentData is a pure ECS-style component, meaning that it defines no behavior,
//only data.IComponentDatas are structs rather than classes
//
//This is a template struct of a single boid/cube
[Serializable]
public struct Boid : IComponentData {

   
    public float3 position;
    public float3 velocity;
    public float3 acceration;

    public float mass { get; set; }
    public float radius { get; set; }

    //clamp the max velocity magnitude
    public float maxLength { get; set; }

    //my functions need:
    public float3 initialPosition;
    public float3 driftPosition;

    public int isInitialStatus;
    public int isRunning;
    public int isStop;
    public int isDriftPosGot;
    public int isReturning;

    public float timer;
    public float freeTime;
    public float driftTime;

    public float bounceTimer;
    public float3 driftScale;
    public int id;
}

public class BoidComponent : ComponentDataWrapper<Boid> { }
