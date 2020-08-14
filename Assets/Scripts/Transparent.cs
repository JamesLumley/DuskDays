using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {
	public GameObject wall;
	// Use this for initialization
	void Start () {
		Color color = new Color ();
		color.a = 0.4f;
		wall.GetComponent<Renderer>().material.color = color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
