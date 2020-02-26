using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public class DummyMovement : NetworkComponent
{
    public override void HandleMessage(string flag, string value)
    {
        char[] remove = { '(', ')' };
        if (flag == "DUMMYPOS")
        {
            string[] data = value.Trim(remove).Split(',');

            Vector3 target = new Vector3(
                                        float.Parse(data[0]),
                                        float.Parse(data[1]),
                                        float.Parse(data[2])
                                        );

            if((target - this.transform.position).magnitude < .5f)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, target, .25f);
            }
            else
            {
                this.transform.position = target;
            }
        }

        if(flag == "DUMMYROT")
        {
            string[] data = value.Trim(remove).Split(',');
            Vector3 euler = new Vector3(
                                                float.Parse(data[0]),
                                                float.Parse(data[1]),
                                                float.Parse(data[2])
                                                );

            if((euler-this.transform.rotation.eulerAngles).magnitude < .5f)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(euler), .25f);
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(euler);
            }
        }
    }

    public override IEnumerator SlowUpdate()
    {
        while(true)
        {
            if (IsServer)
            {
                this.transform.position = new Vector3(Random.Range(-3.0f,3.0f),Random.Range(0f,3.0f),Random.Range(-3.0f,3.0f));
                this.transform.rotation = Quaternion.Euler(this.transform.eulerAngles + new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                SendUpdate("DUMMYPOS", this.transform.position.ToString());
                SendUpdate("DUMMYROT", this.transform.rotation.eulerAngles.ToString());
            }
            yield return new WaitForSeconds(2.0f);
        }
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
