﻿using UnityEngine;
using System.Collections;

using System.Collections.Generic;       //Allows us to use Lists. 

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 3;                                  //Current level number, expressed in game as "Day 1".
    public int playerHP = 100;
    public int playerDodgesMax = 4;
    public int playerDamage = 100;
    public float playerSpeed = 2.0f;
    public float dodgeDistance = 1.0f;
    public float dodgeSpeed = 6.0f;


    public int enemyDamage = 100;
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;                 //if not, set instance to this


        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);        //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.



        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);


        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {


    }

    public void GameOver()
    {
        Debug.Log("u ded");
    } 



    //Update is called every frame.
    void Update()
    {

    }

}