using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

[ExecuteInEditMode]

public class ShadowCaster : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
		GetComponent<Renderer>().receiveShadows = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
