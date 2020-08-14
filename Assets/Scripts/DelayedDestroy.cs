using UnityEngine;
using System.Collections;

public class DelayedDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		StartCoroutine (WaitAndDestroy ());
	}

	IEnumerator WaitAndDestroy()
	{
		yield return new WaitForSeconds(5.0f);

		Destroy (gameObject);
	}
}
