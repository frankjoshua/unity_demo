using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;

public class DataManager : MonoBehaviour
{
    Rigidbody rb;
    GameObject rosObj;
    //Required for TwistMsg
    Vector3Msg linearVel;
    Vector3Msg angularVel;
    TwistMsg msg;
    
    // Start is called before the first frame update
    void Start()
    {
      //Since we attached ROSInitiazer to Main Camera:
        rosObj = GameObject.Find ("MainCamera");
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      //dependant on the message defintion:
        linearVel=new Vector3Msg(
            rb.velocity.x,
            rb.velocity.y,
            rb.velocity.z
        );
        angularVel=new Vector3Msg(
            rb.angularVelocity.x,
            rb.angularVelocity.y,
            rb.angularVelocity.z
        );
        msg=new TwistMsg(linearVel,angularVel);
        rosObj.GetComponent<ROSInitializer>().ros.Publish(
            BallTwistPublisher.GetMessageTopic(),msg
        );
    }
}
