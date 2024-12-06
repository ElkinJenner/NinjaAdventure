using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour{
    void Start(){ 

    }
    void Update(){
    }
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player"){
            PlayerController controller = collision.GetComponent<PlayerController>();
            controller.Die();
        }
    }
}