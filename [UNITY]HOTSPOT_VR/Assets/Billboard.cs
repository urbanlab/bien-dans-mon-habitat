using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	private void Update() {
		transform.forward = -Camera.main.transform.up;
		//transform.LookAt(Camera.main.transform);
		//transform.localRotation = Quaternion.Euler(new Vector3(90,Camera.main.transform.forward.y,90));
	}
}
