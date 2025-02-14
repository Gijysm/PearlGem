using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawn_Main_Spheres : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject TospherePrefab;
    [SerializeField] private int trajectoryPoints = 30; 
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private LineRenderer lineRenderer;
    private Color[] color_list = { Color.red, Color.green, Color.blue, Color.yellow };
    [SerializeField] private int Count_spheres = 8;
    
    private Vector3 launchVelocity;
    
    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = trajectoryPoints;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.yellow;
        }
    }

    private double holdTime = 0;
    void Update()
    {
        DrawTrajectory();

        if (Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;
        }
        else if(holdTime >= 1)
        {
            holdTime = 0;
        }

        if (holdTime < 1 && Input.GetMouseButtonUp(0))
        {
            SpawnSphere();
            holdTime = 0;
        }

    }

    void SpawnSphere()
    {
        if (Count_spheres > 0){ 
        GameObject sphere = Instantiate(spherePrefab,
        new Vector3(launchPoint.position.x, launchPoint.position.y, launchPoint.position.z),
        Quaternion.identity);
        Shader shader = sphere.GetComponent<Renderer>().sharedMaterial.shader;
        Material material = new Material(shader);
        Main_Sphere main_sphere = sphere.GetComponent<Main_Sphere>();
        int random = Random.Range(0, 4);
        material.SetColor("_BaseColor", color_list[random]);
        sphere.GetComponent<Renderer>().material = material;
        main_sphere.SetColor(material);
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = launchVelocity;
        }

        Count_spheres--;
        }
}

    void DrawTrajectory()
    {
        launchVelocity = (transform.forward * launchForce) + (Vector3.up * (launchForce / 2) * (float)holdTime);

        Vector3[] trajectoryPositions = new Vector3[trajectoryPoints];

        Vector3 startPosition = launchPoint.position;
        Vector3 currentVelocity = launchVelocity;
        Vector3 endPoint = TospherePrefab.transform.position;
        bool reachedTarget = false;
        Vector3 hitPosition = TospherePrefab.transform.position;
        
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector3 newPosition = startPosition + (currentVelocity * time) + (0.5f * Physics.gravity * time * time);

            trajectoryPositions[i] = newPosition;
            
            if (!reachedTarget && Physics.Raycast(trajectoryPositions[Mathf.Max(0, i - 1)], newPosition - trajectoryPositions[Mathf.Max(0, i - 1)], out RaycastHit hit, Vector3.Distance(trajectoryPositions[Mathf.Max(0, i - 1)], newPosition)))
            {
                hitPosition = hit.point; 
                reachedTarget = true;
                break; 
            }
        }

        lineRenderer.positionCount = reachedTarget ? trajectoryPositions.Length : trajectoryPoints;
        lineRenderer.SetPositions(trajectoryPositions);
    }
}