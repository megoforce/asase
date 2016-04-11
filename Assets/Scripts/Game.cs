using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	string server = "";
	string credentials = "";
	[SerializeField]
//	List<Fish> fishes;
	List<Shoal> shoals;

	[SerializeField] Text debugText;
	void Awake(){
		shoals = new List<Shoal>();
//		fishes = new List<Fish>();
		List<string> projects = new List<string>();
		List<Vector3> shoalCentroids = new List<Vector3>();

	}
	void Start(){
		StartCoroutine(GetJiraData());
	}
	IEnumerator GetJiraData(){
		while(true){
			TextAsset credentialsTextAsset = Resources.Load("info") as TextAsset;
			if(credentialsTextAsset != null){
				server = credentialsTextAsset.text.Split('\n')[0];
				credentials = credentialsTextAsset.text.Split('\n')[1];
				Debug.Log("Query using:");
				Debug.Log("Server<color=yellow>\t\t"+server+"</color>");
				Debug.Log("Credentials<color=yellow>\t"+credentials+"</color>");

				Dictionary<string,string> headers = new Dictionary<string,string>();
				string url = server+"/rest/api/2/search?jql=resolution+=+Unresolved+ORDER+BY+updatedDate+DESC";
				headers["Authorization"] = "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials));

				JiraToFishes((Resources.Load("test") as TextAsset).text);
//				WWW www = new WWW(url, null, headers);
//				Debug.Log("pre www");
//				yield return www;
//				Debug.Log("post www");
//				if(www.error != null){
//					
//					JiraToFishes(www.text);
//
//
//				} else {
//					Debug.Log("Connection error "+www.error);
//				}
			} else {
				Debug.LogError("No credentials found. Please rename Resources/info-demo to Resources/info and write your server and credentials there.");
			}
			

			yield return new WaitForSeconds(10f);
		}



	}

	void JiraToFishes(string jiraText){
		debugText.text = jiraText;
		Debug.Log(jiraText);
		JSONObject k = new JSONObject(jiraText);

		debugText.text = "";
		debugText.text += "Total issues: "+k["issues"].Count;
		CopyTextoToClipboard(jiraText);


		double maxUnsolved = -1;
		double minUnsolved = 999999;
		for(int i = 0; i < k["issues"].Count; i++){
			string projectName = k["issues"][i]["fields"]["project"]["name"].str;
			Shoal currentShoal = GetShoalFromProjectName(projectName);
			if(currentShoal == null){
				GameObject newShoalGO = new GameObject(projectName);
				Shoal newShoal = newShoalGO.AddComponent<Shoal>();
				newShoal.Initialize(projectName,newShoalGO);
				shoals.Add(newShoal);
				currentShoal = newShoal;
			}
			currentShoal.AddFish(new Issue(k["issues"][i]));
		}



	}


	Shoal GetShoalFromProjectName(string projectName){
		for(int i = 0; i < shoals.Count; i++)
			if(shoals[i].projectName == projectName)
				return shoals[i];
		return null;
	}



	void CopyTextoToClipboard(string text){
		TextEditor te = new TextEditor();
		te.text = text;
		te.SelectAll();
		te.Copy();
	}
}
