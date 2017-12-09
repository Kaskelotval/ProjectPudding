using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    GameObject player;
    public bool roaming = true, guard = false, clockwise = false, dead = false, patrol = false;
    public bool moving = true;
    public bool pursuingPlayer = false, goingToLastLoc = false;
    private float soundDetectionDistance = 20.0f;

    Vector3 target;
    Rigidbody2D rd2d;
    CapsuleCollider2D bodyCollider;
    CircleCollider2D attackCollider;
    public Vector3 playerLastPos;
    RaycastHit2D hit;
    public float huntingSpeed = 1.1f;
    float speed = 1.0f;
    int layerMask = 1 << 8;
    float initialSpeed;
    private Animator anim;                

    //stats
    int hp = 100;
    int enemyDamage = 100;
    public float attackRange = 0.1f;
    private float attackCooldown = 2.0f;
    private float cooldown;

    //pathfinding
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        cooldown = attackCooldown;
        initialSpeed = speed;
        bodyCollider = this.GetComponent<CapsuleCollider2D>();
        attackCollider = this.GetComponent<CircleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = this.GetComponent<Animator>();
        playerLastPos = player.transform.position;
        rd2d = this.GetComponent<Rigidbody2D>();
        layerMask = ~layerMask;
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            Debug.Log("Reached WP");
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;
        while(true)
        {
            if(Vector3.Distance(transform.position,currentWaypoint)<0.1)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    Debug.Log("At final WP");
                    goingToLastLoc = false;
                    pursuingPlayer = false;
                    roaming = true;
                    playerDetect();
                    yield break;
                }
                currentWaypoint = path[targetIndex];
                rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((currentWaypoint.y - transform.position.y), (currentWaypoint.x - transform.position.x)) * Mathf.Rad2Deg);

            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, huntingSpeed*Time.deltaTime);
            yield return null;
        }
    }

    void Update()
    {
        if(!dead)
        {
            if (cooldown > attackCooldown)
                cooldown = attackCooldown;
            else
                cooldown += Time.deltaTime;
            playerDetect();

            movement();
        }
        else
        {
            bodyCollider.enabled = false;
            attackCollider.enabled = false;
        }

    }

    void movement()
    {
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        Vector3 dir = player.transform.position - transform.position;

        //Draw ray to player
        hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(dir.x, dir.y), dist, layerMask);
        Debug.DrawRay(transform.position, dir, Color.blue);

        //Draw ray to view direction
        Vector3 fwt = this.transform.TransformDirection(Vector3.right);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(fwt.x, fwt.y), 1.0f, layerMask);
        //Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(fwt.x, fwt.y), Color.cyan);

        anim.SetBool("moving", true);
        if(moving)
            transform.Translate(Vector3.right * speed * Time.deltaTime);

        if(roaming)
        {
            //Debug.Log("ENEMY: " + "roaminging");
            speed = initialSpeed;
            Debug.DrawRay(transform.position, dir, Color.blue);

            if (hit2.collider != null)
            {
                if(hit2.collider.gameObject.tag == "Wall")
                {
                    if (Random.Range(0, 2) == 1)
                        clockwise = !clockwise;

                    if (clockwise == false)
                        transform.Rotate(0, 0, 90);
                    else
                        transform.Rotate(0, 0, -90);
                }
            }
        }
        else if(pursuingPlayer)
        {
            //Debug.Log("hunting");
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            speed = huntingSpeed;
            rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);
            PathRequestManager.RequestPath(transform.position, playerLastPos, OnPathFound);

            /*if (hit2.collider != null)
            {
                if (hit2.collider.gameObject.tag == "Wall")
                {
                    if (Random.Range(0, 1) > 0.5)
                        clockwise = !clockwise;

                    if (clockwise == false)
                        transform.Rotate(0, 0, 90);
                    else
                        transform.Rotate(0, 0, -90);
                }
            }/*
            /*if (hit.collider.gameObject.tag == "Player")
            {
            }*/
        }
        else if(goingToLastLoc)
        {
            //Debug.Log("ENEMY: " + "Going to check player last location");
            //speed = huntingSpeed;
            //rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);
            rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);

            PathRequestManager.RequestPath(transform.position, playerLastPos, OnPathFound);

            if (Vector3.Distance(this.transform.position,playerLastPos)<0.3f)
            {
                roaming = true;
                goingToLastLoc = false;
            }
        }
    }
    public void playerDetect()
    {
        Vector3 pos = this.transform.InverseTransformPoint(player.transform.position);
        if(hit.collider!=null)
        {
            if (pursuingPlayer == true)
            {
                if (hit.collider.gameObject.tag == "Wall") //If player goes behind a wall
                {
                    Debug.Log("Lost sight of player");
                    pursuingPlayer = false;
                    goingToLastLoc = true;
                }
                else
                {
                    //Enemy sees player and updates last position
                    playerLastPos = player.transform.position;
                }
            }
            else if (hit.collider.gameObject.tag == "Player" && pos.x > 0.1f && Vector3.Distance(this.transform.position,player.transform.position)<9) //detected
            {
                //Debug.Log("ENEMY: " + "Detected player");
                playerLastPos = player.transform.position;
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
                roaming = false;
                pursuingPlayer = true;
                goingToLastLoc = true;
            }
            

        }
    }
    public void PlayerDetectSound()
    {
        if(Vector2.Distance(transform.position,player.transform.position)<=soundDetectionDistance)
        {
            playerLastPos = player.transform.position;
            Debug.Log("I, "+ this.gameObject +" heard the player");
            roaming = false;
            goingToLastLoc = true;
        }
        else
            Debug.Log("I, " + this.gameObject + " did not hear the player");

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        //Debug.Log("ENEMY: " + other + " came into my collider, twas a" + other.GetType());
        if(other.tag == "Player"  && other is CapsuleCollider2D && cooldown >= attackCooldown && attackCollider.enabled) // Check if player. Check if it's players hitbox (not attack box) 
        {
            rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);

            pursuingPlayer = true;  
            //Debug.Log("ENEMY: " + this + " is attacking the Player");
            cooldown = 0.0f;
            //anim.SetTrigger("attacking");
            anim.Play("char2hit");
            player.GetComponent<Player>().takeDamage(enemyDamage);
            pursuingPlayer = false;
        }
        if (other.gameObject.layer == 8) //Crash-and-Turn
        {
            if (clockwise == false)
                transform.Rotate(0, 0, 90);
            else
                transform.Rotate(0, 0, -90);
        }
            

    }

    public void takeDamage(int dam)
    {
        hp -= dam;
        checkIfDead();
    }

    private void checkIfDead()
    {
        if (hp <= 0)
        {
            float d = Random.Range(0.0f, 2.0f);
            int n = Mathf.RoundToInt(d);
            if(n == 0)
                anim.SetTrigger("death1");
            if(n == 1)
                anim.SetTrigger("death2");
            else
                anim.SetTrigger("death3");
            attackCollider.enabled = false;
            bodyCollider.enabled = false;
            dead = true;  
        }

    }

    public void OnDrawGizmos()
    {
        if(path !=null)
        {
            for(int i = targetIndex; i<path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(0.1f,0.1f,0.1f));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }
}