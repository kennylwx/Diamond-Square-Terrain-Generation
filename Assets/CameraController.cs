using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float speed = 1000f;
    public float spinSpeed = 10f;
    public float borderWidth = 0;
    public GameObject terrain;

    private Rigidbody rb;
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Vector3 minBound;
    private Vector3 maxBound;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        this.transform.position = new Vector3(250.0f, 500.0f, 250.0f);
    }

    bool flag = false;

    void Update()
    {
        if (!flag)
        {

            // Boundary of the Terrain
            minBound = new Vector3(terrain.transform.position.x, 0,
                                       terrain.transform.position.z);
            maxBound = new Vector3(terrain.GetComponent<Renderer>().
                                       bounds.size.x, 0,
                                       terrain.GetComponent<Renderer>().
                                       bounds.size.z);
            maxBound += minBound;

            // Border Edging Space
            minBound.x += borderWidth;
            minBound.z += borderWidth;
            maxBound.x -= borderWidth;
            maxBound.z -= borderWidth;
            flag = true;

        }
    }

    void FixedUpdate()
    {
        yaw += spinSpeed * Input.GetAxis("Mouse X");
        pitch -= spinSpeed / 2.0f * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddTorque(transform.up * spinSpeed * -1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            rb.AddTorque(transform.up * spinSpeed);
        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(this.transform.forward * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(this.transform.forward * -speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(this.transform.right * -speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(this.transform.right * speed);
        }

        // Check if camera is out of bounds based on terrain 
        if (transform.position.x > maxBound.x)
        {
            SetTransformX(maxBound.x);
        }
        if (transform.position.z > maxBound.z)
        {
            SetTransformZ(maxBound.z);
        }
        if (transform.position.x < minBound.x)
        {
            SetTransformX(minBound.x);
        }
        if (transform.position.z < minBound.z)
        {
            SetTransformZ(minBound.z);
        }


    }

    void SetTransformX(float n)
    {
        transform.position = new Vector3(n, transform.position.y, transform.position.z);
    }

    void SetTransformZ(float n)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, n);
    }
}
