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
	public static GameObject fishPrefab;
	void Awake(){
		issues = new List<Issue>();
		if(fishPrefab == null){
			fishPrefab = Resources.Load("Fish") as GameObject;
		}
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

			double maxUnsolved = -1;
			double minUnsolved = 999999;
			int doneIssues = 0;
			int undoneIssues = 0;
			for(int i = 0; i < issues.Count; i++){
				
				if(issues[i].solved){
					doneIssues++;
				}
				else{
					undoneIssues++;
					if(issues[i].unsolvedMinutes > maxUnsolved)
						maxUnsolved = issues[i].unsolvedMinutes;
					if(issues[i].unsolvedMinutes < minUnsolved)
						minUnsolved = issues[i].unsolvedMinutes;
				}
			}
			debugText.text += "\nDone: "+doneIssues;
			debugText.text += "\nUndone: "+undoneIssues;

			for(int i = 0; i < issues.Count; i++){
				GameObject newFish = Instantiate(fishPrefab,new Vector3(i % 10,i / 10,0),Quaternion.identity) as GameObject;
				newFish.GetComponent<Fish>().Initialize(issues[i],minUnsolved,maxUnsolved);
			}

		} else {
			Debug.Log("No credentials");
		}



	}

	void CopyTextoToClipboard(string text){
		TextEditor te = new TextEditor();
		te.text = text;
		te.SelectAll();
		te.Copy();
	}
}
