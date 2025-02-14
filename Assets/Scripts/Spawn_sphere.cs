using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_sphere : MonoBehaviour
{
    private Color[] color_list = { Color.red, Color.green, Color.blue, Color.yellow };
    [SerializeField] GameObject sphere;
    [SerializeField] private int totalSpheres = 100;
    private Dictionary<Color, List<GameObject>> Spheres = new Dictionary<Color, List<GameObject>>();

    void Start()
    {
        SpawnSphere();
    }

    void SpawnSphere()
    {
        float radius = GetComponent<SphereCollider>().radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Shader shader = sphere.GetComponent<Renderer>().sharedMaterial.shader;
        for (int i = 0; i < totalSpheres; i++)
        {
            float theta = Mathf.Acos(1 - 2 * (i + 0.5f) / totalSpheres);
            float phi = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;

            Vector3 pos = transform.position + new Vector3(
                radius * Mathf.Sin(theta) * Mathf.Cos(phi),
                radius * Mathf.Cos(theta),
                radius * Mathf.Sin(theta) * Mathf.Sin(phi)
            );

            GameObject newSphere = Instantiate(sphere, pos, Quaternion.identity);
            newSphere.transform.SetParent(transform);

            Color rowColor = color_list[i % color_list.Length];
            Material newMaterial = new Material(shader);
            
            if (newMaterial.HasProperty("_BaseColor"))
                newMaterial.SetColor("_BaseColor", rowColor);
            else if (newMaterial.HasProperty("_Color"))
                newMaterial.SetColor("_Color", rowColor);
            
            newSphere.GetComponent<Renderer>().material = newMaterial;

            Sphere_Character sphereCharacter = newSphere.GetComponent<Sphere_Character>();
            if (sphereCharacter != null)
            {
                sphereCharacter.SetColor(newMaterial);
            }

            if (!Spheres.ContainsKey(rowColor))
            {
                Spheres[rowColor] = new List<GameObject>();
            }
            Spheres[rowColor].Add(newSphere);
        }
    }

    private void Update()
    {
        List<Color> colorsToRemove = new List<Color>();

        foreach (var kvp in Spheres)
        {
            foreach (var sphere in kvp.Value)
            {
                if (sphere == null)
                {
                    continue;
                }

                Sphere_Character sphereCharacter = sphere.GetComponent<Sphere_Character>();
                if (sphereCharacter != null && sphereCharacter.CheckCollision())
                {
                    colorsToRemove.Add(kvp.Key);
                    break;
                }
            }
        }


        foreach (Color color in colorsToRemove)
        {
            if (Spheres.ContainsKey(color))
            {

                foreach (GameObject sphere in Spheres[color])
                {
                    if (sphere != null)
                    {
                        Destroy(sphere);
                    }
                }
                Spheres.Remove(color);
            }
        }

    }

}
