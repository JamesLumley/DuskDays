using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PathNode : MonoBehaviour {

	public List<GameObject> linkedNodes = new List<GameObject>();
	public int cost = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		foreach(GameObject node in linkedNodes)
//		{
//			Debug.DrawLine(this.transform.position, node.transform.position, Color.green);
//		}
	}
}
