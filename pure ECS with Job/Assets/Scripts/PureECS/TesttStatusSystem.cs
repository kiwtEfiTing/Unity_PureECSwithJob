using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TestStatusSystem : ComponentSystem {

    private struct Group
    {
        public TestStatusComponent status;
        public Rigidbody testRigidbody;
    }

    

    protected override void OnUpdate()
    {
       

        foreach (var entity in GetEntities<Group>())
        {

           if (entity.testRigidbody.velocity != Vector3.zero)
            {
                entity.status.isCollisionEnter = true;
            }
        }
    }
}
