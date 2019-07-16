using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;


public class MeshRenderSystem : JobComponentSystem {

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new ChangeMatJob
        {

        };
        return jobHandle.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct ChangeMatJob : IJobProcessComponentData<Boid>
    {
        public void Execute(ref Boid boid)
        {

        }
    }
}
