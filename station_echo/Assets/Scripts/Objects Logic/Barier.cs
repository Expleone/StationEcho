using System.Collections.Generic;
using UnityEngine;

public class Barier : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    List<GameObject> barierParts = new List<GameObject>();
    void Start()
    {
        foreach(Transform child in transform)
        {
            barierParts.Add(child.gameObject);
            #if UNITY_EDITOR
            child.GetComponent<Renderer>().enabled = false;
            #endif
        }
    }

    public void TurnOffBarier()
    {
        foreach(GameObject part in barierParts)
        {
            part.GetComponent<Collider>().enabled = false;
            #if UNITY_EDITOR
            part.GetComponent<Renderer>().enabled = false;
            #endif
        }
    }

    public void TurnOnBarier()
    {
        foreach (GameObject part in barierParts)
        {
            part.GetComponent<Collider>().enabled = true;
            #if UNITY_EDITOR
            part.GetComponent<Renderer>().enabled = true;
            #endif
        }
    }
}
