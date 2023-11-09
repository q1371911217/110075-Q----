using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAround : MonoBehaviour
{
    public float speed;

    private void FixedUpdate()
    {
        transform.RotateAround(this.transform.position, -Vector3.forward, speed * Time.deltaTime);
    }
}
