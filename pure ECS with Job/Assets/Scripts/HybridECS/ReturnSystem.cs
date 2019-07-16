using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Jobs;

public class ReturnSystem : ComponentSystem
{

    struct Group
    {

        public ReturnComponent Return;
        public Transform Transform;
      
    }

    int isInitialPosGot = 0;
    Vector3[] initialPositions;

    protected override void OnUpdate()
    {
        //while (isInitialPosGot == 0)
        //{
        //    //initialPositions = new Vector3[]
        //    isInitialPosGot = 1;
        //    foreach (var entity in GetEntities<Group>())
        //    {
        //        var position = entity.Transform.position;

        //    }

        //}
        //foreach (var entity in GetEntities<Group>())
        //{
        //     entity.Return.timer = 0;
        //}


        var entities = GetEntities<Group>();
        var count = entities.Length;

        
    }


    protected override void OnCreateManager()
    {
        base.OnCreateManager();
    }
}
