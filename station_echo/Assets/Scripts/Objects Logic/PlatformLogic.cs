using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class PlatformLogic : MonoBehaviour, IDataPersistance
{
    [SerializeField] float speed;
    [SerializeField] float waitingTime; //time platform awaits at the waypoint
    [SerializeField] GameObject waypointPrefab;
    [SerializeField] GameObject PathMarkPrefab;
    private List<Transform> waypointTransforms = new List<Transform>();
    private Transform platformObjectTransform;
    private int waypointCount = 0;
    private int currentWaypoint = 0;
    private bool moving = false;
    public bool allowedToMove = true;
    private float currentWaitTime = 0;
    private Vector3 linearVelocity = new Vector3(0, 0, 0);
    private Vector3 linearVelocityGlobal = new Vector3(0, 0, 0);
    private Vector3 currentMovement = new Vector3(0, 0, 0);
    private Vector3 currentMovementGlobal = new Vector3(0, 0, 0);
    private Vector3 extraPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public Vector3 GetPropagationMovement()
    {
        return currentMovementGlobal;
    }
    public List<GameObject> passengers = new List<GameObject>();

    void Start()
    {
        GenerateGuid();
        if (transform.childCount == 0)
        {
            Debug.LogError("Critical Failure: Platform does not contain PlatformObject");
        }
        platformObjectTransform = transform.GetChild(0);

        // Get all waypoints transforms
        for (int i = 1; i < transform.childCount; ++i)
        {
            waypointTransforms.Add(transform.GetChild(i));
        }
        if (waypointTransforms.Count > 0)
        {   //Create a waypoint on the platform's local position
            if(extraPosition == new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
            {
                GameObject spawned = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity, transform); 
                spawned.transform.localPosition = platformObjectTransform.localPosition;
                spawned.GetComponent<AdvancedWaypointGizmos>().platformObjectTransform = platformObjectTransform;
                waypointTransforms.Add(spawned.transform);
                waypointCount = transform.childCount - 1;
                
                extraPosition = spawned.transform.localPosition;
            }
            else
            {
                GameObject spawned = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity, transform); 
                spawned.transform.localPosition = extraPosition;
                spawned.GetComponent<AdvancedWaypointGizmos>().platformObjectTransform = platformObjectTransform;
                waypointTransforms.Add(spawned.transform);
                waypointCount = transform.childCount - 1;
            }
            CreateMarkings();
        }
    }


    void CreateMarkings()
    {
        if(waypointCount == 2)
        {
            Transform first = waypointTransforms[0];
            Transform second = waypointTransforms[1];
            createMarking(first, second);
            return;
        }

        for(int i = 0; i < waypointTransforms.Count; ++i)
        {
            Transform first = waypointTransforms[i];
            Transform second;
            if(i == waypointTransforms.Count - 1)
            {
                second = waypointTransforms[0];
            }
            else
            {
                second = waypointTransforms[i+1];
            }
            createMarking(first, second);
        }
    }


    void createMarking(Transform first, Transform second)
    {
        GameObject spawned = Instantiate(PathMarkPrefab, Vector3.zero, Quaternion.identity, transform);
        float z = Vector3.Magnitude(first.transform.localPosition - second.transform.localPosition);
        spawned.transform.localScale = new Vector3(.1f, spawned.transform.localScale.y, z);
        spawned.transform.localPosition = (first.localPosition + second.localPosition) / 2;
        spawned.transform.LookAt(second);
    }


    void FixedUpdate()
    {
        if (waypointCount > 0 && !moving)
        {
            if (currentWaitTime >= waitingTime)
            {
                if (!allowedToMove)
                {
                    currentMovement = Vector3.zero;
                    currentMovementGlobal = Vector3.zero;
                    return;
                } 
                currentWaitTime = 0;
                calculateNewLinVel();
            }
            else currentWaitTime += Time.fixedDeltaTime;
        }
        else
        {
            if (!allowedToMove)
            {
                currentMovement = Vector3.zero;
                currentMovementGlobal = Vector3.zero;
                return;
            } 
            else if (hasArrived())  //Check if the platform has reached the waypoint
            {
                platformObjectTransform.localPosition = waypointTransforms[currentWaypoint].localPosition;
                moving = false;
                linearVelocity = new Vector3(0, 0, 0);
                currentMovement = new Vector3(0, 0, 0);

                linearVelocityGlobal = new Vector3(0, 0, 0);
                currentMovementGlobal = new Vector3(0, 0, 0);

                currentWaypoint = currentWaypoint + 1 >= waypointCount ? 0 : currentWaypoint + 1;
            }
            else
            {
                currentMovement = linearVelocity * speed * Time.fixedDeltaTime;
                platformObjectTransform.localPosition += currentMovement;

                currentMovementGlobal = linearVelocityGlobal * speed * Time.fixedDeltaTime;
            }
        }
    }
    

    private void calculateNewLinVel()
    {
        Vector3 a = waypointTransforms[currentWaypoint].localPosition;
        Vector3 b = platformObjectTransform.localPosition;
        Vector3 c = a - b;
        linearVelocity = new Vector3(c.x / c.magnitude, c.y / c.magnitude, c.z / c.magnitude);
        moving = true;

        a = waypointTransforms[currentWaypoint].position;
        b = platformObjectTransform.position;
        c = a - b;
        linearVelocityGlobal = new Vector3(c.x / c.magnitude, c.y / c.magnitude, c.z / c.magnitude);
    }


    private bool hasArrived()
    {
        Vector3 a = waypointTransforms[currentWaypoint].localPosition;

        Vector3 min = a - speed * linearVelocity * Time.fixedDeltaTime;
        Vector3 max = a + speed * linearVelocity * Time.fixedDeltaTime;
        if (min.x > max.x)
        {
            float i = max.x;
            max.x = min.x;
            min.x = i;
        } 
        if (min.y > max.y)
        {
            float i = max.y;
            max.y = min.y;
            min.y = i;
        } 
        if (min.z > max.z)
        {
            float i = max.z;
            max.z = min.z;
            min.z = i;
        } 

        Vector3 b = platformObjectTransform.localPosition;

        if ((b.x >= min.x && b.x <= max.x) && (b.y >= min.y && b.y <= max.y) && (b.z >= min.z && b.z <= max.z))
        {
            return true;
        }

        return false;
    }


    public void LoadData(GameData data, string levelId)
    {
        this.extraPosition = data.extraPositions[id];
        this.currentWaypoint = data.currentPlatformWaypoints[id];
    }

    public void SaveData(ref GameData data, string levelId)
    {
        data.extraPositions[id] = this.extraPosition;
        data.currentPlatformWaypoints[id] = this.currentWaypoint;
    }
}
