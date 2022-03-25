using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingOnStair : MonoBehaviour
{
    public Rigidbody rb;
    public Transform stair;
    public Transform player;
    public Transform foot;
    public bool walk;
    public Transform levelEnd;
    void Start()
    {
        
        rb = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(player.position.z >= stair.position.z)
        {
            walk = true;
            Climb();
        }
    }

    public void Climb()
    {
        Ray ray = new Ray(foot.position, player.forward);
        Debug.DrawLine(foot.position, foot.position + player.forward, Color.blue, 2f);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 3f))
        {
            if(hit.transform != levelEnd)
            {
                rb.position = new Vector3(rb.position.x, rb.position.y + 0.3f, rb.position.z);
            }
            
        }
    }
}
