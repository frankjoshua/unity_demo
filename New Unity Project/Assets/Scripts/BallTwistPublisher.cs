using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib.geometry_msgs;

public class BallTwistPublisher : ROSBridgeLib.ROSBridgePublisher
{
    // The following three functions are important
  public static string GetMessageTopic() {
    //topic name is up to the user
    return "/cmd_vel";
  }

  public static string GetMessageType() {
      return "geometry_msgs/Twist";
  }

  public static string ToYAMLString(TwistMsg msg) {
    return msg.ToYAMLString();
  }

  public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
    return new TwistMsg(msg);
  }    
}
