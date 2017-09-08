using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool moving = false;
    private float speed;
    private float attackCooldown = 0.5f;
    private float cooldown;
    private int dodges;
    private float curSpeed;
    private float maxSpeed;

    CircleCollider2D attackTrigger;
    Animator anim;
    Player player;

    void Start()
    {
        
        cooldown = 0.0f;
        anim = this.GetComponent<Animator>();
        attackTrigger = this.GetComponent<CircleCollider2D>();
        attackTrigger.enabled = false;
        player = this.GetComponent<Player>();
        speed = GameManager.instance.playerSpeed;
    }

    void FixedUpdate()
    {
        if(player.hp > 0)
        {

            if (cooldown > attackCooldown)
                cooldown = attackCooldown;
            else
                cooldown += Time.deltaTime;
            movement();    
        }

    }

    void movement()
    {
         
        attackTrigger.enabled = false;
        if (Input.GetButton("Up"))//CHANGE TO BOUND MOVEMENT KEY
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
            moving = true;
        }
        if (Input.GetButton("Left"))//CHANGE TO BOUND MOVEMENT KEY
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
            moving = true;
        }
        if (Input.GetButton("Down"))//CHANGE TO BOUND MOVEMENT KEY
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
            moving = true;
        }
        if (Input.GetButton("Right")) //CHANGE TO BOUND MOVEMENT KEY
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
            moving = true;
        }

        if (Input.GetButton("Fire1") && cooldown >= attackCooldown)
        {
            attackTrigger.enabled = true;
            cooldown = 0.0f;
            anim.SetTrigger("attacking");
        }

        if(Input.GetButtonDown("Dodge") && player.dodges > 0)
        {
            if (Input.GetButton("Up") && !Input.GetButton("Down") && !Input.GetButton("Left") && !Input.GetButton("Right")) //UP
                player.dodge(1);
            else if (!Input.GetButton("Up") && Input.GetButton("Down") && !Input.GetButton("Left") && !Input.GetButton("Right")) //DOWN
                player.dodge(2);
            else if (!Input.GetButton("Up") && !Input.GetButton("Down") && Input.GetButton("Left") && !Input.GetButton("Right")) //LEFT
                player.dodge(3);
            else if (!Input.GetButton("Up") && !Input.GetButton("Down") && !Input.GetButton("Left") && Input.GetButton("Right")) //RIGHT
                player.dodge(4);

            //ADD DIAGONAL DODGES

            //dodges--; //ADD LIMITED DODGES
        }

        if (Input.GetKey(KeyCode.D) != true && Input.GetKey(KeyCode.A) != true && Input.GetKey(KeyCode.S) != true && Input.GetKey(KeyCode.W) != true)
            moving = false;

        anim.SetBool("walking", moving);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (attackTrigger.enabled)
        {
            Debug.Log("this is attack trigger");
            other.GetComponent<Enemy>().takeDamage(GameManager.instance.playerDamage);
            attackTrigger.enabled = false;
        }

    }
}
