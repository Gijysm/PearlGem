using System.Collections.Generic;
using UnityEngine;

public class Spawn_Main_Spheres : MonoBehaviour
{
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private GameObject targetSpherePrefab;
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private Camera launchPoint;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int countSpheres = 8;

    private GameObject sphere;
    private Vector3 launchVelocity;
    private Vector3 viewportPos;
    private float holdTime;
    
    private static readonly Color[] ColorList = { Color.red, Color.green, Color.blue, Color.yellow };

    private void Start()
    {
        if (lineRenderer == null)
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

    private void Update()
    {
        DrawTrajectory();

        if (Input.GetMouseButton(0))
        {
            holdTime += Time.deltaTime;
        }
        else if (holdTime >= 1)
        {
            holdTime = 0;
        }

        if (sphere != null)
        {
            viewportPos = launchPoint.WorldToViewportPoint(sphere.transform.position);
            Vector3 screenPos = launchPoint.WorldToScreenPoint(sphere.transform.position);
            Debug.Log($"ViewportPos: {viewportPos}, ScreenPos: {screenPos}");

            if (!IsInViewport(viewportPos) || screenPos.y < -100)
            {
                Debug.Log("Видаляю сферу!");
                Destroy(sphere);
                sphere = null;
            }
        }


        if (holdTime < 1 && Input.GetMouseButtonUp(0))
        {
            SpawnSphere();
            holdTime = 0;
        }
    }

    private void SpawnSphere()
    {
        if (countSpheres <= 0) return;

        Vector3 spawnPosition = launchPoint.transform.position + launchPoint.transform.forward * 2f;
        sphere = Instantiate(spherePrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"Сфера спавнена на позиции: {spawnPosition}");

        var renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            var material = new Material(renderer.sharedMaterial.shader);
            int randomIndex = Random.Range(0, ColorList.Length);
            material.SetColor("_BaseColor", ColorList[randomIndex]);
            renderer.material = material;

            var mainSphere = sphere.GetComponent<Main_Sphere>();
            mainSphere?.SetColor(material);
        }

        var rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = launchVelocity;
            Debug.Log($"Сфера получила начальную скорость: {launchVelocity}");
        }

        countSpheres--;
    }

    private void DrawTrajectory()
    {
        launchVelocity = transform.forward * launchForce + Vector3.up * (launchForce / 2) * holdTime;

        Vector3[] trajectoryPositions = new Vector3[trajectoryPoints];
        Vector3 startPosition = launchPoint.transform.position;
        Vector3 currentVelocity = launchVelocity;
        Vector3 hitPosition = targetSpherePrefab.transform.position;
        bool reachedTarget = false;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * 0.1f;
            Vector3 newPosition = startPosition + currentVelocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryPositions[i] = newPosition;

            if (!reachedTarget && i > 0 && Physics.Raycast(trajectoryPositions[i - 1], newPosition - trajectoryPositions[i - 1], out RaycastHit hit, Vector3.Distance(trajectoryPositions[i - 1], newPosition)))
            {
                hitPosition = hit.point;
                reachedTarget = true;
                break;
            }
        }

        lineRenderer.positionCount = reachedTarget ? trajectoryPositions.Length : trajectoryPoints;
        lineRenderer.SetPositions(trajectoryPositions);
    }

    private bool IsInViewport(Vector3 position)
    {
        return position.x >= 0 && position.x <= 1 &&
               position.y >= 0 && position.y <= 1 &&
               position.z > 0;
    }
}
