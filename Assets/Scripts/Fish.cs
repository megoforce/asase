using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {
	[SerializeField]
	public Issue issue;
	TrailRenderer myTrailRenderer;
	Transform myTransform;
	float speed = 0.3f;
	float amplitude = 5f;
	float seed;
	public static Material fishMaterial;
	void Awake(){
		myTransform = GetComponent<Transform>();
		myTrailRenderer = GetComponent<TrailRenderer>();

		if(Fish.fishMaterial == null)
			Fish.fishMaterial = Resources.Load("Materials/Fish") as Material;
		

		seed = Random.Range(0,1234567f);

	}

	public void Initialize(Issue _issue){
		issue = _issue;
		myTrailRenderer.startWidth = (float)(0.02f + 0.01f*Mathf.Pow((float)(issue.unsolvedMinutes)*10f,0.3f));

	}
	public void Colorize(Color color){
		myTrailRenderer.material.color = color;
		myTrailRenderer.material.SetColor ("_EmissionColor", color);
	}


	void Update(){
		float xPosition = -amplitude/2f + Mathf.PerlinNoise(seed,Time.time*speed)*amplitude;
		float yPosition = -amplitude/2f + Mathf.PerlinNoise(Time.time*speed,seed)*amplitude;
		float zPosition = -amplitude/2f + Mathf.PerlinNoise(seed+12.234f,Time.time*speed)*amplitude;
		myTransform.localPosition = new Vector3(xPosition,yPosition,zPosition);

	}
}
