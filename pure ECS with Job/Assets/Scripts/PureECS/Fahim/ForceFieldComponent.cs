//Author: Fahim Ahmed

using Unity.Mathematics;
using Unity.Entities;
using System;
using UnityEngine;

//Pure ECS always deals with struct, not class.
//IComponentData is a pure ECS-style component, meaning that it defines no behavior,
//only data.IComponentDatas are structs rather than classes
[Serializable]
public struct Forcefield : IComponentData {
    public enum ForceMode {
        PUSH,
        PULL
    }

    //this is basically the mass of your mouse pointer / forcefield
    public const float G = 9.8f;

    //friction level. how fast the velocity will fall.
    public float frictionCoe;

    //mass of the forcefield / pointer
    public float Mass;

    //x, y, w, h
    public float4 bound;


    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }


    //apply force on boids
    public float3 CastForce(ref float3 position, ref Boid b, ForceMode forceMode, float maxDistance)
    {   
        if (forceMode == ForceMode.PUSH)
        {
            float3 forceDir = (position - b.position);
            float d = math.length(forceDir);

            float clampMin = 40;
            float clampMax = 50;
            //d = map(d, 0, maxDistance, 45, 55);
            //d = math.clamp(d, clampMin, clampMax);

            forceDir = math.normalize(forceDir);


            //Mass = 1;
            // F = GMm / d^2
            //float strength = (G * Mass * b.mass) / (d * d * d);
            float maxStrength = 0.4f;
            float zeroDistance = 5;
            //float strength = (-1) * maxStrength / (zeroDistance * zeroDistance) * (math.length(position - b.position)) * (math.length(position - b.position))
            //   + maxStrength;

            //float strength = (maxStrength / (zeroDistance * zeroDistance) - zeroDistance)
            //    * (math.length(position - b.position)) * (math.length(position - b.position));

            float strength = zeroDistance / (math.length(position - b.position) + maxStrength) -0.06f;


            strength = math.clamp(strength, 0, 20);

            //if (strength < 0f)
            //{
            //    //strength = map(strength, ((-1) * maxStrength / (zeroDistance * zeroDistance) * (maxDistance / 2) * (maxDistance / 2)
            //    //+ maxStrength), -0.2f, 0,  0.05f);
            //    strength = 0;
            //}

            //if (math.length (position - b.position) > maxDistance)
            //{
            //    strength = 0;
            //}


            //strength = map(strength, ((G * Mass * b.mass) / (clampMax * clampMax * clampMax)),
            //    ((G * Mass * b.mass) / (clampMin * clampMin * clampMin)),
            //    0, 0.2f);


            //if (math.length(position - b.position) > 5 )
            //{
            //    strength = map(strength, 0, maxDistance , 0.2f, 0)  ;
            //}
            //if (math.length(position - b.position) > 10)
            //{
            //    strength = 0;
            //}





            forceDir *= strength;
            //return (forceMode == ForceMode.PUSH) ? forceDir * -1 : forceDir;
            return (forceDir * -1) ;
        }
        else
        {
            float3 forceDir = (position - b.position);
            float d = math.length(forceDir);

            d = math.clamp(d, 25, 30);
            forceDir = math.normalize(forceDir);

            Mass = 20;

            // F = GMm / d^2
            float strength = (G * Mass * b.mass) / (d * d);

            if (math.length(position - b.position) > 95)
            {
                strength = 0;
            }

            forceDir *= strength;
            return  forceDir;
        }

      
    }
}

public class ForceFieldComponent : ComponentDataWrapper<Forcefield> { }
