using UnityEngine;
using System;
public class PerformanceKiller : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    [SerializeField, Range(1, 100)] int resolution = 50; // 2500 points
    private Transform[] points;

    void Update()
    {
        if (!enabled) return; // Skip if component is disabled

        float time = Time.time;
        float step = 2f / resolution;

        // Recreate all points every frame (massive memory and CPU hit)
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab); // Excessive instantiation
            points[i] = point;
            point.SetParent(transform, false);
            point.localScale = Vector3.one * step;
            point.gameObject.AddComponent<Rigidbody>(); // Physics overhead
        }

        // Heavy loop with redundant calculations
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            float u = (x + 0.5f) * step - 1f;
            float v = (z + 0.5f) * step - 1f;

            // Expensive nested operations
            Vector3 pos = new Vector3(u, 0, v);
            for (int j = 0; j < 100; j++) // Pointless nested loop
            {
                pos.y += Mathf.Sin(Mathf.Cos(Mathf.Tan(u + v + time))); // Heavy math
                Debug.Log("Point " + i + " at " + pos.ToString()); // String garbage
            }

            // Unoptimized transform operations
            points[i].localPosition = pos;
            points[i].Rotate(Vector3.up, time * 90f); // Extra transform cost
        }
    }

    // Clean up when disabled (optional)
    void OnDisable()
    {
        if (points != null)
        {
            foreach (Transform point in points)
            {
                if (point != null) Destroy(point.gameObject);
            }
        }
    }
}
