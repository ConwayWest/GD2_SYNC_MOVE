using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class PlayerController : NetworkComponent
{
    public Vector3 lastInputPos = Vector3.zero;
    public Vector3 lastInputRot = Vector3.zero;
    public Vector3 lastInputVel = Vector3.zero;
    public Vector3 lastInputRotVel = Vector3.zero;
    public Rigidbody rb;
    public override void HandleMessage(string flag, string value)
    {
        

        if(IsServer)
        {
            if (flag == "MOVEPLAYER")
            {
                // Remove the paranthesis, split on the x, y, z
                char[] remove = { '(', ')' };

                string[] data = value.Trim(remove).Split(',');

                // You need to understand what is the server / client
                // Server and client will be using lastInput in different ways
                lastInputPos = new Vector3(
                    float.Parse(data[0]),
                    float.Parse(data[1]),
                    float.Parse(data[2])
                    );
            }

            if (flag == "ROTATEPLAYER")
            {
                char[] remove = { '(', ')' };
                string[] data = value.Trim(remove).Split(',');

                lastInputRot = new Vector3(
                    float.Parse(data[0]),
                    float.Parse(data[1]),
                    float.Parse(data[2])
                    );
            }

            if (flag == "SPEEDPLAYER")
            {
                char[] remove = { '(', ')' };
                string[] data = value.Trim(remove).Split(',');

                lastInputVel = new Vector3(
                    float.Parse(data[0]),
                    float.Parse(data[1]),
                    float.Parse(data[2])
                    );
            }

            if (flag == "ROTSPEEDPLAYER")
            {
                char[] remove = { '(', ')' };
                string[] data = value.Trim(remove).Split(',');

                lastInputRotVel = new Vector3(
                    float.Parse(data[0]),
                    float.Parse(data[1]),
                    float.Parse(data[2])
                    );
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while (true)
        {
            if(IsLocalPlayer && IsClient)
            {
                Vector3 _tempInputFast = new Vector3(1.0f, 0f, 0f);
                Vector3 _tempInputSlow = new Vector3(-1.0f, 0f, 0f);
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    SendCommand("SPEEDPLAYER", _tempInputSlow.ToString());
                    lastInputVel = _tempInputSlow;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SendCommand("SPEEDPLAYER", _tempInputFast.ToString());
                    lastInputVel = _tempInputFast;
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    SendCommand("ROTSPEEDPLAYER", _tempInputSlow.ToString());
                    lastInputRotVel = _tempInputSlow;
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    SendCommand("ROTSPEEDPLAYER", _tempInputFast.ToString());
                    lastInputRotVel = _tempInputFast;
                }

                // Store the input in a temporary Vector 3
                Vector3 _tempInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

                // Rotation
                Vector3 _tempInputRot = new Vector3(Input.GetAxisRaw("Mouse X") * 20, Input.GetAxisRaw("Mouse Y") * 20, 0);

                // It is important to minimizing the number of packets sent for input

                // How important is it for your game mechanic's control input to be precise?
                // You could use a threshold value instead of hardcoding it

                if ((_tempInput - lastInputPos).magnitude > 0.1f)
                {
                    // Send the command to the server
                    SendCommand("MOVEPLAYER", _tempInput.ToString());

                    // Then store this value as the last value received
                    lastInputPos = _tempInput;
                }

                
                if((_tempInputRot - lastInputRot).magnitude > 0.1f)
                {
                    SendCommand("ROTATEPLAYER", _tempInputRot.ToString());

                    lastInputRot = _tempInputRot;
                }
                
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only on client side
        // Graphical effects
        if (IsServer)
        {
            rb.position += lastInputPos * Time.deltaTime;
            rb.rotation *= Quaternion.Euler(lastInputRot);
            rb.velocity = lastInputVel;
            rb.angularVelocity = lastInputRotVel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Changing a variable must go inside the if(isServer) code
        if(IsServer)
        {
            if(collision.gameObject.tag == "Enemy")
            {
                // HP -= 1;
                // SendUpdate
            }
        }

        // If it is something I want all clients to see
        if(IsClient)
        {
            // Show graphical effect on all clients
        }

        if(IsLocalPlayer)
        {
            // Screen shake on collision
        }

        //***MAKE SET FUNCTIONS***
            // Make sure its the server
            // Then send the update

        // This template works well for GameObjects that move around and interact with the environment
        //If you have a situation where it's different
            //We have really only synchronized primitaves so far
            //We haven't talked about synchronized an array or a list
                // The best we can do is concatenate a string
                // Then parse everything out
    }
}
