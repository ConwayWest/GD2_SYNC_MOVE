using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class FaceSpawn : NetworkComponent
{
    public bool faceSpawned = false;
    public override void HandleMessage(string flag, string value)
    {
        if(flag=="FACE")
        {
            if(IsServer)
            {
                MyCore.NetCreateObject(int.Parse(value), Owner, new Vector3(0.0f, 1.0f, -3.0f));
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while(true)
        {
            if(IsClient && IsLocalPlayer)
            {
                if (!faceSpawned)
                {
                    SpawnFace();
                    faceSpawned = true;
                }
            }
            if(IsLocalPlayer)
            {
                
            }
            if(IsServer)
            {

            }

            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    public void SpawnFace()
    {
        SendCommand("FACE", "1");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
