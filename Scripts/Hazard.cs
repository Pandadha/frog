using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var health = other.GetComponent<PlayerHealth>();
        if (health == null) return;
      
        // Pass damage source position
        health.TakeDamage(damageAmount, transform.position);

     
    }

   
}
