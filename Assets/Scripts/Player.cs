using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MonoBehaviour
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    private Vector3 mousePos;
    private Animator anim;                  //Used to store a reference to the Player's animator component.
    public int hp;                           //Used to store player HP points total during level.
    public int dodges;

    public bool isDodging;
    LayerMask ObstacleLayer;
    public Vector3 velocity;
    public float dodgeDistance;
    public float dodgeSpeed;
    public Vector3 dodgeTarget;
    Vector2 direction;
    Rigidbody2D RBody2D;

    //Start overrides the Start function of MovingObject
    void Start()
    {
        //Get a component reference to the Player's animator component
        anim = GetComponent<Animator>();
        RBody2D = GetComponent<Rigidbody2D>();
        //Get the current food point total stored in GameManager.instance between levels.
        hp = GameManager.instance.playerHP;
        dodges = GameManager.instance.playerDodgesMax;
        Debug.Log("HP on load: " + hp);
        Debug.Log("Dodges on load: " + dodges);
        ObstacleLayer = LayerMask.GetMask("Default");
        dodgeDistance = GameManager.instance.dodgeDistance;
        isDodging = false;
    }


    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        GameManager.instance.playerHP = hp;
    }


    private void Update()
    {

    }
    private void FixedUpdate()
    {
        if(isDodging)
        {

            float distSqr = (dodgeTarget - transform.position).sqrMagnitude;
            if(distSqr <0.1f)
            {
                Debug.Log("Finished dodging");
                onDodgeFinished();
                isDodging = false;
                dodgeTarget = Vector2.zero;
                //MAKE PLAYER INVINCIBLE HERE
            }
            else
            {
                RBody2D.MovePosition(Vector3.Lerp(transform.position, dodgeTarget, dodgeSpeed * Time.deltaTime));
            }
        }
        else
        {
            RBody2D.AddForce(velocity);
        }
    }

    public virtual void ClearVelocity()
    {
        velocity = Vector2.zero;
        RBody2D.velocity = Vector2.zero;
    }
    public void dodge(int dodgeDir)
    {
        //1=UP, 2=DOWN, 3=LEFT, 4=RIGHT
        if (dodgeDir == 1)
            direction = Vector2.up;
        else if (dodgeDir == 2)
            direction = Vector2.down;
        else if (dodgeDir == 3)
            direction = Vector2.left;
        else if (dodgeDir == 4)
            direction = Vector2.right;

        isDodging = true;
        Debug.Log(dodgeDistance);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dodgeDistance, ObstacleLayer);

        if (hit)
        {
            Debug.Log("hit");
            dodgeTarget = transform.position + (Vector3)((direction * dodgeDistance)
                * hit.fraction);
        }
        else
        {
            Debug.Log("No hit");
            dodgeTarget = transform.position + (Vector3)(direction * dodgeDistance);
        }

        dodgeSpeed = GameManager.instance.dodgeSpeed;
        ClearVelocity();
        
    }

    private void onDodgeFinished()
    {
        //do something?
    }

    public void attack()
    {

    }
 

    //To restart scene
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        //SceneManager.LoadScene(0);
    }


    public void takeDamage(int dam)
    {
        hp -= dam; //player takes damage 
        checkIfDead(); //Is player dead?
    }

    private void checkIfDead()
    {
        if (hp <= 0)
        {
            this.GetComponent<CapsuleCollider2D>().enabled = false; //disable collider
            this.GetComponent<CircleCollider2D>().enabled = false; //disable attack collider
            float d = Random.Range(0.0f, 2.0f);
            int n = Mathf.RoundToInt(d);
            if (n == 0)
                anim.SetTrigger("death1");
            if (n == 1)
                anim.SetTrigger("death2");
            else
                anim.SetTrigger("death3");
            
            Debug.Log("U ded");
        }

    }
}