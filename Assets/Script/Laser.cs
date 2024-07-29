using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.Find("player");    
    }

    // Update is called once per frame
    void Update()
    {                                                            //from toy        to player                Max Distance for each frame      
        GetComponent<Rigidbody>().position =Vector3.MoveTowards(transform.position,player.transform.position,1000 * Time.deltaTime);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag== "Player")
        {
            //call HitPlayer
            FindObjectOfType<GameManger>().HitPlayer();
            //distry laser object
            Destroy(gameObject);
        }
    }
}
