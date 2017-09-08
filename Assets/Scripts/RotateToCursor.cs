using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCursor : MonoBehaviour {
    Vector3 mousePos;
    Vector3 direction;
    Camera cam;
    Rigidbody2D rid;
    Player player;

    // Use this for initialization
    void Start () {
        rid = this.GetComponent<Rigidbody2D>();
        cam = Camera.main;
        player = this.GetComponent<Player>();

    }

    // Update is called once per frame
    void Update () {
        if(player.hp > 0)
            rotateToCamera();
	}

    void rotateToCamera()
    {
        mousePos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - cam.transform.position.z));
        rid.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(mousePos.y - transform.position.y, (mousePos.x - transform.position.x)) * Mathf.Rad2Deg);
    }
}
