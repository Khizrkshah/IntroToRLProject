using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class My_Agent : Agent
{

  Rigidbody rb;
  public float speed = 5.0f;
  public float jumpAmount = 10;
    //private Vector3 startingPosition = new Vector3(3.75f,0.958000064f,-4.19999981f);
  private Vector3 startingPosition = new Vector3(3.75f,1.20000005f,-4.19999981f);
  public float currentdistance = 0.0f;
  [SerializeField] Transform door;
    float reward = 0.0f;
    public GameObject key;
    private GameObject skey;
    public GameObject spawner1;
    public GameObject spawner2;
    public GameObject spawner3;
    private bool foundKey = false;
    public Material[] material;
    Renderer rend;

    private enum ACTIONS {
        LEFT = 0,
        NOTHING = 1,
        RIGHT = 2,
        FORWARD = 3,
        BACKWARD = 4
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rend = GameObject.Find("Exit").GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
    }

    public override void OnEpisodeBegin() {
        transform.localPosition = startingPosition;
        rend.sharedMaterial = material[0];
        foundKey = false;
        currentdistance = 0.0f;
        int spawner = Random.Range(1, 4);
        if(spawner == 1)
        {
             skey = Instantiate(key, spawner1.transform.position, Quaternion.identity);
        }else if(spawner == 2)
        {
             skey = Instantiate(key, spawner2.transform.position, Quaternion.identity);
        }else if(spawner == 3)
        {
             skey = Instantiate(key, spawner3.transform.position, Quaternion.identity);
        }

    }
    
  public override void CollectObservations(VectorSensor sensor) {
        // We don't need this function now because we use the RayPerceptionSensor
        // Note however that we could add additional observations here, if we wanted to, like the speed & velocity of the agent etc.
    }
    
    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == -1) {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (horizontal == +1) {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else if(vertical == +1){
          actions[0] = (int)ACTIONS.FORWARD;
        }
        else if(vertical == -1){
          actions[0] = (int)ACTIONS.BACKWARD;
        }
        else {
            actions[0] = (int)ACTIONS.NOTHING;
        }

    }
    
    public override void OnActionReceived(ActionBuffers actions) {
        var actionTaken = actions.DiscreteActions[0];

        switch (actionTaken) {
            case (int)ACTIONS.NOTHING:
                break;
            case (int)ACTIONS.LEFT:
                //transform.Rotate(0, -2, 0);
                transform.Translate(-Vector3.right * speed * Time.fixedDeltaTime);
                break;
            case (int)ACTIONS.RIGHT:
                //transform.Rotate(0, 2, 0);
                transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
                break;
            case (int)ACTIONS.FORWARD:
               transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
               break;
            case (int)ACTIONS.BACKWARD:
               transform.Translate(-Vector3.forward * Time.fixedDeltaTime * speed);
               break;
        }

        /*float distance = Vector3.Distance(door.transform.position,transform.position);
        float deltaDistance = distance - currentdistance;
        currentdistance = distance;
        Debug.Log("deltaDistance" + deltaDistance);
        Debug.Log("Distance" + distance);
        if (deltaDistance < 0){
            AddReward(0.1f);
            reward += 0.1f;
            Debug.Log("reward = " + reward);
        }*/

        
    }
    
    private void OnTriggerEnter(Collider other) {
        // If the agent collided with a ball, we delete the Balls & end the episode
        if(other.tag == "Death") {
            Debug.Log("negative = " + reward);
            AddReward(-10f);
            reward -= 1;
            Debug.Log("negative1 = " + reward);
            Destroy(skey);
            EndEpisode();
        }
        if(other.tag =="Exit" && foundKey == true)
        {
          AddReward(10.0f);
            reward += 10;
            Debug.Log("reward = " + reward);
            Destroy(skey);
            EndEpisode();
        }
        if(other.tag == "Wall"){
            Debug.Log("negative = " + reward);
            AddReward(-10f);
            reward -= 10;
            Debug.Log("negative1 = " + reward);
            Destroy(skey);
            EndEpisode();
        }
        if(other.tag == "Key")
        {
            Debug.Log("Found Key!");
            AddReward(5f);
            reward += 5;
            Debug.Log("reward = " + reward);
            foundKey = true;
            Destroy(skey);
            rend.sharedMaterial = material[1];
        }
    }



    /* Update is called once per frame
    void FixedUpdate()
    {
      if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.S))
        { 
            transform.Translate(-1 * Vector3.forward * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.A)) 
        { 
            transform.Rotate(0, -2, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 2, 0);
        }
        
      if (Input.GetKeyDown(KeyCode.Space))
      {
        rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
      }

    }*/

}
