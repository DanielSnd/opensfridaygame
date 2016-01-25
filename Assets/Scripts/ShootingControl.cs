using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ShootingControl : MonoBehaviour {
    
	public float ShotCooldown = 0.5f;
    
	public Transform projectilePrefab;
	public Transform spawnPosition;

    private float lastTimeShot;

    public bool canShoot { get { return Time.time > lastTimeShot + ShotCooldown; } }
    
	void Awake () {
	    lastTimeShot = Time.time;
	}
    
	void Start () {
		projectilePrefab.CreatePool ();
	}
    
	public void Shoot() {
		if (!canShoot) return;
	    lastTimeShot = Time.time;
        projectilePrefab.Spawn(spawnPosition.position, transform.rotation);
	    //spawnPosition.DOKill();
	    //spawnPosition.DOShakePosition(0.1f, Vector3.one*0.3f, 5);
	}
    
}
