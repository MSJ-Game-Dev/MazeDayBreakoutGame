using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Enemy : MonoBehaviour
{
    public float health = 100;
    public float damage = 5;
    public Animator deathAnim;
    public ParticleSystem fireBurst;
    public Transform laserLight, muzzle, player;
    public HUDDATA playerHud;

    private void Start()
    {

        //calls the shoot method at 2 second intervals, almost like a coroutine
        //the random.value is so all bots dont start firing at the same time
        InvokeRepeating("Shoot", Random.value, 1);
    }

    void Update()
    {
        //look at the player
        transform.LookAt(player);
        //turn light towards player
        laserLight.LookAt(player.position + Vector3.up * 2); //translated up so laser is in cameras
    }


/*    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(player.position, 2f);

    }*/
    private void Shoot()
    {
        if (health <= 0)
        {
            Die();
            CancelInvoke("Shoot");
            return;
        }
        //finds a random point in the vicinity of player so robots arent lazer focused on player with each shot
        //aka adds randomness to shots
        var randPoint = Random.insideUnitSphere * 3f; // *3 increases the radius of the sphere of randomness around player
        var targetDir = randPoint + player.position - muzzle.position; //gets direction to be used for Phys.Raycast()
            
        fireBurst.Play();

        //checks to see if player was hit on raycast, causes damage if so
        if (Physics.Raycast(muzzle.position, targetDir, out RaycastHit hit, 30, LayerMask.GetMask("Player", "EnvironmentalObjs")) && hit.transform.gameObject != null);
        {
            //Weird null transform glitch
            int layer = hit.transform.gameObject.layer;
            if (layer == LayerMask.NameToLayer("EnvironmentalObjs"))
            {
                //TODO Add bullet holes FX Here
            }
            else if (layer == LayerMask.NameToLayer("Player"))
            {
                playerHud.TakeDamage(10);
            }
        }
    }

    private void Die()
    {
        deathAnim.Play("RobotDeath");
        Destroy(transform.gameObject);
    }

    //Similar to HUDDATA.takeDamage(), but for the enemy bots
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
    }
}

