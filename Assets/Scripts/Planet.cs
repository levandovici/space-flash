using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField]
    private float _gravityRadius;

    [SerializeField]
    private float _force = 10f;

    [SerializeField]
    private float _rotationSpeed = 3f;

    [SerializeField]
    private CircleCollider2D _gravity;

    [SerializeField]
    private SpriteRenderer _gravityGlow;



    public float RotationSpeed => _rotationSpeed;



    public void Awake()
    {
        _gravity = GetComponentsInChildren<CircleCollider2D>()[1];
    }

    private void Update()
    {
        transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles,
            new Vector3(0f, 0f, transform.eulerAngles.z + 90f), _rotationSpeed * Time.deltaTime * 30f);
    }



    public Vector2 Force(Vector2 position)
    {
        float dist = Vector2.Distance(position, transform.position);
        if (dist > _gravityRadius)
            return Vector2.zero;

        float force = dist / _gravityRadius;


        Vector2 gravityVector = (Vector2)transform.position - position;
        gravityVector = gravityVector.normalized * force * _force;


        Vector2 rotationVector = NextPointOfCircle(transform.position, position, _rotationSpeed < 0f) - position;
        rotationVector = rotationVector.normalized * force * Mathf.Abs(_rotationSpeed) * _force;


        return gravityVector + rotationVector;
    }

    public void SetUp(float gravityRadius, float force, float maxForce, float rotationSpeed)
    {
        _force = force;
        _gravity.radius = _gravityRadius = gravityRadius;
        _rotationSpeed = rotationSpeed;
       _gravityGlow.transform.localScale = new Vector3(gravityRadius, gravityRadius, 1f);
        _gravityGlow.color = new Color(1f, 1f, 1f, force / maxForce);
    }



    public static float GravityRadius(Planet planet)
    {
        return planet._gravityRadius;
    }

    public static GameObject GravityGlow(Planet planet)
    {
        return planet._gravityGlow.gameObject;
    }


    private Vector2[] CirclePoints(Vector2 center, float radius, int segments)
    {
        Vector2[] arr = new Vector2[segments];

        float angle = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle * i) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle * i) * radius;

            arr[i] = new Vector2(x + center.x, y + center.y);
        }


        return arr;
    }

    private Vector2 NextPointOfCircle(Vector2 center, Vector2 position, bool rotateRight)
    {
        Vector2[] arr = CirclePoints(center, Vector2.Distance(center, position), 72);

        int id = -1;
        float dist = Mathf.Infinity;

        for(int i = 0; i < arr.Length; i++)
        {
            float d = Vector2.Distance(arr[i], position);

            if (d < dist)
            {
                id = i;
                dist = d;
            }
        }

        if(rotateRight)
        {
            id++;

            if (id >= arr.Length)
                id = 0;

            Debug.Log($"Right: {id}");
        }
        else
        {
            id--;

            if (id < 0)
                id = arr.Length - 1;

            Debug.Log($"Left: {id}");
        }

        return arr[id];
    }
}
