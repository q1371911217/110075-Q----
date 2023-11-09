using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public VariableJoystick variableJoystick;
    public Animator anim;

    float keepx = 0;
    float keepy = 0;

    bool isStart = false;

    public void setStart(bool start)
    {
        isStart = start;
        //if (!start)
        //    rb.velocity = Vector2.zero;
    }


    public void FixedUpdate()
    {
        //if (!isStart) return;   
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (Mathf.Abs(variableJoystick.Horizontal) > 0 || Mathf.Abs(variableJoystick.Vertical) > 0)
        {
            keepx = variableJoystick.Horizontal;
            keepy = variableJoystick.Vertical;
        }

        anim.SetFloat("yspeed", keepy);
        anim.SetFloat("xspeed", keepx);

        rb.velocity = new Vector2(variableJoystick.Horizontal * speed * Time.deltaTime, variableJoystick.Vertical * speed * Time.deltaTime);
        
       
    }
}
