using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLogic : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float waitingTime; //time platform awaits at the waypoint
    [SerializeField] GameObject waypointPrefab;
    private List<Transform> waypointTransforms = new List<Transform>();
    private Transform platformObjectTransform;
    private int waypointCount = 0;
    private int currentWaypoint = 0;
    private bool moving = false;
    private float currentWaitTime = 0;
    private Vector3 linearVelocity = new Vector3(0, 0, 0);

    void Start()
    {
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
        {
            GameObject spawned = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity, transform); //Cretae a waypoint on the platform's local position
            spawned.transform.localPosition = platformObjectTransform.localPosition;

            waypointTransforms.Add(spawned.transform);
            waypointCount = transform.childCount - 1;
        }
    }


    void Update()
    {
        if (waypointCount > 0 && !moving)
        {
            if (currentWaitTime >= waitingTime)
            {
                currentWaitTime = 0;
                calculateNewLinVel();
            }
            else currentWaitTime += Time.deltaTime;
        }
        else
        {
            if (hasArrived())  //Check if the platform has reached the waypoint
            {
                platformObjectTransform.localPosition = waypointTransforms[currentWaypoint].localPosition;
                moving = false;
                linearVelocity = new Vector3(0, 0, 0);
                currentWaypoint = currentWaypoint + 1 >= waypointCount ? 0 : currentWaypoint + 1;
            }
            else platformObjectTransform.localPosition += linearVelocity * speed * Time.deltaTime;
        }

    }


    void calculateNewLinVel()
    {
        Vector3 a = waypointTransforms[currentWaypoint].localPosition;
        Vector3 b = platformObjectTransform.localPosition;
        Vector3 c = a - b;
        linearVelocity = new Vector3(c.x / c.magnitude, c.y / c.magnitude, c.z / c.magnitude);
        moving = true;
    }


    bool hasArrived()
    {
        Vector3 a = waypointTransforms[currentWaypoint].localPosition;

        Vector3 min = a - speed * linearVelocity * Time.deltaTime;
        Vector3 max = a + speed * linearVelocity * Time.deltaTime;
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
}
