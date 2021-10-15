using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ROSBridgeLib;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;


public class BallPoseSubscriber : ROSBridgeSubscriber
{
  static GameObject ball;
  
  // These two are important
  public new static string GetMessageTopic() {
    //Topic name is up to the user. It should return the full path to the topic. 
    //For eg, "/turtle1/cmd_vel" instead of "/cmd_vel".
    return "/pose_info";
  }

  public new static string GetMessageType() {
    //Same as the definition present in ROS documentation:
    return "geometry_msgs/Pose";
  }

  // Important function (I think.. Converts json to PoseMsg)
  public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
    return new PoseMsg (msg);
  }

  // This function should fire on each received ros message
  public new static void CallBack(ROSBridgeMsg msg) {
    
    
    Debug.Log("Recieved Message : "+msg.ToYAMLString());
    // Update ball position, or whatever
    ball=GameObject.Find("ball");
    Vector3 ballPos=ball.transform.position;
    ballPos.x = ((PoseMsg) msg).GetPosition().GetX();
    ballPos.y = ((PoseMsg) msg).GetPosition().GetY();
    ballPos.z = ((PoseMsg) msg).GetPosition().GetZ();
    //Changing ball's position to the updated position vector
    ball.transform.position=ballPos;
  }
}
