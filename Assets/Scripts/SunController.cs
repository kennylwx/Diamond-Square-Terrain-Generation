using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {

    public float speed;
    public GameObject terrain; 

	// Use this for initialization
	void Start () {
        transform.Translate(new Vector3(0.0f,1000.0f,0.0f));
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(new Vector3(0.0f,0.0f,250.0f),
                               Vector3.right,speed*Time.deltaTime);
        transform.LookAt(terrain.GetComponent<Renderer>().bounds.size/2);   
	}
}
