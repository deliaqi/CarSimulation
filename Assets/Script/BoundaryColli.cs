using UnityEngine;
using System.Collections;

public class BoundaryColli : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		Debug.Log("碰撞到的物体的名字是：" + collisionInfo.gameObject.name);

	}
	void OnTriggerEnter(Collider collider)
	{
		Debug.Log("OnTriggerEnter = " + collider.gameObject.name);
	}
}
