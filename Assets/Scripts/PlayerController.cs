using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	[HideInInspector]
	public MovementControl movementCtrl;
	[HideInInspector]
	public ShootingControl shootingCtrl;
	[HideInInspector]
	public HealthControl healthCtrl;

	public Transform retina;
	public LayerMask floorLayerMask;

    public static PlayerController instance;

	// Use this for initialization
	void Awake ()
	{
	    instance = this;
		healthCtrl = GetComponent<HealthControl> ();
		movementCtrl = GetComponent<MovementControl> ();
		shootingCtrl = GetComponent<ShootingControl> ();
	}
	
	// Update is called once per frame
	void Update () {
	    if (healthCtrl.Dead)
	    {
	        return; 
	    }

        float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
        
		movementCtrl.Move (h, v);
		movementCtrl.ApplyGravity ();

		Vector2 aimDirection = AdjustRetina();
		movementCtrl.RotateTowards (aimDirection.x, aimDirection.y);
        
		if (Input.GetMouseButton(0)) {
			shootingCtrl.Shoot ();
		}
	}
    
	Vector2 AdjustRetina() {
		Ray rayFromCamera = Camera.main.ScreenPointToRay (Input.mousePosition);
        
		RaycastHit floorHit;
        
		if(Physics.Raycast (rayFromCamera, out floorHit, 100, floorLayerMask)) {
			retina.position = new Vector3 ( floorHit.point.x, floorHit.point.y + 0.05f , floorHit.point.z);
		}
        
		Vector3 directionRetina = retina.position - transform.position;
        
		return new Vector2(directionRetina.x,directionRetina.z);
	}

}
