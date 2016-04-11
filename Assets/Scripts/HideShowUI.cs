using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HideShowUI : MonoBehaviour {
	RectTransform myRectTransform;
	Vector2 targetPosition;
	public bool shown;
	void Awake(){
		myRectTransform = GetComponent<RectTransform>();
	}
	public void Show(){
		
		shown = true;
	}
	public void Hide(){
		
		shown = false;
	}
	void Update(){
		if(shown)
			targetPosition = new Vector2(0,0);
		else
			targetPosition = new Vector2(700,0);

		myRectTransform.anchoredPosition = Vector2.Lerp(myRectTransform.anchoredPosition,targetPosition,Time.deltaTime*7f);
	}

}
