using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ReturnTest : MonoBehaviour {
    public float speed;
}

class ReturnSystem2 : ComponentSystem
{
   
    struct group
    {
        public ReturnTest Return;
    }
    protected override void OnUpdate()
    {
       
        foreach (var e in GetEntities<group>())
        {
            e.Return.speed = 0;
        }
    }
}
