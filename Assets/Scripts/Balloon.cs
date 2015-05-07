using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Balloon : MonoBehaviour {

	public int index = 0;  // index in GameController BalloonStruct
	public Text colorTextObject;
	//public GameObject colorTextObject;
	public Canvas canvas;
	public int balloonColorIndex; // balloons color index in SceneManagar's color lists
	public SceneManager sceneInstance;
	public ParticleEmitter[] particleEffects;
		
	void OnMouseDown()
	{
		Debug.Log("onmousedown");
		if (balloonColorIndex == sceneInstance.selectedColorIndex)
		{
			sceneInstance.AddPoint();
		}
		else
		{
			sceneInstance.AddFault();
		};
		

		if (gameObject != null)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			Text instance = Instantiate (colorTextObject, pos, rot) as Text;
			instance.transform.SetParent(canvas.transform, false);
			instance.enabled = true;
			instance.color = sceneInstance.balloonColors[balloonColorIndex];  
			instance.text = sceneInstance.balloonColorStrings[balloonColorIndex];
			Destroy(instance.gameObject,2f);
			if (balloonColorIndex == sceneInstance.selectedColorIndex)
			{
				RandomizeEffect (pos, rot);
			}
			DestroyBalloon();
			if (!(SoundManager.instance.efxSource.isPlaying && (sceneInstance.previousClickedColorIndex == balloonColorIndex)))
			{
				AudioClip clip1 = sceneInstance.colorAudioClips[balloonColorIndex*2];
				AudioClip clip2 = sceneInstance.colorAudioClips[balloonColorIndex*2+1];
				SoundManager.instance.RandomizeSfx(clip1, clip2);
			}
			sceneInstance.previousClickedColorIndex = balloonColorIndex;
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		DestroyBalloon();
	}
	
	private void DestroyBalloon()
	{
		if (gameObject != null)
		{
			Destroy (gameObject);
			sceneInstance.numberOfExistingBalloons--;
			sceneInstance.balloons[index] = null;
		}
	}
	
	public void RandomizeEffect (Vector3 pos, Quaternion rot)
	{
		int randomIndex = Random.Range(0, particleEffects.Length);
		ParticleEmitter  effect = Instantiate (particleEffects[randomIndex], pos, rot) as ParticleEmitter;
		ParticleAnimator effectAnimator = effect.GetComponent<ParticleAnimator>();
		effectAnimator.autodestruct = true;
		effect.Emit();
	}
	
}