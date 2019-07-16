using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SpeedSystem : ComponentSystem {
    
    struct Group
    {
        public Speed Speed;
    }

    protected override void OnUpdate()
    {
        foreach (var e in GetEntities<Group>())
        {
            e.Speed.value = 0;
        }
    }

}
