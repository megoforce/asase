using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class Issue {
	public string project;
	public double unsolvedMinutes;
	public bool solved;
	public string key;
	public Issue(JSONObject issueInfo){
		
		solved = (JSONObject.HaveFieldAndNotNull(issueInfo["fields"]["resolution"],"name") && issueInfo["fields"]["resolution"]["name"].str == "Done");
		project = issueInfo["fields"]["project"]["name"].str;
		key = issueInfo["key"].str;
		if(!solved){
			DateTime created = DateTime.Parse(issueInfo["fields"]["created"].str);
			unsolvedMinutes = (DateTime.Now - created.Date).TotalMinutes;
		} else {
			unsolvedMinutes = 0;
		}

	}
}
