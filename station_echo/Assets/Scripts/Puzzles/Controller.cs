using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum WhatToDoOptions
    {
        OPEN_DOORS,
        CHANGE_GRAVITY,
        ACTIVATE_PLATFORM,
        DISPENSE_ITEM,
        ACTIVATE_TRAMPOLINO,
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
    public List<Dispenser> dispensers;
    public List<Trampolino> trampolinos;

    public Vector3 gravityStatePast;
    private bool objectDispensed = false;
    public bool gravityChanged = false;

    private void Start()
    {
        if (whatToDo == WhatToDoOptions.OPEN_DOORS) doors = new List<Door>(GetComponentsInChildren<Door>());
        else if (whatToDo == WhatToDoOptions.ACTIVATE_PLATFORM) platforms = new List<PlatformLogic>(GetComponentsInChildren<PlatformLogic>());
        else if (whatToDo == WhatToDoOptions.CHANGE_GRAVITY) gravityStatePast = Physics.gravity;
        else if (whatToDo == WhatToDoOptions.DISPENSE_ITEM) dispensers = new List<Dispenser>(GetComponentsInChildren<Dispenser>());
        else if (whatToDo == WhatToDoOptions.ACTIVATE_TRAMPOLINO) trampolinos = new List<Trampolino>(GetComponentsInChildren<Trampolino>());

        plates = new List<PressurePlate>(GetComponentsInChildren<PressurePlate>());
        switches = new List<Switch>(GetComponentsInChildren<Switch>());
        buttons = new List<Button>(GetComponentsInChildren<Button>());

        foreach (var platform in platforms)
        {
            platform.allowedToMove = false;
        }
        if (whatToDo == WhatToDoOptions.DISPENSE_ITEM)
        {
            foreach (var button in buttons)
            {
                button.ActiveTime = dispensers[0].GetDispenseDelay();
            }
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
                    if (anyActive) moreThanOneActive = true;
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
                    if (anyActive) moreThanOneActive = true;
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
                    if (anyActive) moreThanOneActive = true;
                    anyActive = true;
                }
                else allActive = false;
            }
        }


        bool flag = false; //shows whether we will open doors or change gravity
        switch (activationMode)
        {
            case ActivationMode.OR:
                flag = anyActive;
                break;

            case ActivationMode.AND:
                flag = allActive;
                break;

            case ActivationMode.XOR:
                flag = !(moreThanOneActive || !anyActive);
                break;
        }

        if (!flag) objectDispensed = false;

        switch (whatToDo)
        {
            case WhatToDoOptions.OPEN_DOORS:
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

            case WhatToDoOptions.CHANGE_GRAVITY:
                changeGravity(flag);
                break;

            // Platforms
            case WhatToDoOptions.ACTIVATE_PLATFORM:
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

            case WhatToDoOptions.ACTIVATE_TRAMPOLINO:
                foreach (var trampolino in trampolinos)
                {
                    if (flag)
                    {
                        if (!trampolino.isActive) trampolino.SetActive(true);
                    }
                    else
                    {
                        if (trampolino.isActive) trampolino.SetActive(false);
                    }
                }
                break;

            case WhatToDoOptions.DISPENSE_ITEM:
                foreach (var dispenser in dispensers)
                {
                    if (flag && !objectDispensed)
                    {
                        if (!dispenser.DispenseItem())
                        {
                            foreach (var button in buttons)
                            {
                                StartCoroutine(unpressButtonsAfterDelay(1f));
                            }
                        }
                        objectDispensed = true;
                    }
                }
                break;
        }
    }

    private System.Collections.IEnumerator unpressButtonsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var btn in buttons)
        {
            btn.Unpress();
        }
    }

    private void changeGravity(bool shouldChange)
    {
        // Debug.Log("Should change: " + shouldChange + " gravityChanged: " + gravityChanged);
        if (shouldChange && !gravityChanged)
        {
            // gravityStatePast = Physics.gravity;
            Physics.gravity = -Physics.gravity;
            gravityChanged = true;
            //Debug.Log("true");
        }
        else if (!shouldChange && gravityChanged)
        {
            Physics.gravity = -Physics.gravity;
            gravityChanged = false;
            //Debug.Log("false");
        }
    }
}
