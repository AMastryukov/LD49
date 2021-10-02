using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, rotationSpeed);
    }
}
