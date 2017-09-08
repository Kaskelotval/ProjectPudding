using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
    GameObject player;
    bool followPlayer = true;
    public float distance;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (followPlayer == true)
            CamFollowPlayer();
	}

    public void setFollowPlayer(bool val)
    {
        followPlayer = val;
    }

    void CamFollowPlayer()
    {
        Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z-distance);
        this.transform.position = newPos;
    }
}
