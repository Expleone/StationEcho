using UnityEngine;

public class AdvancedWaypointGizmos : MonoBehaviour
{
    public Transform platformObjectTransform;
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 

        if (platformObjectTransform != null)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, platformObjectTransform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, platformObjectTransform.localScale);
            Gizmos.matrix = oldMatrix;
        }
    }

    void Awake()
    {
        this.transform.GetComponent<Renderer>().enabled = false;
    }
}
