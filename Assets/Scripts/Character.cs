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
	}
}
