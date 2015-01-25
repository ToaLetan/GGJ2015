using UnityEngine;
using System.Collections;

public class GameTimer : MonoBehaviour 
{
    private GameObject minutesObj = null;

    private GameObject[] secondsObj = new GameObject[2];

    private GameManager gameManager = null;

    private float elapsedTime = 0.0f;

	// Use this for initialization
	void Start () 
    {
        minutesObj = gameObject.transform.FindChild("Numbers_Mins").gameObject;
        secondsObj[0] = gameObject.transform.FindChild("Numbers_Secs_Tens").gameObject;
        secondsObj[1] = gameObject.transform.FindChild("Numbers_Secs_Ones").gameObject;

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

    private void UpdateDisplay(bool initializeDisplay = false)
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
        minutesObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Number_" + minute);
        secondsObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Number_" + secondsTens);
        secondsObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Text_Number_" + secondsOnes);
    }
}
