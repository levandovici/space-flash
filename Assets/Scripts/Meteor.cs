using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private float _rotationSpeed = 0f;



    private void Awake()
    {
        _rotationSpeed = UnityEngine.Random.Range(-1, 2) * UnityEngine.Random.Range(60f, 120f);
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
    }
}
