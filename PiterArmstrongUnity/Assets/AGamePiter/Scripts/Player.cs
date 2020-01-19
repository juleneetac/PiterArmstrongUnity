using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    //Piter Armstrong

    //public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
    public int pointsPerAgua = 5;
    public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
    public int pointsPerFood2 = 20;
    public int numHerramienta = 1;
    //Number of points to add to player food points when picking up a soda object.
    public Text vidaText;                       //UI Text to display current player food total.
    public Text encontradoText;



    public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
    private Animator animator;					//Used to store a reference to the Player's animator component.
    private int vida;                           //Used to store player food points total during level.
    private int herramienta;
    

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();
        animator.SetTrigger("playerRun");
        //Get the current food point total stored in GameManager.instance between levels.
        vida = GameManager.instance.playerVida;     //linea32
        herramienta = GameManager.instance.playerHerramienta;

        //Set the foodText to reflect the current player food total.
        vidaText.text = "Vida: " + vida + "   Herramientas: " + herramienta;
        //herramientaText.text = "Cantidad de herramientas: " + herramienta;

        //Call the Start function of the MovingObject base class.
        base.Start();
    }

    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        GameManager.instance.playerVida = vida;
        GameManager.instance.playerHerramienta = herramienta;

    }

    void Update()
    {

        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.

        //Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#else

        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                float x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                float y = touchEnd.y - touchOrigin.y;

                //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                    horizontal = x > 0 ? 1 : -1;
                else
                    //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                    vertical = y > 0 ? 1 : -1;
            }
        }
#endif   //End of mobile platform dependendent compilation section started above with #elif  CIERRO EL VER SI ES UNA PLATAFROMA U OTRA

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall>(horizontal, vertical);
        }

    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Every time player moves, subtract from food points total.
        //food--;

        //Set the foodText to reflect the current player food total.
        vidaText.text = "Vida: " + vida + "   Herramientas: " + herramienta;
        encontradoText.text = "";
        //herramientaText.text = "Cantidad de herramientas: " + herramienta;


        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;


        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }

        else if (other.tag == "End")
        {
            if (herramienta == 9)
            {
                // nos dice que hemos llegado al finald el juego
                GameManager.instance.TheEnd();
            }
            else
            {
                GameManager.instance.Fracaso();
            }
            
        }

        //Check if the tag of the trigger collided with is Food.
        else if (other.tag == "Food")
        {
            //Add pointsPerFood to the players current food total.
            vida += pointsPerFood;

            //Update foodText to represent current total and notify player that they gained points
            //vidaText.text = "+ " + pointsPerFood + " Vida: " + vida;
            vidaText.text = "+ " + pointsPerFood + " de vida ";

            //Disable the food object the player collided with.
            other.gameObject.SetActive(false);
        }

        //Check if the tag of the trigger collided with is food2.
        else if (other.tag == "Food2")
        {
            //Add pointsPerFood to the players current food total.
            vida += pointsPerFood2;

            //Update foodText to represent current total and notify player that they gained points
            //vidaText.text = "+ " + pointsPerFood2 + " Vida: " + vida;
            vidaText.text = "+ " + pointsPerFood2 + " de vida ";

            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Agua")
        {
            //Add pointsPerFood to the players current food total.
            vida += pointsPerAgua;

            //Update foodText to represent current total and notify player that they gained points
            //vidaText.text = "+ " + pointsPerAgua + " Vida: " + vida;
            vidaText.text = "+ " + pointsPerAgua + " de vida ";

            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
        }

        else if (other.tag == "Herramienta")
        {
           encontradoText.text = "Nueva herramienta encontrada";
           animator.SetTrigger("playerHand");
            //Add pointsPerFood to the players current food total.
           herramienta += numHerramienta;

            //Update foodText to represent current total and notify player that they gained points
           vidaText.text = "+ " + numHerramienta + " herramienta " ;


            //Disable the soda object the player collided with.
           other.gameObject.SetActive(false);
       }
    }

    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //Set the attack trigger of the player's animation controller in order to play the player's attack animation.
        animator.SetTrigger("playerBack");
    }
    

    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game. And we load it in "Single" mode so it replace the existing one
        //and not load all the scene object in the current scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void LoseVida(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        //Subtract lost food points from the players total.
        vida -= loss;

        //Update the food display with the new total.
        vidaText.text = "- " + loss + " Vida: " + vida;

        //Check to see if game has ended.
        CheckIfGameOver();
    }


    //CheckIfGameOver checks if the player is out of food points and if so, ends the game.
    private void CheckIfGameOver()
    {
        //Check if food point total is less than or equal to zero.
        if (vida <= 0)
        {
            animator.SetTrigger("playerDead");

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();
        }
    }

}
