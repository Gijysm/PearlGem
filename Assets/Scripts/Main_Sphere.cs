using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Sphere : MonoBehaviour
{
    private Renderer rend;
    private Color sphereColor;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend.material.HasProperty(BaseColorID)) 
        {
            sphereColor = rend.material.GetColor(BaseColorID);
        }
        else
        {
            Debug.LogError("Shader does not have a '_BaseColor' property!");
        }
    }
    void Start()
    {
        if (this.GetComponent<Main_Sphere>() == null)
        {
            Debug.LogError($"{this.name} does NOT have Spawn_Main_Spheres! Adding manually...");
            this.gameObject.AddComponent<Spawn_Main_Spheres>();
        }
    }
    public void SetColor(Material material)
    {
        rend.material = material;
        if (rend.material.HasProperty(BaseColorID))
        {
            sphereColor = rend.material.GetColor(BaseColorID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Color GetMainSphereColor()
    {
        return sphereColor;
    }
}
