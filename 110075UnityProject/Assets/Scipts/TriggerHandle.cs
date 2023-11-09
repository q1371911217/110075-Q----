using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") return;
        if(this.transform.tag == "Destination")
        {           
            SendMessageUpwards("onReceiveDestination", SendMessageOptions.RequireReceiver);                
         
        }else if(this.transform.tag == "Star")
        {
            SendMessageUpwards("onReceiveStar", this.transform, SendMessageOptions.RequireReceiver);
            Destroy(this.gameObject);
        }else if(this.transform.tag == "Enemy")
        {
            SendMessageUpwards("onReceiveEnemy", SendMessageOptions.RequireReceiver);
        }
        
    }
}
