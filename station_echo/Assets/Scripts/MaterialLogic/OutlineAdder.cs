using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OutlineAdder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Material outlineMaterial;
    public void ApplyOutline(Transform target)
    {
        if (target == null) return;
        Renderer render = target.GetComponent<Renderer>();

        if(render == null) render = target.GetComponentInChildren<Renderer>();
        
        if (render != null)
        {
            List<Material> materials = render.materials.ToList();

            if (materials.Count > 0 && materials[materials.Count - 1].name.StartsWith(outlineMaterial.name))
            {
                return; 
            }

            materials.Add(outlineMaterial);
            render.materials = materials.ToArray();
        }
    }

    public void RemoveOutline(Transform target)
    {
        if (target == null) return;
        Renderer render = target.GetComponent<Renderer>();
        if(render == null) render = target.GetComponentInChildren<Renderer>();

        if (render != null)
        {
            List<Material> materials = render.materials.ToList();

            if (materials.Count > 1 && materials[materials.Count - 1].name.StartsWith(outlineMaterial.name)) 
            {
                materials.RemoveAt(materials.Count - 1);
                render.materials = materials.ToArray();
            }
        }
    }

    public void DeleteOutlineWithDelay(Transform target, float delay)
    {
        StartCoroutine(DeleteOutlineWithDelayCoroutine(target, delay));
    }
    public System.Collections.IEnumerator DeleteOutlineWithDelayCoroutine(Transform target, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveOutline(target);
    }
}
