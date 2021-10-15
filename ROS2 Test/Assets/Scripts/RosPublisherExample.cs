using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Sensor;
using UnityEngine.InputSystem;
using RosMessageTypes.Tf2;

/// <summary>
///
/// </summary>
public class RosPublisherExample : MonoBehaviour
{
  ROSConnection ros;
  public string topicName = "/cmd_vel";

  // The game object
  public GameObject cube;
  // Publish the cube's position and rotation every N seconds
  public float publishMessageFrequency = 0.1f;

  // Used to determine how much time has elapsed since the last message was published
  private float timeElapsed;

  private TwistMsg cubePos = new TwistMsg(
              new Vector3Msg(
              0,
              0,
              0),
              new Vector3Msg(0,
              0,
              0)
          );

  void Start()
  {
    // start the ROS connection
    ros = ROSConnection.GetOrCreateInstance();
    ros.RegisterPublisher<TwistMsg>(topicName);
    ros.Subscribe<TFMessageMsg>("/tf", OnMessageReceived);
  }

  void OnMessageReceived(TFMessageMsg data)
  {

    foreach (var transformStamped in data.transforms)
    {
      if (transformStamped.child_frame_id == "base_footprint")
      {
        // Debug.Log(transformStamped.transform.translation);
        // var location = transformStamped.transform.From<FLU>;
        var location = new Vector3(
            -(float)transformStamped.transform.translation.y,
            (float)transformStamped.transform.translation.z + 1f,
            (float)transformStamped.transform.translation.x
            );
        //Debug.Log(location);
        // cube.transform.position = transformStamped.transform.translation.As<FLU>();
        cube.transform.position = location;
      }
    }

  }

  public void OnMovement(InputAction.CallbackContext value)
  {
    Vector2 inputMovement = value.ReadValue<Vector2>();

    Debug.Log(inputMovement);
    cubePos.angular = new Vector3Msg(0, 0, -inputMovement.x);
    cubePos.linear = new Vector3Msg(inputMovement.y, 0, 0);
  }

  private void Update()
  {
    timeElapsed += Time.deltaTime;

    if (timeElapsed > publishMessageFrequency)
    {
      // Finally send the message to server_endpoint.py running in ROS
      ros.Publish(topicName, cubePos);

      timeElapsed = 0;
    }
  }
}
