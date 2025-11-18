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
    public bool allowedToMove = true;
    private float currentWaitTime = 0;
    private Vector3 linearVelocity = new Vector3(0, 0, 0);
    private Vector3 currentMovement = new Vector3(0, 0, 0);
    public Vector3 GetPropagationMovement()
    {
        return currentMovement;
    }
    public List<GameObject> passengers = new List<GameObject>();

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
            // Disable mesh renderer of waypoints
            Renderer renderer = transform.GetChild(i).GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
        if (waypointTransforms.Count > 0)
        {   //Create a waypoint on the platform's local position
            GameObject spawned = Instantiate(waypointPrefab, Vector3.zero, Quaternion.identity, transform); 
            spawned.transform.localPosition = platformObjectTransform.localPosition;
            spawned.GetComponent<AdvancedWaypointGizmos>().platformObjectTransform = platformObjectTransform;

            waypointTransforms.Add(spawned.transform);
            waypointCount = transform.childCount - 1;
        }
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
                return;
            } 
            else if (hasArrived())  //Check if the platform has reached the waypoint
            {
                platformObjectTransform.localPosition = waypointTransforms[currentWaypoint].localPosition;
                moving = false;
                linearVelocity = new Vector3(0, 0, 0);
                currentMovement = new Vector3(0, 0, 0);

                currentWaypoint = currentWaypoint + 1 >= waypointCount ? 0 : currentWaypoint + 1;
            }
            else
            {
                currentMovement = linearVelocity * speed * Time.fixedDeltaTime;
                platformObjectTransform.localPosition += currentMovement;
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
}
