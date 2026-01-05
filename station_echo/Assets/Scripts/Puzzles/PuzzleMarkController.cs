using UnityEngine;
using System.Collections.Generic;

public class PuzzleMarkController : MonoBehaviour
{
    public List<MaterialSwapper> childObjects = new List<MaterialSwapper>();

    void Start()
    {
        GetChildren();
    }

    void GetChildren()
    {
        foreach(Transform child in transform)
        {
            MaterialSwapper swapper = child.GetComponent<MaterialSwapper>();
            if(swapper)
            {
                childObjects.Add(swapper);
            }
        }
    }
}