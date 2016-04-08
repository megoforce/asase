using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class Issue {
	public string project;
	public DateTime created;
	public string createdStr;
	public bool solved;
	public Issue(JSONObject issueInfo){
		solved = (JSONObject.HaveFieldAndNotNull(issueInfo["fields"]["resolution"],"name") && issueInfo["fields"]["resolution"]["name"].str == "Done");
		project = issueInfo["fields"]["project"]["name"].str;
		created = DateTime.Parse(issueInfo["fields"]["created"].str);

	}
}
