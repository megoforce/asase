using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {
	[SerializeField]
	Issue issue;

	public static Material solvedMaterial, unsolvedMaterial;
	void Awake(){
		if(solvedMaterial == null){
			solvedMaterial = Resources.Load("Materials/Solved") as Material;
			unsolvedMaterial = Resources.Load("Materials/Unsolved") as Material;
		}
	}

	public void Initialize(Issue _issue,double minUnsolved, double maxUnsolved){
		issue = _issue;
		GetComponent<Renderer>().sharedMaterial = (issue.solved) ? solvedMaterial : unsolvedMaterial;
		if(!issue.solved){
			double delta = maxUnsolved - minUnsolved;
			GetComponent<Transform>().localScale = (float)(0.5f + ((issue.unsolvedMinutes-minUnsolved) / delta))*Vector3.one;
		} else {
			GetComponent<Transform>().localScale = 0.5f*Vector3.one;
		}
		
		
			
	}
}
