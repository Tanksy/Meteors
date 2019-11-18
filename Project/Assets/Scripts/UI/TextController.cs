using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *  Auth:   Jake Anderson
 *  Date:   27/10/2019
 *  Last:   08/11/2019
 *  Name:   Text Controller
 *  Vers:   2.0 - Text Controllers are now handled by a ScoreManager. TextController serves to shrink scale and lerp color if not default.
 */

public class TextController : MonoBehaviour
{
	//The text component is updated to display the Score when the score method is called.
	private Text myText;

	//The text controller tracks the player's score and increases it by 1 each time the score method is called.
	private byte myScore;

	//The player number informs the ScoreTextManager which player we are displaying the score of.
	private byte myPlayer_Number;
	public byte Player { get { return myPlayer_Number; } }

	//The text color is set to this when the score method is called. The update method then lerps the color to White.
	private Color myPlayer_Color;

	//If true, the text will lose opacity over time.
	private bool isFadingOut;
	public bool FadingOut { set { isFadingOut = value; } }

	private GameObject myInstance;
	public GameObject Instance { get { return myInstance; } set { myInstance = value; } }

	private void Awake()
	{
		myScore = 0;
		myText = GetComponent<Text>();
	}

	public void Setup(Color aColor, Vector2 aPosition)
	{
		myPlayer_Color = aColor;

		transform.localPosition = aPosition;
	}

	public void Score()
	{
		myScore++;
		myText.text = myScore.ToString();
		myText.color = myPlayer_Color;
		transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void ResetText()
	{
		myScore = 0;
		myText.text = "0";
	}

	private void Update()
	{
		if (transform.localScale.x > 0.25f)
		{
			Vector3 newScale = new Vector3(transform.localScale.x - 1f * Time.deltaTime, transform.localScale.y - 1f * Time.deltaTime, 1f);
			transform.localScale = newScale;
		}
		else
		{
			Vector3 newScale = new Vector3(0.25f, 0.25f, 1f);
			transform.localScale = newScale;
		}
		if (myText.color != Color.white)
			myText.color = Color.Lerp(myText.color, Color.white, Time.deltaTime);

		//Lower opacity over time.
		if (isFadingOut)
			//We only want to reduce the opacity whilst it's above 0.
			if (myText.color.a > 0f)
			{
				Color newColor = myText.color;
				newColor.a -= 1.33f * Time.deltaTime;
				myText.color = newColor;
			}
			//If our alpha is <= 0 then we're done fading out.
			else
				isFadingOut = false;
	}
}
