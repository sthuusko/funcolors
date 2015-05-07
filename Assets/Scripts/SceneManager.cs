using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

	public GameObject balloon;
	public GameObject destroyingCollider;
	public Transform statusPanel;
	public Text pointText;
	public Text levelText;
	
	public int numberOfBalloons = 5;
	public int selectedColorIndex = 8;
	public int previousSelectedColorIndex = 8;
	public int previousClickedColorIndex;
	public int numberOfSelectedColorBalloons = 3;
	public float levelSeconds = 20f;
	public float minGravity = -1.0f;
	public float maxGravity = -0.1f;
	public float minSizeScaleMultiplier = 0.5f;
	public float maxSizeScaleMultiplier = 1.0f;
	
	public float levelGravityIncrease = 0.5f;
	public int levelNOFBalloonsIncrease = 5;
	public int levelNOFSelectedBalloonsIncrease = 3;
	
//	public struct BalloonStruct {
//		public float instanceTime;		// the time when the balloon is shown
//		public int balloonColorIndex;	// color index of the balloon
//		public Vector3 position;		// position of the balloon
//		public float gravityScale;		// gravityscale of the balloon, others are faster
//		public Vector3 sizeScale;		// gravityscale of the balloon, others are faster
//	}
	
	public class BalloonClass {
		public float instanceTime;		// the time when the balloon is shown
		public int balloonColorIndex;	// color index of the balloon
		public Vector3 position;		// position of the balloon
		public float gravityScale;		// gravityscale of the balloon, others are faster
		public Vector3 sizeScale;		// gravityscale of the balloon, others are faster
	}
	
	public int numberOfExistingBalloons = 0;
	public Image exampleBalloon;
	public Image faultBalloon1;
	public Image faultBalloon2;
	public Image faultBalloon3;
	
	private int levelPoints = 0;
	private int totalPoints = 0;
	private int faults = 0;
	private int level = 1;
	private float startTime;
	private static Color32 brown 	= new Color32(150,75,0,128);
	private static Color32 orange 	= new Color32(255,165,0,128);
	private static Color32 purple 	= new Color32(128,0,128,128);
	private static Color32 pink 	= new Color32(255,192,203,128);
	private static Color32 yellow 	= new Color32(255,255,0,128);
	private static Color32 black 	= new Color32(50,50,50,128);
	private static Color32 gray 	= new Color32(150,150,150,128);
	
	// define the colors used in the balloons
	public Color[] balloonColors = new Color[] {black, 
		Color.blue, brown, Color.green, gray,
		orange,pink, purple,Color.red, Color.white, yellow};
	
	// define the color names used in the balloons, use the same order as above!
	public String[] balloonColorStrings = new String[] {"black", 
		"blue", "brown", "green", "gray", "orange", "pink", "purple","red", "white", "yellow"}; 
		
	public AudioClip[] colorAudioClips = new AudioClip[22] ;  // 2d arrays don't show in inspector, that's why we have double sized 1d array
	
	public BalloonClass[] balloons;
	public GameMenuManager gameMenuManager;
	
	private int balloonsInstantiated = 0;
	
	private float maxX;  // maximum X axis position where balloons appear
	private float maxY;  // maximum Y axis position, used for setting the place for balloon destroying collider
	private float posY;  // Y axis position where balloons appear
	
	
	void Awake () {
		Camera cam = Camera.main;
		Vector3 maxPoint = new Vector3 (cam.pixelWidth,cam.pixelHeight,0f);  
		
		// set the place for balloon destorying collider over the screen
		//Debug.Log("cam.pixelHeight = " + cam.pixelHeight + "cam.ScreenToWorldPoint(maxPoint).y " + cam.ScreenToWorldPoint(maxPoint).y);
		float colliderYPos = cam.ScreenToWorldPoint(maxPoint).y + balloon.renderer.bounds.size.y + destroyingCollider.collider2D.bounds.size.y/2;
		//Debug.Log(" colliderYPos" +  colliderYPos);
		Vector3 collYPos = new Vector3(destroyingCollider.transform.position.x, colliderYPos, destroyingCollider.transform.position.z); 
		destroyingCollider.transform.position = collYPos;
		maxX = cam.ScreenToWorldPoint(maxPoint).x; //- balloon.renderer.bounds.extents.x;//- balloon.renderer.bounds.size.x/2;
		Debug.Log (balloon.renderer.bounds);
		RectTransform panelRectTransform = statusPanel.GetComponent<RectTransform>();
		
		// set position for instantiaing balloons below the screen
		posY = cam.ScreenToWorldPoint(new Vector3(0f,panelRectTransform.sizeDelta.y,0f)).y;
		Debug.Log (cam.ScreenToWorldPoint(maxPoint).y + " " 
		           + panelRectTransform.sizeDelta.y + " " 
		           + cam.ScreenToWorldPoint(new Vector3(0f,panelRectTransform.sizeDelta.y,0f)).y);
		           
//		balloons = new BalloonClass[numberOfBalloons];
//		for (int i=0; i < numberOfBalloons ; i++)
//		{
//			balloons[i] = new BalloonClass();
//		}
	}
	
	// Use this for initialization
	void Start () {
		if (!GameManager.instance.gameStarted)
		{
			ShowStartLevelPanel();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance.gameStarted)
		{
			if (balloonsInstantiated < numberOfBalloons)
			{
				//Debug.Log(balloons.Length + " " + balloonsInstantiated);
				if ((startTime + balloons[balloonsInstantiated].instanceTime) < Time.time)
				{
					Vector3 pos = balloons[balloonsInstantiated].position;
					Quaternion rot = transform.rotation;
					GameObject instance = Instantiate (balloon, pos, rot) as GameObject;
					Balloon balloonInstance = instance.GetComponent<Balloon>();
					balloonInstance.sceneInstance = this;
					balloonInstance.index = balloonsInstantiated;
					balloonInstance.balloonColorIndex = balloons[balloonsInstantiated].balloonColorIndex;
					instance.renderer.material.color = balloonColors[balloons[balloonsInstantiated].balloonColorIndex];
					instance.rigidbody2D.gravityScale = balloons[balloonsInstantiated].gravityScale;
					instance.transform.localScale  = balloons[balloonsInstantiated].sizeScale;
					balloonsInstantiated++;
					numberOfExistingBalloons++;
				}
			}
			else
			{
				if (balloonsInstantiated == numberOfBalloons)
				{
					if (numberOfExistingBalloons == 0)
						{
							GameManager.instance.gameStarted = false;
							if (levelPoints >= (numberOfSelectedColorBalloons/2f))//level completed
								ShowNextLevelMenu();
							else
							{
								GameFailed();	
							}
						}
				}
			}
		}
	}
	
	private void CreateBalloons () {
		
		balloons = new BalloonClass[numberOfBalloons];
		
		for (int i = 0; i < numberOfBalloons; i++) 
		{
		
			balloons[i] = new BalloonClass();		
		
			// set default values
			balloons[i].balloonColorIndex = -1;  // default color

			if (i == 0)
			{
				// set balloons instance time, place and gravityscale
				// first balloon appears immediately and fast, other balloons randomly 
				balloons[0].instanceTime = 0f;
				balloons[0].gravityScale = minGravity;
			}
			else
			{
				// set random instance time 
				float instTime = UnityEngine.Random.Range (0f,levelSeconds);
				balloons[i].instanceTime = instTime;

				// set random gravity
				float gravScale = UnityEngine.Random.Range (minGravity,maxGravity);
				balloons[i].gravityScale = gravScale;
			}
			
			// set random size
			float sizeScaleMultiplier = UnityEngine.Random.Range (minSizeScaleMultiplier, maxSizeScaleMultiplier);
			Vector3 sizeScale = new Vector3(balloon.transform.localScale.x * sizeScaleMultiplier, balloon.transform.localScale.y * sizeScaleMultiplier, 1);
			balloons[i].sizeScale = sizeScale;

			// set random position
			Vector3 pos = new Vector3(0f,0f,100f - 0.01f*i);  //z value has to be set so that OnMouseClick event recognizes the top mos sprite
			pos.x = UnityEngine.Random.Range (-maxX, maxX);
			Debug.Log (balloon.renderer.bounds.size.y *sizeScaleMultiplier);
			//pos.y = posY; // - (balloon.renderer.bounds.size.y * sizeScaleMultiplier)/2;
			pos.y = posY - (balloon.renderer.bounds.size.y * sizeScaleMultiplier)/2;
			//pos.y = posY - (balloon.renderer.bounds.size.y)/2;
			balloons[i].position = pos;
			
						
		}
		
		// sort the balloon array ordered by instance time
		Array.Sort<BalloonClass>(balloons, (x,y) => x.instanceTime.CompareTo(y.instanceTime));
		
		if (numberOfSelectedColorBalloons > numberOfBalloons)      // just for making sure 
			numberOfSelectedColorBalloons = numberOfBalloons;  
			
		// set balloon colors
		// set "selected color" balloons first
		for (int i=0; i < numberOfSelectedColorBalloons; i++) 
		{
			int index = UnityEngine.Random.Range(0,numberOfBalloons);
			if (balloons[index].balloonColorIndex == -1)	// value not set yet
			{
				balloons[index].balloonColorIndex = selectedColorIndex;
			}
			else
			{
				i--;  
			}
		}
		
		// set rest of the balloon colors randomly
		for (int i=0; i < numberOfBalloons; i++) 
		{
			if (balloons[i].balloonColorIndex == -1)	// value not set yet
			{
				int index = UnityEngine.Random.Range(0,balloonColors.Length);
				if (index != selectedColorIndex)
				{
					balloons[i].balloonColorIndex = index;
				}
				else 
				{
					i--; 
				}
			}
		}
	}
	
	public void AddPoint()
	{
		levelPoints ++;
		totalPoints++;
		pointText.GetComponent<Text>().text = levelPoints.ToString() + "/"+numberOfSelectedColorBalloons;
		SoundManager.instance.RandomizeSfx2(SoundManager.instance.pop);
	}
	
	public void AddFault()
	{
		faults ++;
		SoundManager.instance.RandomizeSfx2(SoundManager.instance.deflate);
		
		switch (faults)
		{
		
			case 1:
				faultBalloon1.color = Color.gray;
				break;
			case 2:
				faultBalloon2.color = Color.gray;
			break;
			case 3:
				faultBalloon3.color = Color.gray;
				GameFailed();
			break; 	
			default:
				break;
		}
	}
	
	private void GameFailed()
	{
		// ilmoitetaan lopputulos GameFailed
		GameManager.instance.gameStarted = false;
		gameMenuManager.ShowGameFailedMenu(levelPoints, totalPoints, level);
	}
	
	private void ShowNextLevelMenu()
	{
		// ilmoitetaan lopputulos ja Next Level
		gameMenuManager.ShowNextLevelMenu(levelPoints, totalPoints, level);
	}
	
	public void StartNextLevel()
	{
		level++;
		minGravity -= levelGravityIncrease;
		maxGravity -= levelGravityIncrease;
		numberOfBalloons += levelNOFBalloonsIncrease;
		numberOfSelectedColorBalloons += levelNOFSelectedBalloonsIncrease;

		ShowStartLevelPanel();
	}
	
	public void ShowStartLevelPanel()
	{
		// reset counters
		balloonsInstantiated = 0;
		numberOfExistingBalloons = 0;
		levelPoints = 0;
		faults = 0;
				
		// pick random color, different than in last level
		int index = previousSelectedColorIndex;
		while (index == previousSelectedColorIndex)
			index = UnityEngine.Random.Range(0,balloonColors.Length);
		
		selectedColorIndex = index;
		previousSelectedColorIndex = index;
		
		// reset status panel values
		exampleBalloon.color = balloonColors [selectedColorIndex]; 
		faultBalloon1.color = Color.white;
		faultBalloon2.color = Color.white;
		faultBalloon3.color = Color.white;
		pointText.text = levelPoints.ToString() + "/"+numberOfSelectedColorBalloons;
		levelText.text = "Level " + level.ToString();
		
		//set start level menu values
		string levText =  "Level " + level + ": Collect " + balloonColorStrings[selectedColorIndex] + " balloons";
		Color selColor = balloonColors[selectedColorIndex];
		gameMenuManager.ShowStartLevelMenu(levText, selColor);
		
		//play selected color audio
		AudioClip clip1 = colorAudioClips[selectedColorIndex*2];
		AudioClip clip2 = colorAudioClips[selectedColorIndex*2+1];
		SoundManager.instance.RandomizeSfx(clip1, clip2);
		
	}
	
	public void StartGame()
		{
			CreateBalloons ();
			GameManager.instance.gameStarted = true;
			Time.timeScale = 1;
			startTime = Time.time;
		}
}
 