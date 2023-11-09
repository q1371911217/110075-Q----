using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public List<Transform> posList;

    //public Transform rotateCenter;

    //public bool isRotation;

    int index = 0;
    Vector3 leftVec3 = new Vector3(-1, 1, 1);
    Vector3 lastPos;
    private void Start()
    {
        lastPos = transform.position;
    }
    Vector3 direction, newPosition;
    private void FixedUpdate()
    {
        //if(isRotation)
        //{

        //    //transform.RotateAround(rotateCenter.position, -Vector3.forward, speed *Time.deltaTime);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateCenter.rotation, speed * Time.deltaTime);
        //    return;
        //}

        if (Vector3.Distance(this.transform.localPosition, posList[index].localPosition) < 0.9f)
            index = (index + 1) % posList.Count;
        direction = posList[index].position - transform.position;
        newPosition = Vector3.MoveTowards(transform.position, posList[index].position, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
        if (lastPos.x < newPosition.x)
            transform.localScale = leftVec3;
        else
            transform.localScale = Vector3.one;

        lastPos = newPosition;
    }
}
