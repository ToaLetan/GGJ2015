using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour 
{
    private GameObject minutesObj = null;

    private GameObject[] secondsObj = new GameObject[2];

    private SpriteRenderer minutesTextRenderer = null;
    private SpriteRenderer secondsTensTextRenderer = null;
    private SpriteRenderer secondsOnesTextRenderer = null;

    private GameManager gameManager = null;

    private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () 
    {
        minutesObj = gameObject.transform.FindChild("Numbers_Mins").gameObject;
        secondsObj[0] = gameObject.transform.FindChild("Numbers_Secs_Tens").gameObject;
        secondsObj[1] = gameObject.transform.FindChild("Numbers_Secs_Ones").gameObject;

        minutesTextRenderer = minutesObj.GetComponent<SpriteRenderer>();
        secondsTensTextRenderer = secondsObj[0].GetComponent<SpriteRenderer>();
        secondsOnesTextRenderer = secondsObj[1].GetComponent<SpriteRenderer>();

        gameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

        UpdateDisplay(true);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.GameTimer.IsTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            UpdateDisplay();
        }
	}

    public void UpdateDisplay(bool initializeDisplay = false)
    {
        //Calculate the time remaining and display it accordingly.
        float remainingTime = 0;
        
        if(initializeDisplay)
            remainingTime = GameManager.ROUND_LENGTH;
        else
            remainingTime = GameManager.ROUND_LENGTH - gameManager.GameTimer.CurrentTime;

        int minute = Mathf.FloorToInt(remainingTime / 60);
        int secondsTens = Mathf.FloorToInt(remainingTime % 60) / 10;
        int secondsOnes = Mathf.FloorToInt(remainingTime % 60) % 10;

        //Display the time.
        minutesTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + minute);
        secondsTensTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + secondsTens);
        secondsOnesTextRenderer.sprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/New UI/UI_Text", "Num_" + secondsOnes);
    }
}
