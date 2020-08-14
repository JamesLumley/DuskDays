using UnityEngine;
using System.Collections;

public class MainMenuScout : MonoBehaviour {

	public Transform shootPoint;
	private int count;
	// Use this for initialization
	void Start () {
		count = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime >= count)
		{
			StartCoroutine (stopAttack ());
			count++;
		}
	}

	public IEnumerator stopAttack()
	{
		yield return new WaitForSeconds(0.3f);
		
		GameObject stone = Instantiate(Resources.Load("RockLarge"), shootPoint.transform.position, Random.rotation) as GameObject;
		stone.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 40 + new Vector3(0,3,0);
		stone.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 50;
		
		yield return new WaitForSeconds(0.45f);
		
	}
}
