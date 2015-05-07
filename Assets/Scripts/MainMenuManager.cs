using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	public void StartNewGame()
	{
		Application.LoadLevel("GameScene");
	}

	public void ExitGame()
	{
		Application.Quit ();
	}
}
