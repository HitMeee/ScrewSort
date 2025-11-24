using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrewBase : MonoBehaviour
{
    public int id;
    public List<Material> materials;
    public MeshRenderer meshRenderer;

    
    public void Init(int id)
    {
        this.id = id;
        meshRenderer.material = GetMaterials(id);
    }

    public Material GetMaterials(int id)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            if (i == id)
            {
                return materials[i];
            }
        }
        return null;
    }
}