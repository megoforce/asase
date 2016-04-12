using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIProject : MonoBehaviour {
	[SerializeField] Text myText;
	public void SetProject(string text){
		myText.text = text;
	}

}
