using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public GameObject itemPrefab;

    public GameObject objectInstance;
    private Rigidbody rbInstance;

    [SerializeField] private Door panel1, panel2;

    [SerializeField] private Door cubePlatform;

    [SerializeField] private Transform dispensePoint;
    private Vector3 initialDispensePointPosition;
    [SerializeField] private float openDelay = 0.1f;
    [SerializeField] private float closeDelay = 1f;

    [SerializeField] private Barier barier;
    [SerializeField] private Vector3 CheckZoneSize = new Vector3(2f, 3f, 2f);
    [SerializeField] private bool hasBody = true;
    private List<GameObject> allChildren = new List<GameObject>();

    private bool isDispensing = false;
    private bool tryingToDispense = false;

    private string activeMaterial = "active";
    private string inactiveMaterial = "inactive";

    private bool ftSpawn = false;
    private string cubeId;

    [SerializeField] private MaterialSwapper materialSwapper;

    // #if UNITY_EDITOR
    private OutlineAdder outlineAdder;
    List<GameObject> outlinedObjects = new List<GameObject>();
    // #endif

    public float GetDispenseDelay()
    {
        return openDelay + panel1.OpenTime + closeDelay + panel1.OpenTime + 0.5f;
    }
    void Start()
    {

        if (itemPrefab == null)
        {
            Debug.LogWarning("No item prefab assigned to Dispenser.");
        }
        if (objectInstance != null)
        {
            rbInstance = objectInstance.GetComponent<Rigidbody>();
            MovableObjectData cubeData = objectInstance.GetComponent<MovableObjectData>();
            cubeData.SetDispenser(this);
            cubeId = cubeData.GetId();
        }
        else
        {
            Debug.Log("spawning cube for the first time");
            ftSpawn = true;
        }
        initialDispensePointPosition = dispensePoint.localPosition;
        panel1.UseTimeBasedMovement = true;
        panel2.UseTimeBasedMovement = true;
        panel1.OpenTime = 1.0f;
        panel2.OpenTime = 1.0f;

        foreach (Transform child in transform)
        {
            allChildren.Add(child.gameObject);

        }
        // #if UNITY_EDITOR
        outlineAdder = GetComponent<OutlineAdder>();
        // #endif
    }

    public bool DispenseItem()
    {
        if (isDispensing || !IsDispancePlaceClear())
        {

            // #if UNITY_EDITOR
            if (!tryingToDispense)
            {
                foreach (var outlinedObject in outlinedObjects)
                {
                    if (outlinedObject != null) outlineAdder.DeleteOutlineWithDelay(outlinedObject.transform, 1f);
                }
                outlinedObjects.Clear();
            }
            // #endif
            return false;
        }
        isDispensing = true;
        tryingToDispense = false;

        if (itemPrefab == null)
        {
            Debug.LogWarning("No item prefab assigned to Dispenser.");
            return false;
        }

        string id = string.Empty;
        if (objectInstance != null)
        {
            Destroy(objectInstance);
        }

        if (itemPrefab != null)
        {
            objectInstance = Instantiate(itemPrefab, dispensePoint.position, Quaternion.identity);
            if (ftSpawn)
            {
                StartCoroutine(GetCubeID());
            }
            else
            {
                MovableObjectData cubeData = objectInstance.GetComponent<MovableObjectData>();
                cubeData.SetId(cubeId);
                cubeData.SetDispenser(this);
                Debug.Log("ID was set to: " + cubeData.GetId());
            }

            rbInstance = objectInstance.GetComponent<Rigidbody>();
            if (hasBody)
                rbInstance.useGravity = false;
        }
        if (hasBody)
        {
            StartCoroutine(MovePanels());
        }
        else
        {
            isDispensing = false;
        }
            
        return true;

    }

    private System.Collections.IEnumerator KillObject(GameObject target)
    {
        // 1. Disable the collider first
        Collider col = target.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false; // This forces OnTriggerExit to fire immediately
        }
        yield return new WaitForSeconds(0.2f); // Wait for one frame to ensure OnTriggerExit is processed

        // 2. Then destroy the object
        Destroy(target);
    }

    private System.Collections.IEnumerator MovePanels()
    {
        materialSwapper.SetMaterial(0, activeMaterial);
        barier.TurnOnBarier();

        yield return new WaitForSeconds(openDelay);
        panel1.Open();
        panel2.Open();

        yield return new WaitForSeconds(panel1.OpenTime);
        // cubePlatform.Open();
        MoveDispensePoint(new Vector3(0, 1f, 0));


        yield return new WaitForSeconds(closeDelay);
        // cubePlatform.Close();

        yield return new WaitForSeconds(0.5f);
        panel1.Close();
        panel2.Close();

        yield return new WaitForSeconds(panel1.OpenTime);
        barier.TurnOffBarier();
        materialSwapper.SetMaterial(0, inactiveMaterial);
        rbInstance.useGravity = true; // Enable gravity after dispensing
        isDispensing = false;
        dispensePoint.localPosition = initialDispensePointPosition;
    }

    private void MoveDispensePoint(Vector3 moveVector)
    {
        if (objectInstance != null)
        {
            StartCoroutine(MovePos(dispensePoint, dispensePoint.localPosition, dispensePoint.localPosition + moveVector, 0.5f));
        }
    }

    private System.Collections.IEnumerator MovePos(Transform objectTransform, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            objectTransform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        objectTransform.localPosition = to;
    }
    private System.Collections.IEnumerator GetCubeID()
    {
        yield return null;
        MovableObjectData cubeData = objectInstance.GetComponent<MovableObjectData>();
        cubeId = cubeData.GetId();
        cubeData.SetDispenser(this);
        Debug.Log("Got ID: " + cubeId);
        ftSpawn = false;
    }


    void FixedUpdate()
    {
        if (objectInstance != null && rbInstance != null && isDispensing && hasBody)
        {
            rbInstance.AddForce(1f * (dispensePoint.position - rbInstance.position), ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (objectInstance == null || rbInstance == null)
        {
            tryingToDispense = true;
            DispenseItem();
        }
    }

    bool IsDispancePlaceClear()
    {
        // 1. DEFINITIONS
        // If you want a 2x3x2 box, your half extents are 1x1.5x1
        Vector3 boxSize = CheckZoneSize; // Adjust for scale
        Vector3 halfExtents = boxSize / 2f;

        // Determine the center. (Assuming you want to check BELOW the dispenser?)
        // If you actually want UP, change Vector3.down to Vector3.up
        Vector3 center = transform.position + (Vector3.up * halfExtents.y);
        Quaternion orientation = transform.rotation;

        // 2. CLEAR PREVIOUS OUTLINES (Editor Only)
        if (tryingToDispense)
        {
            foreach (var outlinedObject in outlinedObjects)
            {
                if (outlinedObject != null) outlineAdder.RemoveOutline(outlinedObject.transform);
            }
            outlinedObjects.Clear();
        }

        // 3. THE PHYSICS CHECK
        // Added 'orientation' so the box rotates with the dispenser
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, orientation);

        bool isBlocked = false;

        foreach (var hitCollider in hitColliders)
        {
            // Ignore self and children
            if (hitCollider.gameObject != this.gameObject && !allChildren.Contains(hitCollider.gameObject) && hitCollider.gameObject != objectInstance)
            {
                isBlocked = true;

                // #if UNITY_EDITOR
                if (!outlinedObjects.Contains(hitCollider.gameObject))
                {
                    outlineAdder.ApplyOutline(hitCollider.transform);
                    outlinedObjects.Add(hitCollider.gameObject);
                }
                // We don't return false immediately here so we can outline ALL blocking objects, 
                // but if you only care about the first one, you can return false here.
                // #endif

                print("Dispenser blocked by " + hitCollider.gameObject.name);
            }
        }

        return !isBlocked;
    }


    void OnDrawGizmos()
    {
        if (dispensePoint == null) return;

        Gizmos.color = Color.red;

        // 1. Setup the Matrix so the Gizmo rotates with the object
        // Assuming the check is "Down" relative to the dispense point
        Vector3 boxSize = CheckZoneSize; // Adjust for scale
        Vector3 center = transform.position + (Vector3.up * (boxSize.y / 2));

        // This allows the Gizmo to match the OverlapBox rotation
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        // 2. Draw the Cube (Pass Vector3.zero because the matrix handles the position)
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // Optional: Draw a faint fill to see volume
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(Vector3.zero, boxSize);
    }

    public void DispenseItemAt(string Id, Vector3 pos)
    {
        Debug.Log("dispensed item with id " + Id + " at " + pos);
        if (objectInstance != null)
        {
            Destroy(objectInstance);
        }
        objectInstance = Instantiate(itemPrefab, pos, Quaternion.identity);
        MovableObjectData cubeData = objectInstance.GetComponent<MovableObjectData>();
        cubeId = Id;
        cubeData.SetId(Id);
        cubeData.SetDispenser(this);
        Debug.Log("ID was set to: " + cubeData.GetId());
        rbInstance = objectInstance.GetComponent<Rigidbody>();
    }
}
