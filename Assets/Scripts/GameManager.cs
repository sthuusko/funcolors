using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PersistentObject))]
public class GameManager: MonoBehaviour {

	public static GameManager instance;
	public bool gameStarted = false;
	
	void Start () {
		if(instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}
	


}
