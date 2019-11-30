using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

/**
 *  Auth:   Jake Anderson
 *  Date:   25/10/2019
 *  Last:   08/11/2019
 *  Name:   Game Manager
 *  Vers:   2.0 - Implemented a better scoring system using a ScoreManager.
 */

public class GameManager : MonoBehaviour
{
	[Header("Players")]
	[Tooltip("This array holds each Player object.")]
	[SerializeField] private Player[] myPlayers;

	[Header("Ships")]
	//Each ship has the same functionality and the same ShipManager.
	//	This prefab is the base ship object to create when spawning ships.
	[Tooltip("This prefab is the base ship object to create when spawning ships.")]
	[SerializeField] private GameObject myTemplateShip;
	//The ships created for each player are held in this array.
	private ShipManager[] myShips;
	public ShipManager[] Ships { get { return myShips; } }

	[Header("Game Data")]
	[Tooltip("The goal to reach for a player to win.")]
	[SerializeField] private byte myScoreGoal;
	//We keep track of the previous round's winner to award them the special winner medal.
	private Player myRoundWinner;
	//Spawn points are on the east and west sides of the map.
	private Vector2[] mySpawnPointVectors;
	[Tooltip("Each player's displayed score is an instance of this prefab.")]
	[SerializeField] private GameObject myScoreHolderPrefab;

	[Header("Timers")]
	[Tooltip("The delay in seconds before the start of a new match.")]
	[SerializeField] private float myStartDelay;
	private WaitForSeconds myStartWait;
	[Tooltip("The delay in seconds at the end of a match.")]
	[SerializeField] private float myEndDelay;
	private WaitForSeconds myEndWait;

	//The world is an instance of this prefab.
	[Tooltip("Each generated world is an instance of this prefab.")]
	[SerializeField] private GameObject myWorld_Prefab;
	//The world is generated when the match start timer begins counting down.
	private GameObject myWorld_Instance;
	//The world manager script is used to tell the worlsd to generate.
	private WorldManager myWorld_Manager;

	//The message canvas is used to display messages and the player's scores.
	private GameObject myMessageCanvas;
	private Text myMessageCanvas_Text;

	//The ScoreManager handles scoring and tells the GameManager when there's a match winner.
	private ScoreManager myScoreManager;

	[Header("Texts")]
	[Tooltip("Holds all potentially chosen pre-game messages.")]
	[SerializeField] private string[] myPreGameMessages;

	[Header("UI")]
	[Tooltip("The Background sprite that appears before the logo.")]
	[SerializeField] private GameObject myBackgroundUIPrefab;
	[Tooltip("The Logo sprite.")]
	[SerializeField] private GameObject myLogoPrefab;
	private UIManager myMenuController;
	[Tooltip("The Audio object that is instantiated when the players are both ready to play.")]
	[SerializeField] private GameObject myReadyAudioPrefab;

	public static bool isPaused;

	private void Awake()
	{
		//myCameraRig = GetComponentInChildren<CameraController>();
		mySpawnPointVectors = new Vector2[2];
		mySpawnPointVectors[0] = new Vector2(-7, 0);
		mySpawnPointVectors[1] = new Vector2(7, 0);
		myMessageCanvas = GetComponentInChildren<Canvas>().gameObject;
		myMessageCanvas_Text = myMessageCanvas.GetComponentInChildren<Text>();
		myScoreManager = gameObject.AddComponent<ScoreManager>();
		
		for (int i = 0; i < myPlayers.Length; i++)
		{
			myPlayers[i].Index = (byte) i;
			myPlayers[i].Medals = new List<Medal>();
		}

		myStartWait = new WaitForSeconds(myStartDelay);
		myEndWait = new WaitForSeconds(myEndDelay);

		myMenuController = gameObject.AddComponent<UIManager>();
		myMenuController.Setup(myBackgroundUIPrefab, myLogoPrefab, myPlayers, myReadyAudioPrefab);
	}

	private void EnableShips()
	{
		for (int i = 0; i < myShips.Length; i++)
		{
			myShips[i].EnableControl();
		}
	}

	private void DisableShips()
	{
		for (int i = 0; i < myShips.Length; i++)
		{
			myShips[i].DisableControl();
		}
	}

	private void SetupShips()
	{
		myShips = new ShipManager[2];
		
		for (int i = 0; i < myPlayers.Length; i++)
		{
			//Create a new SpawnPoint vector3
			Vector3 newPosition = new Vector3(mySpawnPointVectors[i].x, mySpawnPointVectors[i].y, 0f);
			//Create a new Ship Manager game object.
			GameObject thisShip = Instantiate(myTemplateShip, this.transform) as GameObject;
			myShips[i] = thisShip.GetComponent<ShipManager>();
			//Tell the ship to setup its controllers.
			myShips[i].Setup(myPlayers[i], mySpawnPointVectors, myScoreManager);
			
			myShips[i].Instance = thisShip;
			//Position the ship to the spawnpoint.
			myShips[i].transform.position = newPosition;

            //Add this transform to the camera's Target Group.
            GameObject.Find("TargetGroup1").GetComponentInChildren<CinemachineTargetGroup>().AddMember(myShips[i].Instance.transform, 1, 1);
        }
	}

