using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum WhatToDoOptions
    {
        OPEN_DOORS,
        CHANGE_GRAVITY,
        ACTIVATE_PLATFORM,
    }

    public enum ActivationMode
    {
        AND,
        OR,
	    XOR,
    }

    [Header("Activation Settings")]
    public WhatToDoOptions whatToDo = WhatToDoOptions.OPEN_DOORS;
    public ActivationMode activationMode = ActivationMode.OR;
    public Vector3 GravityChangerDirection = Vector3.up;

    [Header("Controlled Elements")]
    public List<Door> doors;
    public List<PlatformLogic> platforms;
    public List<PressurePlate> plates;
    public List<Switch> switches;
    public List<Button> buttons;

    private Vector3 gravityStatePast;

    private void Start()
    {
        if(whatToDo == WhatToDoOptions.OPEN_DOORS)              doors = new List<Door>(GetComponentsInChildren<Door>());
        else if(whatToDo == WhatToDoOptions.ACTIVATE_PLATFORM)  platforms = new List<PlatformLogic>(GetComponentsInChildren<PlatformLogic>());
        else if(whatToDo == WhatToDoOptions.CHANGE_GRAVITY)     gravityStatePast = Physics.gravity;

        plates = new List<PressurePlate>(GetComponentsInChildren<PressurePlate>());
        switches = new List<Switch>(GetComponentsInChildren<Switch>());
        buttons = new List<Button>(GetComponentsInChildren<Button>());

        foreach(var platform in platforms)
        {
            platform.allowedToMove = false;
        }
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

	
        bool flag = false; //shows whether we will open doors or change gravity
        switch(activationMode)
        {
            case ActivationMode.OR : flag = anyActive; 
            break;

            case ActivationMode.AND : flag = allActive;
            break;

            case ActivationMode.XOR : flag = !(moreThanOneActive || !anyActive);
            break;
        }

        switch(whatToDo)
        {
            case WhatToDoOptions.OPEN_DOORS :
                foreach (var door in doors)
                {
                    if (flag)
                    {
                        if (!door.IsOpen) door.Open();
                    }
                    else
                    {
                        if (door.IsOpen) door.Close();
                    }
                }
            break;
            
            case WhatToDoOptions.CHANGE_GRAVITY :
            changeGravity(flag);
            break;

            // Platforms
            case WhatToDoOptions.ACTIVATE_PLATFORM :
                foreach (var platform in platforms)
                {
                    if (flag)
                    {
                        if (!platform.allowedToMove) platform.allowedToMove = true;
                    }
                    else
                    {
                        if (platform.allowedToMove) platform.allowedToMove = false;
                    }
                }
            break;
        }
    }

    private bool gravityChanged = false;

    private void changeGravity(bool shouldChange)
    {
        if (shouldChange && !gravityChanged)
        {
            gravityStatePast = Physics.gravity;
            Physics.gravity = GravityChangerDirection;
            gravityChanged = true;
            Debug.Log("true");
        }
        else if (!shouldChange && gravityChanged)
        {
            Physics.gravity = gravityStatePast;
            gravityChanged = false;
            Debug.Log("false");
        }
    }
}
