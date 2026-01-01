using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Trampolino : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 bounceVector = new Vector3(0, 15f, 0);
    public bool isActive = true;

    List<GameObject> bouncedObjects = new List<GameObject>();
    MaterialSwapper swapper;
    
    void Start()
    {
        swapper = GetComponentInChildren<MaterialSwapper>();
        if(swapper != null){
            if(isActive){
                swapper.SetMaterial(0, "active");
            } else {
                swapper.SetMaterial(0, "inactive");
            }
        }
    }

    public void SetActive(bool active){
        isActive = active;
        if(swapper != null){
            if(isActive){
                swapper.SetMaterial(0, "active");
            } else {
                swapper.SetMaterial(0, "inactive");
            }
        }
    }

    void FixedUpdate(){
        bouncedObjects.RemoveAll(obj => obj == null);
        if(!isActive) return;
        foreach(var obj in bouncedObjects){
            print("Bouncing " + obj.name);
            var playerMovement = obj.GetComponent<ThirdPersonMovement>();
            if(playerMovement != null){
                playerMovement.AddVelocity(bounceVector);
            }
            var rb = obj.GetComponent<Rigidbody>();
            if(rb != null){
                // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(bounceVector, ForceMode.VelocityChange);
            }
        }
        bouncedObjects.Clear();
    }

    void OnTriggerEnter(Collider other){
        print("Trampolino triggered by " + other.gameObject.name);
        StartCoroutine(AddToBouncedObjects(other));
    }
    
    void OnTriggerExit(Collider other){
        bouncedObjects.Remove(other.gameObject);
    }

    private System.Collections.IEnumerator AddToBouncedObjects(Collider other){
        yield return new WaitForSecondsRealtime(0.1f);
        bouncedObjects.Add(other.gameObject);
    }
}
