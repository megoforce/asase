using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	string server = "";
	string credentials = "";
	[SerializeField] List<Shoal> shoals;
	List<UIProject> projectsUI;

	[SerializeField] RectTransform projectsUIContainer;
	[SerializeField] HideShowUI hideShowUI;


	static GameObject prjectUIPrefab;
	static AudioClip newProject, endProject, endFish;
	void Awake(){
		shoals = new List<Shoal>();
		projectsUI = new List<UIProject>();

		if(prjectUIPrefab == null){
			prjectUIPrefab = Resources.Load("UIProject") as GameObject;
			newProject = Resources.Load("Audio/new-project") as AudioClip;
			endProject = Resources.Load("Audio/end-project") as AudioClip;
			endFish = Resources.Load("Audio/end-fish") as AudioClip;

		}
	}
	void Start(){
		StartCoroutine(GetJiraData());
	}

	IEnumerator GetJiraData(){
		int i = 0;
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

//				JiraToFishes((Resources.Load("test"+i) as TextAsset).text);
//				i = (i+1) % 4;
				WWW www = new WWW(url, null, headers);
				yield return www;

				if(www.error == null){
					JiraToFishes(www.text);
				} else {
					Debug.Log("Connection error "+www.error);
				}
			} else {
				Debug.LogError("No credentials found. Please rename Resources/info-demo to Resources/info and write your server and credentials there.");
			}
			

			yield return new WaitForSeconds(5f);
		}



	}

	void JiraToFishes(string jiraText){
//		Debug.Log(jiraText);
		JSONObject k = new JSONObject(jiraText);

//		debugText.text = "Total: "+k["issues"].Count;
//		CopyTextoToClipboard(jiraText);



		//Add new issues
		for(int i = 0; i < k["issues"].Count; i++){
			string projectName = k["issues"][i]["fields"]["project"]["name"].str;
			Shoal currentShoal = GetShoalFromProjectName(projectName);
			if(currentShoal == null){
				GameObject newShoalGO = new GameObject(projectName);
				Shoal newShoal = newShoalGO.AddComponent<Shoal>();
				newShoal.Initialize(projectName,newShoalGO,ProjectToColor(projectName));
				shoals.Add(newShoal);
				currentShoal = newShoal;

				GameObject newProjectUI = Instantiate(prjectUIPrefab) as GameObject;
				RectTransform newProjectUITransform = newProjectUI.GetComponent<RectTransform>();
				newProjectUITransform.SetParent(projectsUIContainer);
				newProjectUITransform.sizeDelta = new Vector2(0,40f);
				newProjectUITransform.anchoredPosition = new Vector2(0,-20f-(shoals.Count-1)*40f);
				newProjectUI.GetComponent<UIProject>().SetProject(projectName);
				projectsUI.Add(newProjectUI.GetComponent<UIProject>());

				Debug.Log("new project: "+projectName);
				MonophonicAudio.instance.Play(newProject,1);
			}
			currentShoal.AddFish(new Issue(k["issues"][i]));
		}


		//Remove done issues
		for(int i = 0; i < shoals.Count; i++){
			for(int j = 0; j < shoals[i].fishes.Count; j++){
				if(!KeyExistInJSON(k,shoals[i].fishes[j].issue.key)){
					Debug.Log("end fish: "+shoals[i].fishes[j].issue.key);
					shoals[i].RemoveFish(shoals[i].fishes[j].issue.key);
					MonophonicAudio.instance.Play(endFish,2);
				}
			}

			if(!ProjectExistInJSON(k,shoals[i].projectName)){
				Debug.Log("end project: "+shoals[i].projectName);
				Destroy(projectsUI[i].gameObject);
				projectsUI.RemoveAt(i);
				Destroy(shoals[i].gameObject);
				shoals.RemoveAt(i);

				MonophonicAudio.instance.Play(endProject,1);
			}
		}




	}

	bool ProjectExistInJSON(JSONObject k, string projectName){
		for(int i = 0; i < k["issues"].Count; i++)
			if(k["issues"][i]["fields"]["project"]["name"].str == projectName)
				return true;
		
		return false;
	}
	bool KeyExistInJSON(JSONObject k, string key){
		for(int i = 0; i < k["issues"].Count; i++){
			if(k["issues"][i]["key"].str == key){
				return true;
			}
		}
		return false;
	}

	Shoal GetShoalFromProjectName(string projectName){
		for(int i = 0; i < shoals.Count; i++)
			if(shoals[i].projectName == projectName)
				return shoals[i];
		return null;
	}


	//Returns a deterministic orange for a string
	Color ProjectToColor(string projectName){
		int a = projectName.GetHashCode();
		float[] col = new float[3];

		//Get 3 random deterministic numbers
		col[0] = Mathf.Abs((a % 12345) / 12345f);
		col[1] = Mathf.Abs((a % 9017) / 9017f);
		col[2] = Mathf.Abs((a % 16778) / 16778f);

		//Normalize
		float max = float.MinValue;
		for(int i = 0; i < col.Length; i++)
			if(col[i] > max)
				max = col[i];

		for(int i = 0; i < col.Length; i++)
			col[i] /= max;
		

		return new Color(1,0.2f+col[1]*0.25f,0);
	}
	void CopyTextoToClipboard(string text){
		TextEditor te = new TextEditor();
		te.text = text;
		te.SelectAll();
		te.Copy();
	}
	void Update(){
		if(Input.GetKeyDown(KeyCode.Tab)){
			if(hideShowUI.shown)
				hideShowUI.Hide();
			else
				hideShowUI.Show();
		}
	}
}
