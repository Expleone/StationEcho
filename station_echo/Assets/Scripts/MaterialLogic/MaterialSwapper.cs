using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialSwapper : MonoBehaviour
{
    [System.Serializable]
    public class NamedMaterial
    {
        public string name;
        public Material material;
    }

    public List<NamedMaterial> availableMaterials;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void SetMaterial(int slotIndex, Material newMaterial)
    {
        if (rend == null || newMaterial == null) return;

        // Копируем массив, чтобы не трогать sharedMaterials напрямую
        var mats = rend.materials;

        if (slotIndex >= 0 && slotIndex < mats.Length)
        {
            mats[slotIndex] = newMaterial;
            rend.materials = mats;
        }
        else
        {
            Debug.LogWarning($"Slot index {slotIndex} out of range for {gameObject.name}");
        }
    }

    public void SetMaterial(int slotIndex, string materialName)
    {
        var found = availableMaterials.Find(m => m.name == materialName);
        if (found != null)
        {
            SetMaterial(slotIndex, found.material);
        }
        else
        {
            Debug.LogWarning($"Material '{materialName}' not found in {gameObject.name}");
        }
    }

    public Material GetCurrentMaterial(int slotIndex)
    {
        if (rend == null) return null;
        if (slotIndex < 0 || slotIndex >= rend.materials.Length) return null;
        return rend.materials[slotIndex];
    }
}
