using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	public LayerMask enemyLayerMask;
	public LayerMask wallLayerMask;
    
	public float projectileSpeed = 10;
	public float autoDestructTime=0.5f;

    Rigidbody mRigidbody;

	public int Damage = 1;

	public float knockbackIntensity=2f;
	public float knockbackDuration=0.1f;

    void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

	void OnEnable() {
		if(autoDestructTime>0)
			StartCoroutine (SelfDestruct ());
	}
    
	void FixedUpdate () {
		mRigidbody.MovePosition (transform.position + transform.forward * projectileSpeed * Time.deltaTime);
	}
    
	void OnTriggerEnter (Collider other) {
        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            HealthControl otherActorHealth = other.GetComponent<HealthControl>();
			if(otherActorHealth != null) {
				otherActorHealth.TakeDamage(Damage);
			    if (!otherActorHealth.Dead && otherActorHealth.mov && knockbackDuration > 0)
			    {
			        otherActorHealth.mov.TakeKnockback(transform.forward*knockbackIntensity, knockbackDuration);
			    }
			}
            
			DestroyProjectile ();
		}

        if (((1 << other.gameObject.layer) & wallLayerMask) != 0)
        {
            DestroyProjectile ();
		}
	}

	//Destruir o projetil
	void DestroyProjectile() {
		transform.Recycle ();
	}
    
	void OnDisable() {
		StopAllCoroutines();
	}
    
	IEnumerator SelfDestruct() {
		yield return new WaitForSeconds(autoDestructTime);
        
		DestroyProjectile ();
	}
}