	private void SetupScoreManager()
	{
		Color[] colors = new Color[2];
		for (int i = 0; i < colors.Length; i++)
			colors[i] = myPlayers[i].Palette.Colors[0];

		myScoreManager.Setup(colors, myMessageCanvas, myScoreHolderPrefab, myScoreGoal);
	}

	private string EndMessage()
	{
		//By default, we want to return a "Draw!" message if we have no match winner.
		string ret = "IT'S A DRAW!";

		if (myRoundWinner != null)
			ret = myRoundWinner.ColoredName + " WINS!";

		return ret;
	}

	private IEnumerator BeginFadeOut()
	{
		yield return new WaitForSeconds(myEndDelay / 2);

		//Create the menu background "Fade" sprite.
		myMenuController.StartEndingSequence();

		//Tell the world manager that the match is ending.
		myWorld_Manager.EndMatch();

		//Clear the player win text.
		myMessageCanvas_Text.text = "";

		//Tell the score texts to fade out.
		myScoreManager.FadeOutScores();
	}

    //The match starts once both players have chosen their ship and are both ready.
    public void StartMatch()
    {
        //Create a new instance of the world prefab.
        myWorld_Instance = Instantiate(myWorld_Prefab, this.transform) as GameObject;
        //Get the world manager component.
        myWorld_Manager = myWorld_Instance.GetComponent<WorldManager>();

        StartCoroutine(MatchLoop());
    }

    private void EndMatch()
    {
        //Destroy the old world manager.
        myWorld_Manager = null;
        Destroy(myWorld_Instance);

        //Reset the player's scores.
        for (int i = 0; i < myPlayers.Length; i++)
            myPlayers[i].Score = 0;

        //Destroy each player's ship object as new ones will be generated for any other matches.
        for (int i = 0; i < myShips.Length; i++)
            Destroy(myShips[i].Instance);
		
		//Remove old score texts after they've faded out.
		myScoreManager.RemoveTexts();
	}

    private IEnumerator MatchStarting()
	{
        //Setup the ScoreManager.
        SetupScoreManager();
        //Create each player's ship for this match.
        SetupShips();
		DisableShips();
		myWorld_Manager.GenerateWorld();

		yield return myStartWait;
	}

	private IEnumerator MatchPlaying()
	{
        //Create scores
        myScoreManager.CreateScores();

		//At the beginning of the round, enable the Ship objects to allow player control.
		EnableShips();

		//Generate Asteroids.
		myWorld_Manager.GenerateAsteroids();

		//Clear the pre-game message text.
		myMessageCanvas_Text.text = string.Empty;
        //Tell the ScoreManager that the game is in progress.
        myScoreManager.MatchInProgress = true;

		//While a player hasn't met the score goal, we return null.
		while (!myScoreManager.PlayerMetGoal(myPlayers))
		{
			yield return null;
		}
	}

	private IEnumerator MatchEnding()
	{
		//At the end of the round, We tell the ships that the match is ending and use the end delay to allow them to leave the world.
		DisableShips();

		//Nullify the previous round winner and declare a new one.
		myRoundWinner = null;
        //Declare a round winner.
        //This is done by itterating through the player list and calling the winner based on which player has met the score goal.
        for (int i = 0; i < myPlayers.Length; i++)
            if (myPlayers[i].Score >= myScoreGoal)
            {
                myRoundWinner = myPlayers[i];
                break;
            }

		//Award the winner medal.
		Medal winnerMedal = new Medal("Winner", 1, "You won the last round!");
		myRoundWinner.Medals.Add(winnerMedal);

		//Create a new message to declare the winner of the match.
		string message = EndMessage();
		//Set the message.
		myMessageCanvas_Text.text = message;

		//Tell the ships that the match is ending.
		for (int i = 0; i < myShips.Length; i++)
			myShips[i].EndGame();

		StartCoroutine(BeginFadeOut());

		yield return myEndWait;
	}

	//The match loop is called from the start and holds the game loop together. Each phase of the game (start, play and end) runs one after the other.
	private IEnumerator MatchLoop()
	{
		yield return StartCoroutine(MatchStarting());

		yield return StartCoroutine(MatchPlaying());

		yield return StartCoroutine(MatchEnding());

		//After all phases are complete...
		EndMatch();
	}

	//When a player pauses the game, time is scaled down (or up when resumed).
	private IEnumerator ScaleTime(float aStartTimescale, float anEndTimescale, float aTimeframe)
	{
		float lastTime = Time.realtimeSinceStartup;
		float timer = 0.0f;

		while (timer < aTimeframe)
		{
			Time.timeScale = Mathf.Lerp(aStartTimescale, anEndTimescale, timer / aTimeframe);
			Time.fixedDeltaTime = Time.timeScale * 0.02f;
			timer += (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}

		Time.timeScale = anEndTimescale;
		Time.maximumDeltaTime = anEndTimescale;
	}

	public void Pause()
	{
		myMessageCanvas_Text.text = "Paused";

		StartCoroutine(ScaleTime(1.0f, 0.0f, 0.5f));

		isPaused = true;
	}

	public void Resume()
	{
		myMessageCanvas_Text.text = "";

		StartCoroutine(ScaleTime(0.0f, 1.0f, 0.5f));

		isPaused = false;
	}

	private void Start()
	{
		myMenuController.StartOpeningSequence();
	}
}
