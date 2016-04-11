using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shoal : MonoBehaviour{
	List<Fish> fishes;
	public GameObject shoalGameObject;
	public Transform shoalTransform;

	public string projectName;
	public static GameObject fishPrefab;

	float speed = 0.07f;
	float amplitude = 15f;
	float seed;



	public void Initialize(string _projectName, GameObject _shoalGameObject){
		fishes = new List<Fish>();
		projectName = _projectName;
		shoalGameObject = _shoalGameObject;
		shoalTransform = shoalGameObject.GetComponent<Transform>();
		if(fishPrefab == null){
			fishPrefab = Resources.Load("Fish") as GameObject;
		}
		seed = Random.Range(0,1234567f);


	}
	public void AddFish(Issue issue){
		Fish currentFish = GetFish(issue.key);
		if(currentFish == null){
			GameObject newFishGO = Instantiate(fishPrefab) as GameObject;
			newFishGO.name = issue.key;
			newFishGO.GetComponent<Transform>().SetParent(shoalGameObject.GetComponent<Transform>());
			Fish newFish = newFishGO.AddComponent<Fish>();
			newFish.Initialize(issue);
			newFish.Colorize(ProjectToColor());
			fishes.Add(newFish);
		}

	}
	public void RemoveFish(string key){
		Fish currentFish = GetFish(key);
		Destroy(currentFish.gameObject);
		fishes.Remove(currentFish);

	}

	Fish GetFish(string key){
		for(int i = 0; i < fishes.Count; i++)
			if(fishes[i].issue.key == key)
				return fishes[i];
		
		return null;
	}

	//Returns a deterministic random color for a string
	public Color ProjectToColor(){
		int a = projectName.GetHashCode();
		float r = (float)(a % 12345) / 12345f;
		float g = (float)(a % 9017) / 9017f;
		float b = (float)(a % 16778) / 16778f;	
		return new Color(r,g,b);
	}
	void Update(){
		float xPosition = (-amplitude/2f + Mathf.PerlinNoise(seed,Time.time*speed)*amplitude)*(Screen.width/Screen.height);
		float yPosition = (-amplitude/2f + Mathf.PerlinNoise(Time.time*speed,seed)*amplitude)*(Screen.height/Screen.width);
		float zPosition = (-amplitude/2f + Mathf.PerlinNoise(seed+12.234f,Time.time*speed)*amplitude)*(Screen.height/Screen.width);
		shoalTransform.position = new Vector3(xPosition,yPosition,zPosition);

	}
}
