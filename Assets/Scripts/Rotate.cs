using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	float speed = 0.07f;
	float amplitude = 1f;
	float seed;
	Transform myTransform;
	void Awake(){
		seed = Random.Range(0,1234567f);	
		myTransform = GetComponent<Transform>();
	}

	void Update () {
		float x = (-amplitude/2f + Mathf.PerlinNoise(seed,Time.time*speed)*amplitude);
		float y = (-amplitude/2f + Mathf.PerlinNoise(Time.time*speed,seed)*amplitude);
		float z = (-amplitude/2f + Mathf.PerlinNoise(seed+12.234f,Time.time*speed)*amplitude);

		myTransform.Rotate(x,y,z);
	}
}
