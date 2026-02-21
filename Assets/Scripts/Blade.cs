using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Blade : MonoBehaviour
{
    public float minMoveDistance = 0.05f;

    LineRenderer lr;
    EdgeCollider2D edge;
    Vector3 lastWorldPos;
    bool hasLast = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();

        lr.positionCount = 0;
        edge.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hasLast = false;
            lr.positionCount = 0;
            edge.enabled = true;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0f;

            if (!hasLast)
            {
                AddPoint(world);
                lastWorldPos = world;
                hasLast = true;
            }
            else if (Vector3.Distance(world, lastWorldPos) >= minMoveDistance)
            {
                AddPoint(world);
                lastWorldPos = world;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            lr.positionCount = 0;
            edge.enabled = false;
            hasLast = false;
        }
    }

    void AddPoint(Vector3 p)
    {
        int n = lr.positionCount;
        lr.positionCount = n + 1;
        lr.SetPosition(n, p);

        // Update collider from line points
        Vector2[] pts = new Vector2[lr.positionCount];
        for (int i = 0; i < pts.Length; i++)
        {
            Vector3 v = lr.GetPosition(i);
            pts[i] = new Vector2(v.x, v.y);
        }
        edge.points = pts;
    }
}