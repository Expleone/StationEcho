using UnityEngine;

public class AdvancedWaypointGizmos : MonoBehaviour
{
    public Transform platformObjectTransform;
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; 

        if (platformObjectTransform != null)
        {
            Gizmos.DrawWireCube(transform.position, platformObjectTransform.localScale);
        }
    }
}
