using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public float height = 5f;
    public float distance = 10f;

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z) - target.forward * distance;
        transform.LookAt(target);
    }
}
