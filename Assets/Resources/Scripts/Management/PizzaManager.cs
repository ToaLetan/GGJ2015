﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PizzaManager : MonoBehaviour 
{
    private const float MOVE_SPEED = 180.0f; //How fast the tray is initially moved down from the top of the screen
    private const float SLIDE_MOVE_SPEED = 240.0f; //How fast the tray is moved to the side
    private const float DECELERATION_SPEED = 60.0f; //How much the tray slows down when slid from the top
    private const float SPAWN_POS_Y = 272.0f;
    private const float MAX_DECELERATION = MOVE_SPEED - 5.0f; //The slowest the tray can move by towards the end
    private const float DESPAWN_RANGE = 175.0f;

    GameObject[] pizzaTrays = new GameObject[2];

    private Vector3[] trayPositions = new Vector3[2];

    private int[] previousNumSlices = new int[2];

    private bool[] haveReachedDestinations = new bool[2];
    private bool[] markedForDeletion = new bool[2];

    private float[] deceleration = new float[2];

    private GameManager gameManager = null;

	// Use this for initialization
	void Start () 
    {
        pizzaTrays = GameObject.FindGameObjectsWithTag("Platter");

        for(int i = 0; i < pizzaTrays.Length; i++)
        {
            previousNumSlices[i] = pizzaTrays[i].transform.childCount;
            trayPositions[i] = pizzaTrays[i].transform.position;
            haveReachedDestinations[i] = true;
            markedForDeletion[i] = false;
            deceleration[i] = 0;
        }

        gameManager = gameObject.GetComponent<GameManager>(); //This is attached to the game manager.
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(gameManager.IsGamePaused == false)
            UpdateNumSlices();
	}

    private void UpdateNumSlices()
    {
        for (int i = 0; i < pizzaTrays.Length; i++)
        {
            if (pizzaTrays[i] != null)
            {
                previousNumSlices[i] = pizzaTrays[i].transform.childCount;

                if (pizzaTrays[i].transform.childCount <= 0) //We're out of pizza, spawn a new one
                    markedForDeletion[i] = true;

                if (haveReachedDestinations[i] == false) //Slide the pizza to the destination
                    SlidePizzaTrayDown(i);

                if (markedForDeletion[i] == true) //Remove the pizza
                    SlidePizzaTraySide(i);
            }
        }
    }

    private void SpawnNewPizza(int trayNum)
    {
        Vector3 trayPos = trayPositions[trayNum];
        trayPos.y = SPAWN_POS_Y;

        pizzaTrays[trayNum] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PizzaPlatter"), trayPos, Quaternion.identity) as GameObject;

        haveReachedDestinations[trayNum] = false;
    }

    private void SlidePizzaTrayDown(int trayNum)
    {
        Vector3 newPos = pizzaTrays[trayNum].transform.position;

        if (deceleration[trayNum] < MAX_DECELERATION)
            deceleration[trayNum] += DECELERATION_SPEED * Time.deltaTime;

        float currentSpeed = (MOVE_SPEED - deceleration[trayNum]) * Time.deltaTime * -1;

        newPos.y += currentSpeed;

        pizzaTrays[trayNum].transform.position = newPos;

        if (pizzaTrays[trayNum].transform.position.y <= trayPositions[trayNum].y)
        {
            haveReachedDestinations[trayNum] = true;
            deceleration[trayNum] = 0;
        }
    }

    private void SlidePizzaTraySide(int trayNum)
    {
        Vector3 newPos = pizzaTrays[trayNum].transform.position;

        int direction = 0;

        if (pizzaTrays[trayNum].transform.position.x > 0) //Slide right
            direction = 1;
        else //Slide left
            direction = -1;

        newPos.x += MOVE_SPEED * Time.deltaTime * direction;

        pizzaTrays[trayNum].transform.position = newPos;

        if (direction == 1)
        {
            if (pizzaTrays[trayNum].transform.position.x > trayPositions[trayNum].x + DESPAWN_RANGE)
            {
                markedForDeletion[trayNum] = false;
                GameObject.Destroy(pizzaTrays[trayNum]);
                SpawnNewPizza(trayNum);
                deceleration[trayNum] = 0;
            }
        }
        if (direction == -1)
        {
            if (pizzaTrays[trayNum].transform.position.x < trayPositions[trayNum].x - DESPAWN_RANGE)
            {
                markedForDeletion[trayNum] = false;
                GameObject.Destroy(pizzaTrays[trayNum]);
                SpawnNewPizza(trayNum);
                deceleration[trayNum] = 0;
            }
        }   
    }

    public void ResetPizzaSpawns()
    {
        for (int i = 0; i < pizzaTrays.Length; i++)
        {
            GameObject.Destroy(pizzaTrays[i]);

            pizzaTrays[i] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PizzaPlatter"), trayPositions[i], Quaternion.identity) as GameObject;

            previousNumSlices[i] = pizzaTrays[i].transform.childCount;
            trayPositions[i] = pizzaTrays[i].transform.position;
            haveReachedDestinations[i] = true;
            markedForDeletion[i] = false;
            deceleration[i] = 0;
        }
    }
}
