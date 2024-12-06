using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    //Global variables
    public float jumpForce = 6f;
    public float runningSpeed = 4f;
    //Components
    Rigidbody2D rigidBody;
    Animator animator;
    public LayerMask groundMask;
    Vector3 startPosition;
    [SerializeField]
    private int healthPoints, manaPoints;
    public const int INITIAL_HEALTH = 100, INITIAL_MANA = 15,
    MAX_HEALTH = 200, MAX_MANA = 30,
    MIN_HEALTH = 10, MIN_MANA = 0;
    //Parameter values
    const string STATE_ALIVE = "isAlive";
    const string STATE_IDLE = "isIdle";
    const string STATE_THE_GROUND = "isOnTheGround";
    const string STATE_ATTACK = "isOnAttack";
    const string STATE_RUN = "isRunning";
    const string STATE_THROW = "isThrowing";
    void Awake(){
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Start(){
        startPosition = this.transform.position;
    }
    //Game
    public void StartGame(){
        animator.SetBool(STATE_ALIVE, true);
        animator.SetBool(STATE_IDLE, true);
        animator.SetBool(STATE_RUN, false);
        animator.SetBool(STATE_THE_GROUND, false);
        animator.SetBool(STATE_ATTACK, false);
        animator.SetBool(STATE_THROW, false);

        healthPoints = INITIAL_HEALTH;
        manaPoints = INITIAL_MANA;

        Invoke("RestartPosition", 0.2f);
    }
    void RestartPosition(){
        this.transform.position = startPosition;
        this.rigidBody.velocity = Vector2.zero;

        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.GetComponent<CameraFollow>().ResetCameraPosition();
    }

    public void CollectMana(int points){
        this.manaPoints += points;
        if (this.manaPoints >= MAX_MANA)
        {
            this.manaPoints = MAX_MANA;
        }
    }
    public int GetHealth(){
        return healthPoints;
    }

    public int GetMana(){
        return manaPoints;
    }
    public float GetTravelledDistance(){
        return this.transform.position.x - startPosition.x;
    }
    void Update(){ 
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
            Jump();
        }
        // Update parameter if touching the ground
        animator.SetBool(STATE_THE_GROUND, IsTouchingTheGround());     
        Debug.DrawRay(this.transform.position, Vector2.down * 1.1f, Color.red);

        // Detect attack with the X key or right click
        if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1)){
            Attack();
        }
        else{
            animator.SetBool(STATE_ATTACK, false);
        }
        if(Input.GetKeyDown(KeyCode.C)){
            Throw();
        }
        else{
            animator.SetBool(STATE_THROW, false);
        }

    }
    void FixedUpdate(){
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0){ 
            rigidBody.velocity = new Vector2(horizontalInput * runningSpeed, rigidBody.velocity.y);
            // Change sprite direction
            if (horizontalInput > 0){
                transform.localScale = new Vector3(1, 1, 1); 
            }
            else if (horizontalInput < 0){
                transform.localScale = new Vector3(-1, 1, 1);
            }
            animator.SetBool(STATE_RUN, true);
            animator.SetBool(STATE_IDLE, false);
        }
        else{
            animator.SetBool(STATE_RUN, false);
            animator.SetBool(STATE_IDLE, true);
        }
    }
    //Player mechanics
    void Attack(){
        animator.SetBool(STATE_ATTACK, true);
        animator.SetBool(STATE_IDLE, false);
    }
    void Throw(){
        animator.SetBool(STATE_THROW, true);
        animator.SetBool(STATE_IDLE, false);
    }
    void Jump(){
        if(IsTouchingTheGround()){
            rigidBody.AddForce(Vector2.up *jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioSource>().Play();
        }
    }
    bool IsTouchingTheGround(){
        if(Physics2D.Raycast(this.transform.position, Vector2.down, 1.5f, groundMask)){
            return true;

        }
        else{
            return false;
        }
    }

    public void Die(){
        animator.SetBool(STATE_ALIVE, false);
        GameManager.sharedInstance.GameOver();

    }
    //Another methods
    public void CollectHealth(int points){
        this.healthPoints += points;
        if (this.healthPoints >= MAX_HEALTH)
        {
            this.healthPoints = MAX_HEALTH;
        }

        if (this.healthPoints <= 0)
        {
            Die();
        }
    }
}
