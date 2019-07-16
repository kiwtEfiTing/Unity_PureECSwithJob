using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class ObjGridSystem : ComponentSystem {

    private static EntityArchetype cubeArchetype;
    private static MeshInstanceRenderer cubeRenderer;


    struct Group
    {
        [ReadOnly]

        public SharedComponentDataArray<ObjGrid> PointObject;

        public ComponentDataArray<Position> Position;
        public EntityArray Entity;
        public readonly int Length;

    }

    [Inject] Group m_Group;

    protected override void OnUpdate()
    {
        while (m_Group.Length != 0)
        {
            var pointObject = m_Group.PointObject[0];
            var sourceEntity = m_Group.Entity[0];
            var center = m_Group.Position[0].Value;
            var count = (pointObject.gridSizeX + 1) * (pointObject.gridSizeY + 1);

            var entities = new NativeArray<Entity>(count, Allocator.Temp);
            //EntityManager.Instantiate(pointObject.prefab, entities);
            

            var positions = new NativeArray<float3>(count, Allocator.Temp);

            float ox = pointObject.gridSizeX * pointObject.gridGap;
            float oy = pointObject.gridSizeY * pointObject.gridGap;
            //bound of the forcefield
            float4 v = new float4(-ox * 0.5f, -oy * 0.5f, Camera.main.aspect * Camera.main.orthographicSize * 2, Camera.main.orthographicSize * 2);

            float forceMass = pointObject.forceMass;

            //Debug.Log(v);
            //float4 v = new float4(-200, -100, 480, 270);
            //forcefield properties
            Forcefield f = new Forcefield { Mass = forceMass, bound = v, frictionCoe = 0.15f };

            for (int i = 0, y = 0; y <= pointObject.gridSizeY * pointObject.gridGap; y += pointObject.gridGap)
            {
                for (int x = 0; x <= pointObject.gridSizeX * pointObject.gridGap; x += pointObject.gridGap, i++)
                {
                    //var z = UnityEngine.Random.Range(-0.5f, 0.5f);
                    positions[i] = new Vector3(x, y);
                    cubeArchetype = EntityManager.CreateArchetype(
                                     typeof(Position),
                                     typeof(Boid),
                                     typeof(Rotation),
                                     typeof(Scale),
                                     typeof(MeshInstanceRenderer),
                                     typeof(Forcefield));
                    Position initialPosition = new Position {Value = positions[i]};
                    Scale scale = new Scale { Value = new Vector3(7, 7, 7) };
                    Boid b = new Boid
                    {
                        position = initialPosition.Value,
                        initialPosition = initialPosition.Value,
                        radius = 1f,
                        mass = pointObject.boidMass,
                        maxLength = 10,
                        velocity = Vector3.zero,
                        acceration = Vector3.zero,
                        timer = 0,
                        freeTime = pointObject.freeTime,
                        isDriftPosGot = 0,
                        driftTime = pointObject.driftTime,
                        isReturning = 0,
                        isRunning = 0,
                        driftPosition = 0,
                        isInitialStatus = 1,
                        isStop = 0,
                        driftScale = 1,
                        id = i,
                        bounceTimer = 0
                    };
                   
                    entities[i] = EntityManager.CreateEntity(cubeArchetype);
                    EntityManager.SetComponentData(entities[i], initialPosition);
                    EntityManager.SetComponentData(entities[i], scale);
                    EntityManager.SetComponentData(entities[i], f);
                    EntityManager.SetComponentData(entities[i], b);
                    EntityManager.SetSharedComponentData(entities[i], new MeshInstanceRenderer
                    {
                        mesh = pointObject.myMesh,
                        material = pointObject.myMat
                    });
                }
            }

            
       


            entities.Dispose();
            positions.Dispose();

            EntityManager.RemoveComponent<ObjGrid>(sourceEntity);

            // Instantiate & AddComponent & RemoveComponent calls invalidate the injected groups,
            // so before we get to the next spawner we have to reinject them  
            UpdateInjectedComponentGroups();

          
        }

    }
}
