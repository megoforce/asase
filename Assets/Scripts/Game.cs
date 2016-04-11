using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	string server = "";
	string credentials = "";
	[SerializeField] List<Shoal> shoals;

	[SerializeField] Text debugText;
	[SerializeField] RectTransform projectsUIContainer;
	[SerializeField] HideShowUI hideShowUI;


	static GameObject prjectUIPrefab;
	void Awake(){
		shoals = new List<Shoal>();

		if(prjectUIPrefab == null){
			prjectUIPrefab = Resources.Load("UIProject") as GameObject;
		}
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

//				JiraToFishes((Resources.Load("test") as TextAsset).text);
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
			

			yield return new WaitForSeconds(10f);
		}



	}

	void JiraToFishes(string jiraText){
//		Debug.Log(jiraText);
		JSONObject k = new JSONObject(jiraText);

		debugText.text = "";
		debugText.text += "Total: "+k["issues"].Count;
//		CopyTextoToClipboard(jiraText);


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
				newProjectUI.GetComponent<UIProject>().SetProject(projectName,ProjectToColor(projectName));
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


	//Returns a deterministic random color for a string
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
			col[i] /= (max+0.7f);
		

		return new Color(col[0],col[1],col[2]);
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
