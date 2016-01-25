using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public enum State { Idle, ChasingPlayer, Attacking, Dead, Hurt }
    
	public State currentState = State.Idle;

	[HideInInspector]
	public MovementControl movementCtrl;
	[HideInInspector]
	public ShootingControl shootCtrl;
	[HideInInspector]
	public HealthControl healthCtrl;
    
	public float distanceToAttack=2;
    
	void Awake () {
		movementCtrl = GetComponent<MovementControl> ();
		shootCtrl = GetComponent<ShootingControl> ();
		healthCtrl = GetComponent<HealthControl> ();
	}


	void Update () {
		movementCtrl.ApplyGravity ();

	    if (PlayerController.instance == null || PlayerController.instance.healthCtrl.Dead) return;
        
		if (healthCtrl.Dead)
				currentState = State.Dead;

	    switch (currentState)
	    {
	        case State.Idle:
	            if (!PlayerController.instance.healthCtrl.Dead)
	                currentState = State.ChasingPlayer;
	            break;

	        case State.ChasingPlayer:
	            if (PlayerController.instance.healthCtrl.Dead)
	                currentState = State.Idle;

	            MoveToPlayer();
	            RotateToPlayer();

	            if (GetDistanceToPlayer() < distanceToAttack && shootCtrl.canShoot)
	                currentState = State.Attacking;
	            break;

	        case State.Attacking:
	            RotateToPlayer();

	            if (shootCtrl.canShoot &&  GetDistanceToPlayer() < distanceToAttack)
                { 
	                shootCtrl.Shoot();
	            }
	            else
		        {
		            currentState = State.ChasingPlayer;
		        }

		        break;
		}
	}

    private float GetDistanceToPlayer()
    {
        if (PlayerController.instance == null) return Mathf.Infinity;

        return Vector3.Distance(PlayerController.instance.transform.position, transform.position);
    }

    void MoveToPlayer () {
		if (!movementCtrl.charControl.isGrounded || !PlayerController.instance) return;
        
		Vector3 directionToPlayer = Vector3.Normalize (PlayerController.instance.transform.position - transform.position);
        
		movementCtrl.Move (directionToPlayer.x,directionToPlayer.z);
	}

	void RotateToPlayer () {
		if (!PlayerController.instance) return;

        Vector3 direcaoJogador = GetDirectionToPlayer();
        
		movementCtrl.RotateTowards (direcaoJogador.x,direcaoJogador.z);
	}

    private Vector3 GetDirectionToPlayer()
    {
        if (PlayerController.instance == null) return Vector3.forward;

        Vector3 direcaoJogador = Vector3.Normalize(PlayerController.instance.transform.position - transform.position);
        return direcaoJogador;
    }
}
