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
    [SerializeField] private float closeDelay = 3.0f;

    private bool isDispensing = false;

    void Start()
    {
        if (objectInstance != null)
        {
            itemPrefab = objectInstance;
            rbInstance = itemPrefab.GetComponent<Rigidbody>();
        }
        initialDispensePointPosition = dispensePoint.localPosition;
        panel1.UseTimeBasedMovement = true;
        panel2.UseTimeBasedMovement = true;
        panel1.OpenTime = 1.0f;
        panel2.OpenTime = 1.0f;
    }

    public void DispenseItem()
    {
        if (isDispensing) return;
        isDispensing = true;
        if (itemPrefab == null) 
        {
            Debug.LogWarning("No item prefab assigned to Dispenser.");
            return;
        }
        if(objectInstance != null)
        {
            StartCoroutine(KillObject(objectInstance));
        }
        
        if(itemPrefab != null)
        {
            objectInstance = Instantiate(itemPrefab, dispensePoint.position, Quaternion.identity);
            rbInstance = objectInstance.GetComponent<Rigidbody>();
            rbInstance.useGravity = false;
        }

        StartCoroutine(MovePanels());
        
        
    }

    private System.Collections.IEnumerator KillObject(GameObject target)
    {
        // 1. Disable the collider first
        Collider col = target.GetComponent<Collider>();
        if(col != null) 
        {
            col.enabled = false; // This forces OnTriggerExit to fire immediately
        }
        yield return new WaitForSeconds(0.2f); // Wait for one frame to ensure OnTriggerExit is processed

        // 2. Then destroy the object
        Destroy(target);
    }

    private System.Collections.IEnumerator MovePanels()
    {
        yield return new WaitForSeconds(openDelay);
        panel1.Open();
        panel2.Open();

        yield return new WaitForSeconds(panel1.OpenTime);
        cubePlatform.Open();
        MoveDispensePoint(new Vector3(0, 1.5f, 0));


        yield return new WaitForSeconds(closeDelay);
        cubePlatform.Close();

        yield return new WaitForSeconds(0.5f);
        panel1.Close();
        panel2.Close();

        yield return new WaitForSeconds(panel1.OpenTime);
        rbInstance.useGravity = true; // Enable gravity after dispensing
        isDispensing = false;
        dispensePoint.localPosition = initialDispensePointPosition;
    }

    private void MoveDispensePoint(Vector3 moveVector)
    {
        if(objectInstance != null)
        {
            StartCoroutine(MovePos(dispensePoint, dispensePoint.localPosition, dispensePoint.localPosition + moveVector, 1.0f));
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

    void FixedUpdate()
    {
        if (objectInstance != null && rbInstance != null && isDispensing)
        {
            rbInstance.AddForce(5 * (dispensePoint.position - rbInstance.position), ForceMode.Force);
        }
    }

    void Update()
    {
        if (objectInstance == null || rbInstance == null){
            DispenseItem();
        }
    }
}
