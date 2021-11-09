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

  public string goalTopic = "/goal_pose";

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

  private Camera cam = null;

  void Start()
  {
    
    cam = Camera.main;
    // start the ROS connection
    ROSConnection.SetIPPref("192.168.33.58");
    ros = ROSConnection.GetOrCreateInstance();
    ros.RegisterPublisher<TwistMsg>(topicName);
    ros.RegisterPublisher<PoseStampedMsg>(goalTopic);
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
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
    timeElapsed += Time.deltaTime;

    if (timeElapsed > publishMessageFrequency)
    {
      // Finally send the message to server_endpoint.py running in ROS
      // ros.Publish(topicName, cubePos);

      timeElapsed = 0;
    }

    MoveNavigatePosition();
  }

  private void MoveNavigatePosition(){
    if(Mouse.current.leftButton.wasPressedThisFrame){

      Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
      Plane plane = new Plane(Vector3.up, 0f);
      float distanceToPlane;
      if(plane.Raycast(ray, out distanceToPlane)){
        Vector3 clickPosition = ray.GetPoint(distanceToPlane);
        Debug.Log(clickPosition);
        //Send goal
        PoseMsg poseMsg = new PoseMsg(new PointMsg(clickPosition.z,-clickPosition.x,0), new QuaternionMsg(0,0,0,1));
        PoseStampedMsg goal = new PoseStampedMsg(new RosMessageTypes.Std.HeaderMsg(new RosMessageTypes.BuiltinInterfaces.TimeMsg(), "map"), poseMsg);
        ros.Publish(goalTopic, goal);
      }
    }
  }
}
