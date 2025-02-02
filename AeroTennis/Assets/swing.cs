using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;


public class swing : NetworkBehaviour
{
    private bool isInRange = false;

    public float shotDir = 0f;
    public float upwardForce = .5f;
    public float launchForce = 17f;
    public bool cooldownOver = true;
    public GameObject PlayerModel;
    Vector3 lastMousePos;
    float swipeThreshold = 150f;
    private GameObject thisPlayer;
    public Vector3 BallHitLoc;
    public string sound;
    
    public AudioManager soundPlayer;
    

    private void Start()
    {
        cooldownOver = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        

        Vector3 currentMousePos = Input.mousePosition;
        float swipeDistance = (currentMousePos - lastMousePos).magnitude;
        //print(swipeDistance);
        if ((swipeDistance > swipeThreshold) && cooldownOver )
        {
            if (Input.GetAxis("Mouse X") < 0)
            {
                PlayerModel.GetComponent<PlayerAnimScript>().PlayAnimation("SwingMovingRight");
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    shotDir = -0f;
                    sound = "stocky hit";
                }
                else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                {
                    shotDir = -0.4f;
                    sound = "hard hit";
                }
                else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
                {
                    shotDir = -.15f; 
                    sound = "wide hit";
                }
                else
                {
                    shotDir = -0.25f;
                    sound = "normal hit";
                }
                if (isInRange)
                {
                    soundPlayer.PlaySound(sound);
                    LaunchBall(shotDir);
                }
                else
                {
                    soundPlayer.PlaySound("swing");
                }
            }
            else if (Input.GetAxis("Mouse X") > 0)
            {
                PlayerModel.GetComponent<PlayerAnimScript>().PlayAnimation("SwingMovingleft");
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    shotDir = 0f;
                    sound = "stocky hit";
                }
                else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                {
                    shotDir = 0.4f;
                    sound = "hard hit";
                }
                else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
                {
                    shotDir = 0.15f;
                    sound = "wide hit";
                }
                else
                {
                    shotDir = 0.25f;
                    sound = "normal hit";
                }
                if (isInRange)
                {
                    soundPlayer.PlaySound(sound);
                    LaunchBall(shotDir);
                }else
                {
                    soundPlayer.PlaySound("swing");
                }
            }
            StartCoroutine(StartCooldown());
        }
        
        lastMousePos = currentMousePos;
        
        
        
        
        
        
        
        
        
    }

    IEnumerator StartCooldown()
    {
        cooldownOver = false;
        yield return new WaitForSeconds(.5f);
        cooldownOver = true;
    }


    


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BallZone"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BallZone"))
        {
            isInRange = false;
        }
    }
    

    [Command]
    private void LaunchBall(float curve)
    {
        GameObject ball = GameObject.FindGameObjectWithTag("BallZone");
        BallHitLoc = ball.transform.position;
        if (ball != null)
        {
            Rigidbody ballZoneRigidbody = ball.GetComponent<Rigidbody>();
            if (ballZoneRigidbody != null)
            {
                ballZoneRigidbody.velocity = Vector3.zero;
                ball.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + .5f);
                Vector3 launchDirection = (ball.transform.position - transform.position).normalized;
                if (transform.position.z > 0)
                {
                    launchDirection.z *= -1;
                    curve *= -1;
                }
                
                launchDirection.x = curve;
                launchDirection.y = upwardForce;
                ballZoneRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            }
        }
    }
}
