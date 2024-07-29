using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    bool running = false;
    bool game_started = false;
    bool game_over = false;

    public GameObject player;
    public Animator player_animator;

    public GameObject toy;
    public Animator toy_animator;

    public GameObject laser;

    public GameObject camera;

    public ParticleSystem blood_spray;
    public GameObject blood;

    AudioSource source;

    public AudioClip step;
    public AudioClip shooting;
    public AudioClip hit;
    public AudioClip fall;

    float steps_counter;

    public GameObject ui_start;
    public GameObject ui_gameover;
    public GameObject ui_win;
    public GameObject ui_guide;
    public Text ui_guideText;
  
    public Button level1;
    public Button level2;

    float spead = 1;
    int level=0;
    
    //keys to stop player 
    KeyCode key1=0 , key2=0 , key3 = 0;

    void Start()
    {
        source =GetComponent<AudioSource>();  
        ui_start.SetActive(true);
        level1.onClick.AddListener(() => {
            level = 1;
            Debug.Log(level);
        });
        level2.onClick.AddListener(() => {
            level = 2;
            Debug.Log(level);
        });
    }

    void Update()
    {
        if (running)
        {
           player.transform.position -= new Vector3(0, 0, 0.5f* Time.deltaTime);
           camera.transform.position -= new Vector3(0, 0, 0.5f* Time.deltaTime);
            steps_counter += Time.deltaTime;
            if (steps_counter > 0.25f)
            {//trac steps to display sound of each step
                steps_counter = 0;
               source.PlayOneShot(step);
            }
        }  
        if (level!=0 && !game_started)
        {
            ui_start.SetActive(false);
            ui_guide.SetActive(true) ;
            running = true;
            game_started = true;
            player_animator.SetTrigger("run");
            StartCoroutine(Sing());
        }
        if (Input.GetKeyDown(KeyCode.Space) && game_over)
        {
            SceneManager.LoadScene("Game");
        }
        //to make player stop lesten key input 
        //should press on 3 key at a same time 
        if (Input.GetKey(key1) && Input.GetKey(key2) && Input.GetKey(key3) && !game_over) {
            running = false;
            //to stop all animation of player
            player_animator.speed = 0;
        }
        // Check if the player doesn't hold all 3 keys 
        else if((Input.GetKeyUp(key1)|| Input.GetKeyUp(key2) || Input.GetKeyUp(key3) ) && !game_over){
            running =true;
            player_animator.speed = 1; 
        }

    }

    IEnumerator Sing()
    {
        toy.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2f);
        //when sing stoped player should stop
                                              //type                  (string value )     Random char from [A-Z]    
        key1 = (KeyCode)System.Enum.Parse( typeof(KeyCode), (System.Char.ConvertFromUtf32('A'+Random.Range(0,25)).ToString()) );
        key2 = (KeyCode)System.Enum.Parse( typeof(KeyCode), (System.Char.ConvertFromUtf32('A'+Random.Range(0,25)).ToString()) );
        key3 = (KeyCode)System.Enum.Parse( typeof(KeyCode), (System.Char.ConvertFromUtf32('A'+Random.Range(0,25)).ToString()) );

        //Guide Hint to stop player
        ui_guideText.text = "PRESS   " + key1 + " + " + key2 + " + " + key3+" TO STOP";
        
        yield return new WaitForSeconds(2.5f/spead);

        toy_animator.SetTrigger("look");
        yield return new WaitForSeconds(2/spead);
        //check player still Running
        if (running)
        {
            Debug.Log("Shoot the Player"); 
            GameObject new_laser =Instantiate(laser);
            //put the laser at postion of toy head 
            new_laser.transform.position=toy.transform.GetChild(0).transform.position;
            game_over = true;
            source.PlayOneShot(shooting);
        }
        ui_guideText.text = "";
        yield return new WaitForSeconds(2 / spead);
        toy_animator.SetTrigger("idle");
        yield return new WaitForSeconds(1 / spead);   
        toy.GetComponent<AudioSource>().Stop();
        if(level == 2)
            spead = spead * 1.15f;
        
        toy.GetComponent<AudioSource>().pitch = spead;

        if(!game_over) StartCoroutine(Sing());
    } 
    public void HitPlayer()
    {
        running =false;
        player_animator.SetTrigger("idle");
        player.GetComponent<Rigidbody>().velocity=new Vector3(0,2,2);
        player.GetComponent<Rigidbody>().angularVelocity=new Vector3(3,0,0);
        camera.GetComponent<Animator>().Play("camera_lose");
        blood_spray.Play();
        StartCoroutine(ShowBlood());
        source.PlayOneShot(hit);
    }

    IEnumerator ShowBlood()
    {
        yield return new WaitForSeconds(0.3f);
        ui_gameover.SetActive(true);
        source.PlayOneShot(fall);
        blood.SetActive(true);
        blood.transform.position=new Vector3(player.transform.position.x,0.001f ,player.transform.position.z+0.15f);        
    }

    public IEnumerator PlayerWin()
    {
        game_over = true; 
        yield return new WaitForSeconds(1f);

        running = false;
        player_animator.SetTrigger("idle");
        
         ui_win.SetActive(true);
    }
}
