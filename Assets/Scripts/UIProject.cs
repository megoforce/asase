using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIProject : MonoBehaviour {
	[SerializeField] Image myImage;
	[SerializeField] Text myText;
	public void SetProject(string text, Color color){
		myImage.color = color;
		myText.text = text;
	}

}
