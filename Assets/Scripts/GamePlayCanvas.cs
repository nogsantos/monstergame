using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayCanvas : MonoBehaviour {

	public static GamePlayCanvas instance;
	public GameObject directionalLight;
	public Monster[] monsters;
	public Text txtPages;
	public string pagesString;
	public int pagesTotal = 4;
	public int pagesFound = 0;


	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		updateCanvas ();
	}

	public void updateCanvas(){
		this.pagesString = "Pages " + this.pagesFound.ToString()+"/"+this.pagesTotal.ToString();
		this.txtPages.text = this.pagesString; 
	}

	public void findPage(){
		this.pagesFound++;
		this.updateCanvas ();

		if(this.pagesFound >= this.pagesTotal){
			this.directionalLight.SetActive (true);

			for(int n = 0; n < this.monsters.GetLength(0); n++){
				this.monsters [n].death ();
			}
		}
	}
		
}
