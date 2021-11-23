using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acceleration : MonoBehaviour
{
    /*public Rigidbody m_rigidbody;
    public Vector3 acceleration;

    private Vector3 lastVelocity;
    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        lastVelocity = m_rigidbody.velocity;
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        acceleration = (m_rigidbody.velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = m_rigidbody.velocity;
        Debug.DrawLine(m_rigidbody.position, m_rigidbody.position + acceleration /10, Color.green);
        Debug.Log("(" + acceleration.x + ", " + acceleration.y + ", " + acceleration.z + ")");
    }*/


    public GameObject _grabbable;
    public GameObject _hand;
    public GameObject _m_object;
    public Vector3 accH2P = new Vector3(0, 0, 0);
    public Vector3 forceH2P_hand = new Vector3(0, 0, 0);



    private Vector3 m_Acceleration = new Vector3(0, 0, 0);
    private Vector3 outputAcceleration_old = new Vector3(0, 0, 0);
    private Vector3 outputAcceleration_new = new Vector3(0, 0, 0);
    private Vector3 f_position_old = new Vector3(0, 0, 0);
    private Vector3 f_position_new = new Vector3(0, 0, 0);
    private Vector3 lastVelocity = new Vector3(0, 0, 0);
    private Vector3 newVelocity = new Vector3(0, 0, 0);
    private Vector3 lastPosition;
    private Vector3 newPosition = new Vector3(0, 0, 0);
    private Transform m_Transform;
    private bool grabState;
    private float alpha =  0.1f;
    private float beta = 0.2f;
    private Vector3 gravity =new Vector3(0, -9.81f, 0);
    private Transform _handtrans;
    private float objectMass = 0f;
    private Vector3 accH2P_hand = new Vector3(0, 0, 0);


    void Awake()
    {
        m_Transform = GetComponent<Transform>();
        _handtrans = _hand.GetComponent<Transform>();
        grabState = _grabbable.GetComponent<HandPosing.Interaction.Grabbable>().IsGrabbed;
        lastPosition = m_Transform.position;
        newPosition = m_Transform.position;
        objectMass = _m_object.GetComponent<Rigidbody>().mass;

}

    // Update is called once per frame
    private void Update()
    {
        grabState = _grabbable.GetComponent<HandPosing.Interaction.Grabbable>().IsGrabbed;
        if(grabState)
        {
            Debug.DrawLine(newPosition, newPosition + accH2P, Color.green);
        }
        
        //Debug.Log("(" + m_Acceleration.x + ", " + m_Acceleration.y + ", " + m_Acceleration.z + ")");
    }

    void FixedUpdate()
    {
        newPosition = m_Transform.position;
        f_position_new = alpha * newPosition + (1 - alpha) * f_position_old; //One-order Low-pass Filter for Position
        newVelocity = (f_position_new - f_position_old) / Time.fixedDeltaTime;
        m_Acceleration = (newVelocity - lastVelocity) / Time.fixedDeltaTime;
        outputAcceleration_new = beta * m_Acceleration + (1 - beta) * outputAcceleration_old; // One-order Low-pass Filter for Acceleration
        outputAcceleration_old = outputAcceleration_new;
        lastPosition = newPosition;
        lastVelocity = newVelocity;
        f_position_old = f_position_new;
        accH2P = gravity - outputAcceleration_new;
        accH2P_hand = _handtrans.InverseTransformVector(accH2P);
        forceH2P_hand = accH2P_hand * objectMass;
    }
}
