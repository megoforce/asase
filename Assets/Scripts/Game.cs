using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	string server = "";
	string credentials = "";
	[SerializeField]
	List<Issue> issues;

	[SerializeField] Text debugText;

	void Awake(){
		issues = new List<Issue>();
	}
	void Start(){
		StartCoroutine(GetJiraData());
	}
	IEnumerator GetJiraData(){
		TextAsset credentialsTextAsset = Resources.Load("info") as TextAsset;
		if(credentialsTextAsset != null){
			server = credentialsTextAsset.text.Split('\n')[0];
			credentials = credentialsTextAsset.text.Split('\n')[1];

			Dictionary<string,string> headers = new Dictionary<string,string>();
			string url = server+"/rest/api/2/search?jql=ORDER+BY+createdDate+DESC";
			headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
				System.Text.Encoding.ASCII.GetBytes(credentials));

			WWW www = new WWW(url, null, headers);
			yield return www;

//			debugText.text = www.text;
//			Debug.Log(www.text);
			JSONObject k = new JSONObject(www.text);

			debugText.text = "";
			debugText.text += "Total issues: "+k["issues"].Count;
//			CopyTextoToClipboard(www.text);


			for(int i = 0; i < k["issues"].Count; i++)
				issues.Add(new Issue(k["issues"][i]));

			int doneIssues = 0;
			int undoneIssues = 0;
			for(int i = 0; i < issues.Count; i++){
				if(issues[i].solved)
					doneIssues++;
				else
					undoneIssues++;
			}

			debugText.text += "\nDone: "+doneIssues;
			debugText.text += "\nUndone: "+undoneIssues;


		} else {
			Debug.Log("No credentials");
		}



	}

	void CopyTextoToClipboard(string text){
		TextEditor te = new TextEditor();
		te.content = new GUIContent(text);
		te.SelectAll();
		te.Copy();
	}
}
