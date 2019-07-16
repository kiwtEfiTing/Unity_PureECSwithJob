//Author: Fahim Ahmed

using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Rendering;

//A job system puts jobs into a queue to execute. 
//Worker threads in a job system take items from the job queue and execute them.
//This is the main happening center. This where you will write behaviours / logics
public class BoidSystem : JobComponentSystem
{
    bool _isForceOn = false;
    Forcefield.ForceMode _forceMode = Forcefield.ForceMode.PULL;
    float3 mousePos;

    float3 _freePos = 0;
    bool _isFreePosGot = false;
    int isSceneStart = 1;

    float _k = 50f;
    float _fric = 24f;
    float _force = 1f;
    float3 _mVelocity = 0;
    int bounceBool = 0;
    float _maxDistance = 0;
    Vector3 furthestMous;

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

  
    //equivalent to Update method from MonoBehaviour.
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        while (_maxDistance == 0)
        {
            furthestMous = GetWorldPositionOnPlane(
                  new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0), 0);

            _maxDistance = math.length(furthestMous - Vector3.zero);
            Debug.Log(_maxDistance);
        }
        // ------- input handle ------
        //left - pull
        if (Input.GetMouseButtonDown(0))
        {
            _isForceOn = true;
            _forceMode = Forcefield.ForceMode.PULL;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isForceOn = false;
        }

        //right - push
        if (Input.GetMouseButtonDown(1))
        {
            _isForceOn = true;
            _forceMode = Forcefield.ForceMode.PUSH;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isForceOn = false;
        }

        mousePos = GetWorldPositionOnPlane(Input.mousePosition, 0);
        mousePos.z = 0;
        // ------- end input handle ------

        //start a job for entity. you will access the properties of entity from here.
        //JobProcess is a struct that will implement the IJobProcessComponentData interface
        //job receives parameters and operates on data, similar to how a method call behaves.    
        //you can not  have instance property or initializer inside a struct. You have to pass it from outside
        var job = new JobProcess {
            isForceOn = _isForceOn,
            forceMode = _forceMode,
            mousePosition = mousePos,
            

            deltaTime = Time.deltaTime,
            freePos = _freePos,
            isFreePosGot = _isFreePosGot,
            k = _k,
            fric= _fric,
            force = _force,
            mVelocity = _mVelocity,
            maxDistance = _maxDistance
        };

        //start the job execution. This allows you to schedule a single job that runs 
        //in parallel to other jobs and the main thread.            
        return job.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct JobProcess : IJobProcessComponentData<Position, Boid, Forcefield, Scale>
    {
        public bool isForceOn;
        public Forcefield.ForceMode forceMode;
        public float3 mousePosition;

        

        public float deltaTime;
        public bool isFreePosGot;
        public float3 freePos;
        public float3 driftPos;
        public int isSceneStart;

        public float k;
        public float fric;
        public float force;
        public float3 mVelocity;
        public int bounceBool;
        public float maxDistance;
        //Calls for each entity
        public void Execute(ref Position position, ref Boid boid, ref Forcefield forcefield, ref Scale scale)
        {

            //Apply force on user input.
            if (isForceOn)
            {
               

                float3 f = forcefield.CastForce(ref mousePosition, ref boid, forceMode, maxDistance);
                ApplyForce(ref boid, f);
            }
            var initialPos = boid.initialPosition;
            var currentPos = position.Value;

            var driftTime = boid.driftTime;
            var timer = boid.timer;
            //if it's very small set the velocity to zero
            //saves unnecessary calculations
            if (math.length(boid.velocity) >= 0.05f) { 
                //apply friction, slows down over time
                ApplyForce(ref boid, CalculateFriction(forcefield.frictionCoe, ref boid));
                boid.isInitialStatus = 0;
                boid.isRunning = 1;
               // boid.isStop = 0;
                }
            else
            {
               // if (boid.isStop == 0 && boid.isInitialStatus == 0)
               // {
                    Stop(ref boid);
              //      boid.isStop = 1;
                    
                    //Return(ref boid);
              //  }
            }
            if (boid.isRunning == 1)
            {
                boid.timer += deltaTime;
            }

            if (boid.timer > boid.freeTime && boid.timer < boid.driftTime + boid.freeTime)
            {
                if (boid.isDriftPosGot == 1)
                {
                    boid.isReturning = 1;
                    boid.isRunning = 0;
                    boid.timer += deltaTime;
                    //change position - back to original pos
                    var ratio = (boid.timer - boid.freeTime) / boid.driftTime;
                    Vector3 vecPos = new Vector3(boid.position.x, boid.position.y, boid.position.z);
                    vecPos = Vector3.Lerp(boid.driftPosition, boid.initialPosition, ratio);
                    boid.position.x = vecPos.x;
                    boid.position.y = vecPos.y;
                    boid.position.z = vecPos.z;

                    //bounce back to original pos
                    //var f = (boid.initialPosition - boid.position) * k;
                    //mVelocity += f * Time.deltaTime;
                    //mVelocity = Vector3.Lerp(mVelocity, Vector3.zero, fric * Time.deltaTime);

                    //boid.position += mVelocity;

                    //scale lerp
                    Vector3 vecScale = new Vector3(scale.Value.x, scale.Value.y, scale.Value.z);
                    vecScale = Vector3.Lerp(boid.driftScale, new Vector3(7f, 7f, 7f), ratio);

                    scale.Value.x = vecScale.x;
                    scale.Value.y = vecScale.y;
                    scale.Value.z = vecScale.z;
                }
                else if (boid.isDriftPosGot == 0)
                {
                    boid.driftPosition = boid.position;
                    boid.driftScale = scale.Value;
                    //boid.isStop = 0;
                    boid.isDriftPosGot = 1;
                }
 
            }

            else if (boid.timer > boid.driftTime + boid.freeTime)
            {
                boid.timer = 0;
                boid.isReturning = 0;
                boid.position = boid.initialPosition;
                boid.velocity = 0;
                boid.isDriftPosGot = 0;
            }


          
            //boid.bounceTimer += deltaTime * bounceBool;



            //if (boid.bounceTimer >= 1)
            //{
            //    bounceBool = -1;
            //}
            //else if (boid.bounceTimer <= 0)
            //{
            //    bounceBool = 1;
            //}



            //var bounceRatio = boid.bounceTimer / boid.driftTime;

            //if (boid.id % 2 == 0)
            //{
            //    scale.Value.x = Mathf.Lerp(1, 3, bounceRatio);
            //    scale.Value.y = Mathf.Lerp(1, 3, bounceRatio);
            //}

            //else
            //{
            //    scale.Value.x = Mathf.Lerp(3, 1, bounceRatio);
            //    scale.Value.y = Mathf.Lerp(3, 1, bounceRatio);

            //}




            //if (boid.isStop == 1 && boid.isReturning == 0)
            //{
            //    boid.driftPosition = currentPos;
            //    boid.isStop = 0;
            //    boid.isReturning = 1;
            //}

            //if (boid.isReturning == 1)
            //{
            //    boid.timer += deltaTime;
            //    var ratio = timer / driftTime;
            //    Vector3 vecPos = new Vector3(boid.position.x, boid.position.y, boid.position.z);
            //    vecPos = Vector3.Lerp(freePos, initialPos, ratio);
            //    //boid.position.x = vecPos.x;
            //    //boid.position.y = vecPos.y;
            //    //boid.position.z = vecPos.z;
            //}

            //if (timer > boid.driftTime)
            //{
            //    boid.timer = 0;
            //    boid.isReturning = 0;
            //}







            //add acceleration
            boid.velocity += boid.acceration;
            

            //clip max velocity
            if (math.length(boid.velocity) > boid.maxLength)
            {
                boid.velocity = math.normalize(boid.velocity);
                boid.velocity *= boid.maxLength;
            }

            //check bound
            CheckEdge(ref forcefield, ref boid);

            //update position based on velocity
            boid.position += boid.velocity;


            if(boid.isRunning == 1 && math.length(mousePosition - boid.position) < 40)
            {
               // boid.position.z += 0.2f;
                var scaleOffset = 0.2f;
                scale.Value.x += scaleOffset;
                scale.Value.y += scaleOffset;
                scale.Value.z += scaleOffset;
            }
          

            //[transform.position = boid.position]
            position.Value = boid.position;

            //reset acceleration
            boid.acceration *= 0;

  
           

        }

        public void ApplyForce(ref Boid b, float3 force)
        {
            //F = ma formula
            b.acceration = b.acceration + (force / b.mass);
            
           
        }

        public void Stop(ref Boid b)
        {
            b.velocity *= 0;
         
        }

       

        float3 CalculateFriction(float coe, ref Boid b)
        {
            float3 friction = b.velocity;
            friction *= -1;
            friction = math.normalize(friction);
            friction *= coe;

            return friction;
        }

        //portal effect
        public void CheckEdge(ref Forcefield forcefield, ref Boid b)
        {
            if (forcefield.bound.z == 0) return;

            if (b.position.x > forcefield.bound.z)
            {
                b.position.x = 0;
            }
            else if (b.position.x < forcefield.bound.x)
            {
                b.position.x = forcefield.bound.z;
            }

            if (b.position.y > forcefield.bound.w)
            {
                b.position.y = 0;
            }
            else if (b.position.y < forcefield.bound.y)
            {
                b.position.y = forcefield.bound.w;
            }
        }
    }
    public const float epsilon_normal = 1E-30F;

    //remap a value in different range
    float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax, bool clamp)
    {

        if (math.abs(inputMin - inputMax) < epsilon_normal)
        {
            return outputMin;
        }
        else
        {
            float outVal = ((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin);

            if (clamp)
            {
                if (outputMax < outputMin)
                {
                    if (outVal < outputMax) outVal = outputMax;
                    else if (outVal > outputMin) outVal = outputMin;
                }
                else
                {
                    if (outVal > outputMax) outVal = outputMax;
                    else if (outVal < outputMin) outVal = outputMin;
                }
            }
            return outVal;
        }

    }
}
