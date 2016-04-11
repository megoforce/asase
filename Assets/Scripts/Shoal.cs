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

	public Color color;


	public void Initialize(string _projectName, GameObject _shoalGameObject, Color _color){
		fishes = new List<Fish>();
		projectName = _projectName;
		shoalGameObject = _shoalGameObject;
		color = _color;
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
			newFish.Colorize(color);
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


	void Update(){
		float x = (-amplitude/2f + Mathf.PerlinNoise(seed,Time.time*speed)*amplitude)*(Screen.width/Screen.height);
		float y = (-amplitude/2f + Mathf.PerlinNoise(Time.time*speed,seed)*amplitude)*(Screen.height/Screen.width);
		float z = (-amplitude/2f + Mathf.PerlinNoise(seed+12.234f,Time.time*speed)*amplitude)*(Screen.height/Screen.width);
		shoalTransform.position = new Vector3(x,y,z);

	}
}
