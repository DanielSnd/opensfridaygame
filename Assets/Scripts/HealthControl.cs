using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class HealthControl : MonoBehaviour {

	public bool Dead=false;
	public int CurrentHealth=10;
	public int MaximumHealth = 10;
	float timeLastTookDamage;
	public float takeHitCooldown=0.2f;
    
	public bool ShouldRecycle=false;
    
    [HideInInspector]
	public MovementControl mov;
    
	void Awake() {
		mov = GetComponent<MovementControl> ();
	}
    
	void OnEnable () {
		timeLastTookDamage = Time.time;
		CurrentHealth = MaximumHealth;
		Dead = false;
	}
    
	public bool TakeDamage(int damage) {
		if (Time.time < timeLastTookDamage + takeHitCooldown || Dead) return false;

		CurrentHealth = CurrentHealth - damage;

        timeLastTookDamage = Time.time;

	    //transform.DOKill();
	    //transform.DOShakeScale(0.1f, 2);

		if (CurrentHealth <= 0)
			Die ();
        
		return true;
	}
    
	public void Die() {
		Dead = true;
        
		if(ShouldRecycle)
			transform.Recycle ();
	}
}
