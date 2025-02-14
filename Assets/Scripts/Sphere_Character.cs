using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere_Character : MonoBehaviour
{
    private Renderer rend;
    private Color sphereColor;
    [SerializeField] private ParticleSystem particlePrefab;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend.material.HasProperty(BaseColorID)) 
        {
            sphereColor = rend.material.GetColor(BaseColorID);
        }
    }
    
    public Color GetColor()
    {
        return sphereColor;
    }

    public void SetColor(Material material)
    {
        rend.material = material;
        if (rend.material.HasProperty(BaseColorID))
        {
            sphereColor = rend.material.GetColor(BaseColorID);
        }
    }

    public bool CheckCollision()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.4f);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && col.CompareTag("SpawnedSphere"))
            {
                Debug.Log($"Collided with {col.gameObject.name}");
                Main_Sphere otherSphere = col.GetComponent<Main_Sphere>(); 

                if (otherSphere == null)
                {
                    continue;
                }

                string thisColor = ColorUtility.ToHtmlStringRGBA(sphereColor);
                string otherColor = ColorUtility.ToHtmlStringRGBA(otherSphere.GetMainSphereColor());
                ParticleSystem particle = Instantiate(particlePrefab);
                particle.Play();
                if (thisColor == otherColor)
                {
                    Destroy(gameObject);
                }
            }
        }
        return false;
    }
    
}