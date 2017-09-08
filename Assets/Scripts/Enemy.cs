using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    GameObject player;
    public bool patrol = true, guard = false, clockwise = false, dead = false;
    public bool moving = true;
    public bool pursuingPlayer = false, goingToLastLoc = false;
    public float huntingSpeed = 4.5f;

    Vector3 target;
    Rigidbody2D rd2d;
    CapsuleCollider2D bodyCollider;
    CircleCollider2D attackCollider;
    public Vector3 playerLastPos;
    RaycastHit2D hit;
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

    void Start()
    {
        cooldown = attackCooldown;
        initialSpeed = speed;
        bodyCollider = this.GetComponent<CapsuleCollider2D>();
        attackCollider = this.GetComponent<CircleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = this.GetComponent<Animator>();
        playerLastPos = this.transform.position;

        rd2d = this.GetComponent<Rigidbody2D>();
        layerMask = ~layerMask;
    }

    void Update()
    {
        if(!dead)
        {
            if (cooldown > attackCooldown)
                cooldown = attackCooldown;
            else
                cooldown += Time.deltaTime;
            movement();
            playerDetect();
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
        hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(dir.x, dir.y), dist, layerMask);
        Debug.DrawRay(transform.position, dir, Color.red);

        Vector3 fwt = this.transform.TransformDirection(Vector3.right);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(fwt.x, fwt.y), 1.0f, layerMask);
        Debug.DrawRay(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(fwt.x, fwt.y), Color.cyan);

        if(moving == true)
            transform.Translate(Vector3.right * speed * Time.deltaTime);

        if(patrol==true)
        {
            //Debug.Log("ENEMY: " + "patrolling");
            speed = initialSpeed;
            
            if(hit2.collider != null)
            {
                if(hit2.collider.gameObject.tag == "Wall")
                {
                    if (clockwise == false)
                        transform.Rotate(0, 0, 90);
                    else
                        transform.Rotate(0, 0, -90);
                }
            }
        }

        if(pursuingPlayer == true)
        {
            //Debug.Log("hunting");
            speed = huntingSpeed;
            rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);

            if (hit.collider.gameObject.tag == "Player")
                playerLastPos = player.transform.position;
        }

        if(goingToLastLoc==true)
        {
            //Debug.Log("ENEMY: " + "Going to check player last location");
            speed = huntingSpeed;
            rd2d.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2((playerLastPos.y - transform.position.y), (playerLastPos.x - transform.position.x)) * Mathf.Rad2Deg);
            if(Vector3.Distance(this.transform.position,playerLastPos)<0.5f)
            {
                patrol = true;
                goingToLastLoc = false;
            }
        }
    }
    public void playerDetect()
    {
        playerLastPos = player.transform.position;

        Vector3 pos = this.transform.InverseTransformPoint(player.transform.position);
        if(hit.collider!=null)
        {
            if(hit.collider.gameObject.tag == "Player" && pos.x > 1.2f && Vector3.Distance(this.transform.position,player.transform.position)<9) //detected
            {
                //Debug.Log("ENEMY: " + "Detected player");
                patrol = false;
                pursuingPlayer = true;
                goingToLastLoc = true;
            }
            
            else if (pursuingPlayer == true)
            {
                
                //pursuingPlayer = false;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        //Debug.Log("ENEMY: " + other + " came into my collider, twas a" + other.GetType());
        if(other.tag == "Player"  && other is CapsuleCollider2D && cooldown >= attackCooldown && attackCollider.enabled) // Check if player. Check if it's players hitbox (not attack box) 
        {
            pursuingPlayer = true;  
            //Debug.Log("ENEMY: " + this + " is attackin the Player");
            cooldown = 0.0f;
            anim.SetTrigger("attacking");
            player.GetComponent<Player>().takeDamage(enemyDamage);
            pursuingPlayer = false;
        }
        if (other.gameObject.layer == 8)
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
}