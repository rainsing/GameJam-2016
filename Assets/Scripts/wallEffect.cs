using UnityEngine;
using System.Collections;

public class wallEffect : MonoBehaviour {
	public GameObject person_start;
	public GameObject person_mid_1;
	public GameObject person_mid_2;
	public GameObject person_end;

	GameObject person = null;
	public bool isMesh = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
//		Global.WallProgress += 0.003f;
//		if (Global.WallProgress >= 1.0f) {
//			Global.WallProgress = 0.0f;
//		}
//		GetComponent<SpriteRenderer> ().material.SetFloat ("_FinalScale", Global.WallProgress);

		//if (Input.GetMouseButtonDown (0))
		{

			Vector3 start = person_start.transform.position;
			Vector3 end = person_end.transform.position;

			Vector3 start_scale = person_start.transform.localScale;
			Vector3 end_scale = person_end.transform.localScale;

			float random = Random.value;


			Vector3 pos = start+(end - start)*random;

			//判断pos是否在person_mid_1和person_mid_2之间
			Vector3 dir1 = pos-person_mid_1.transform.position;
			Vector3 dir2 = pos-person_mid_2.transform.position;
			//if (Vector3.Dot (dir1, dir2) < 0 || dir2.magnitude<dir1.magnitude)
			if (Vector3.Dot (dir1, dir2) < 0) {
				Vector3 v1 = person_mid_1.transform.position;
				Vector3 v2 = person_mid_2.transform.position;
				pos+=(v2 - v1);
			}




			Vector3 scale = start_scale+(end_scale - start_scale)*random;
			GameObject.Destroy (person);
			person = GameObject.Instantiate (person_start);

			if (isMesh) {
				person.transform.parent = transform;
			}
			//person.transform.Translate (pos);
			person.transform.position = pos;
			person.transform.localScale = scale;
			person.transform.localRotation = person_start.transform.localRotation;


			person.SetActive (true);


			//Global.WallProgress += 0.05f;
			if (isMesh) {
				GetComponent<MeshRenderer> ().material.SetFloat ("_FinalScale", Global.WallProgress);
			} else {
				GetComponent<SpriteRenderer> ().material.SetFloat ("_FinalScale", Global.WallProgress);
			}

			/*
			if (Global.WallProgress > 1.0f) {
				Global.WallProgress = 0.0f;
			}
			*/
		}
	
	}
		
}
