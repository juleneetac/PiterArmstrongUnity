﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour   
{
    //Piter Armstrong

   
    // Start is called before the first frame update
    public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.
    public int playerVida= 100;                             //Starting value for Player food points.
    public int playerHerramienta = 0;

    [HideInInspector]public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.

    private Text levelText;						   //Text to display current level number.
    private GameObject levelImage;				   //Image to block out level as levels are being set up, background for levelText.
    private int level = 0;                         //Current level number, expressed in game as "Day 3".
    private List<Enemy> enemies;			       //List of all Enemy units, used to issue them move commands.
    private bool enemiesMoving;					   //Boolean to check if enemies are moving.
    private bool doingSetup;							//Boolean to check if we're setting up board, prevent Player from moving during setup.

    // Start is called before the first frame update

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    private void OnLevelWasLoaded(int index)    //esta bien ahora
    {
        level++;
        InitGame();
    }


    void InitGame()
    {
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        //Get a reference to our image LevelImage by finding it by name.
        levelImage = GameObject.Find("LevelImage");

        //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        //Set the text of levelText to the string "Day" and append the current level number.
        levelText.text = "Mapa " + level;

        //Set levelImage to active blocking player's view of the game board during setup.
        levelImage.SetActive(true);

        //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
        Invoke("HideLevelImage", levelStartDelay);

        //Clear any Enemy objects in our List to prepare for next level.
        enemies.Clear();
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);
        
    }

    //Hides black image used between levels
    void HideLevelImage()
    {
        //Disable the levelImage gameObject.
        levelImage.SetActive(false);

        //Set doingSetup to false allowing player to move again.
        doingSetup = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (playersTurn || enemiesMoving || doingSetup)
        {
            //If any of these are true, return and do not start MoveEnemies.
            return;
        }

        //Start moving enemies.
        StartCoroutine(MoveEnemies());

    }

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }
    

    //Coroutine to move enemies in sequence.
    IEnumerator MoveEnemies()
    {
        //While enemiesMoving is true player is unable to move.
        enemiesMoving = true;

        //Wait for turnDelay seconds, defaults to .1 (100 ms).
        yield return new WaitForSeconds(turnDelay);

        //If there are no enemies spawned (IE in first level):
        if (enemies.Count == 0)
        {
            //Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
            yield return new WaitForSeconds(turnDelay);
        }

        //Loop through List of Enemy objects.
        for (int i = 0; i < enemies.Count; i++)
        {
            //Call the MoveEnemy function of Enemy at index i in the enemies List.
            enemies[i].MoveEnemy();

            //Wait for Enemy's moveTime before moving next Enemy, 
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        //Once Enemies are done moving, set playersTurn to true so player can move.
        playersTurn = true;

        //Enemies are done moving, set enemiesMoving to false.
        enemiesMoving = false;
    }

   


    public void GameOver()
    {
      

        //Set levelText to display number of levels passed and game over message
       levelText.text = "Has muerto en el mapa " + level ;

        //Enable black background image gameObject.
       levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }
    public void TheEnd()
    {
        levelText.text = "Has reparado la nave y has huido. THE END";
        //Enable black background image gameObject.
        levelImage.SetActive(true);

        //Disable this GameManager.
        enabled = false;
    }

    public void Fracaso()
    {
        levelText.text = "Has fracasado te dejaste alguna herramienta por el camino";
        //Enable black background image gameObject.
        levelImage.SetActive(true);
        
    }

}
