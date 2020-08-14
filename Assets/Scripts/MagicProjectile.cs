using UnityEngine;
using System.Collections;

public class MagicProjectile : MonoBehaviour {

	public GameObject target;
	// Use this for initialization
	void Start () {
		StartCoroutine (WaitAndDestroy ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(target != null)
		{
			this.transform.position = Vector3.MoveTowards (this.transform.position, target.transform.position + Vector3.up,15 * Time.deltaTime);
		}
	}

	IEnumerator WaitAndDestroy()
	{
		yield return new WaitForSeconds(2.0f);
		
		Destroy (gameObject);
	}
}
