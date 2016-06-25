using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	void Update ()
	{
		this.transform.LookAt (
			transform.position + Camera.main.transform.rotation * Vector3.forward, 
			Camera.main.transform.rotation * Vector3.up
		);

		this.transform.position += new Vector3 (-2.0f * Time.deltaTime, 0.0f, 0.0f);
	}
}
