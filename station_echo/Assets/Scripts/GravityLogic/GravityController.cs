using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public enum ActivationMode
    {
        AND,
        OR,
	XOR
    }

    [Header("Activation Settings")]
    public ActivationMode activationMode = ActivationMode.OR;

    [Header("Controlled Elements")]
    public List<Door> doors;
    public List<PressurePlate> plates;
    public List<Switch> switches;
    public List<Button> buttons;

    private void Awake()
    {
        doors = new List<Door>(GetComponentsInChildren<Door>());
        plates = new List<PressurePlate>(GetComponentsInChildren<PressurePlate>());
        switches = new List<Switch>(GetComponentsInChildren<Switch>());
        buttons = new List<Button>(GetComponentsInChildren<Button>());
    }

    private void Update()
    {
        bool allActive = true;
        bool anyActive = false;
	bool moreThanOneActive = false;

        foreach (var plate in plates)
        {
            if (plate != null)
            {
                if (plate.IsPressed)
		{ 
		    if(anyActive) moreThanOneActive = true;
		    anyActive = true;
		}
                else allActive = false;
            }
        }

        foreach (var sw in switches)
        {
            if (sw != null)
            {
                if (sw.IsOn)
		{ 
		    if(anyActive) moreThanOneActive = true;
		    anyActive = true;
		}
                else allActive = false;
            }
        }

        foreach (var btn in buttons)
        {
            if (btn != null)
            {
                if (btn.IsPressed)
		{ 
		    if(anyActive) moreThanOneActive = true;
		    anyActive = true;
		}
                else allActive = false;
            }
        }

        bool shouldOpen = false;
	switch(activationMode)
	{
	    case ActivationMode.OR : shouldOpen = anyActive; 
	    break;

	    case ActivationMode.AND : shouldOpen = allActive;
	    break;

	    case ActivationMode.XOR : shouldOpen = !(moreThanOneActive || !anyActive);
	    break;
	}

        foreach (var door in doors)
        {
            if (shouldOpen)
            {
                if (!door.IsOpen) door.Open();
            }
            else
            {
                if (door.IsOpen) door.Close();
            }
        }
    }
}
