using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {
	string server = "";
	string credentials = "";
	[SerializeField]
	List<Fish> fishes;

	[SerializeField] Text debugText;
	public static GameObject fishPrefab;
	void Awake(){
		fishes = new List<Fish>();
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
			string url = server+"/rest/api/2/search?jql=resolution+=+Unresolved+ORDER+BY+updatedDate+DESC";
			headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
				System.Text.Encoding.ASCII.GetBytes(credentials));

			WWW www = new WWW(url, null, headers);
			yield return www;

//			debugText.text = www.text;
//			Debug.Log(www.text);
			JSONObject k = new JSONObject(www.text);

			debugText.text = "";
			debugText.text += "Total issues: "+k["issues"].Count;
			CopyTextoToClipboard(www.text);


			double maxUnsolved = -1;
			double minUnsolved = 999999;
			for(int i = 0; i < k["issues"].Count; i++){
				if(!FishAlreadyExist(k["issues"][i]["key"].str)){
					GameObject newFish = Instantiate(fishPrefab,new Vector3(i % 10,i / 10,0),Quaternion.identity) as GameObject;
					newFish.GetComponent<Fish>().Initialize(new Issue(k["issues"][i]));
					fishes.Add(newFish.GetComponent<Fish>());

					if(fishes[i].issue.unsolvedMinutes > maxUnsolved)
						maxUnsolved = fishes[i].issue.unsolvedMinutes;
					if(fishes[i].issue.unsolvedMinutes < minUnsolved)
						minUnsolved = fishes[i].issue.unsolvedMinutes;
					
				}
			}
			for(int i = 0; i < fishes.Count; i++){
				fishes[i].NormalizeSize(minUnsolved,maxUnsolved);
			}




		} else {
			Debug.Log("No credentials");
		}



	}
	bool FishAlreadyExist(string key){
		for(int i = 0; i < fishes.Count; i++)
			if(fishes[i].issue.key == key)
				return true;

		return false;
			
		
	}

	void CopyTextoToClipboard(string text){
		TextEditor te = new TextEditor();
		te.text = text;
		te.SelectAll();
		te.Copy();
	}
}
