using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {
	[SerializeField]
	public Issue issue;
	TrailRenderer myTrailRenderer;
	Transform myTransform;
	float speed = 0.3f;
	float amplitude = 15f;
	float seed;
	public static Material solvedMaterial, unsolvedMaterial;
	void Awake(){
		myTransform = GetComponent<Transform>();
		myTrailRenderer = GetComponent<TrailRenderer>();

		if(Fish.solvedMaterial == null){
			Fish.solvedMaterial = Resources.Load("Materials/Solved") as Material;
			Fish.unsolvedMaterial = Resources.Load("Materials/Unsolved") as Material;
		}

		seed = Random.Range(0,1234567f);
	}

	public void Initialize(Issue _issue){
		issue = _issue;
		myTrailRenderer.sharedMaterial = (issue.solved) ? solvedMaterial : unsolvedMaterial;
	}
	public void NormalizeSize(double minUnsolved, double maxUnsolved){
		if(!issue.solved){
			double delta = maxUnsolved - minUnsolved;
			myTrailRenderer.startWidth = (float)(0.1f + ((issue.unsolvedMinutes-minUnsolved) / delta));
		} else {
			myTrailRenderer.startWidth = 0.1f;
		}
	}

	void Update(){
		float xPosition = (-amplitude/2f + Mathf.PerlinNoise(seed,Time.time*speed)*amplitude)*(16f/9f);
		float yPosition = -amplitude/2f + Mathf.PerlinNoise(Time.time*speed,seed)*amplitude;


		myTransform.position = new Vector3(xPosition,yPosition,0);

	}
}
