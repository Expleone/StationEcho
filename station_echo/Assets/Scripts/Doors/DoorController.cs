using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Door door;
    public List<PressurePlate> keys;

    private void Awake()
    {
        keys = new List<PressurePlate>(GetComponentsInChildren<PressurePlate>());
        door = GetComponentInChildren<Door>();
    }

    private void Update()
    {
        if (keys != null && keys.Count > 0)
        {
            foreach (var key in keys)
            {
                if (!key.IsPressed)
                {
                    if (door.IsOpen) door.Close();
                    return;
                }
            }
            if (!door.IsOpen) door.Open();
        }
    }
}