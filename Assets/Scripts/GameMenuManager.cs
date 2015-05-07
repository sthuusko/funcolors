using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {

	public GameObject escapeMenuPanel;
	public GameObject gameFailedMenuPanel;
	public GameObject nextLevelMenuPanel;
	public GameObject startLevelMenuPanel;
	public Text failedScoreText;
	public Text nextLevelScoreText;
	public Text levelCompletedText;
	public Text levelFailedText;
	public Text startLevelText;
	public Image balloonImage;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			escapeMenuPanel.SetActive(!escapeMenuPanel.activeInHierarchy);
		}
	}

	public void ReturnToMainMenu()
	{
		Time.timeScale = 1;
		GameManager.instance.gameStarted = false;
		Application.LoadLevel("MainMenuScene");
	}

	public void ShowGameFailedMenu(int score, int totalScore, int level)
	{
		Time.timeScale = 0;
		levelFailedText.text = "Level " + level + " failed";
		failedScoreText.text = "You got " + score + " level points. Your total score is " + totalScore;
		gameFailedMenuPanel.SetActive(true);
	}
	
	public void ShowNextLevelMenu(int score, int totalScore, int level)
	{
		Time.timeScale = 0;
		levelCompletedText.text = "Level " + level + " completed";
		nextLevelScoreText.text = "You got " + score + " points. Your total score is " + totalScore;;
		nextLevelMenuPanel.SetActive(true);
	}

	public void ShowStartLevelMenu(string levelText, Color selectedColor)
	{
		startLevelText.text = levelText;
		balloonImage.color = selectedColor;
		startLevelMenuPanel.SetActive(true);
	}
	
}
