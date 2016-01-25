using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MovementControl : MonoBehaviour {
    
	public CharacterController charControl;
	public float movementSpeed=3;
	public float rotationSpeed=10;
	public float gravity=5;
    
    [HideInInspector]
	public bool takingKnockback=false;

    public Transform body;

	// Use this for initialization
	void Awake () {
		charControl = GetComponent<CharacterController> ();
	}

	void OnEnable() {
		takingKnockback = false;
	    //Vector3 desiredScale = Vector3.one;
	    //desiredScale.y = 0.7f;
	    //body.transform.localScale = desiredScale;
	    //body.DOKill();
	    //body.DOScaleY(1.45f, 0.17f).SetLoops(-1,LoopType.Yoyo);
	}

	//Função de movimento, chamado por outros scripts, no caso atual, o ControleJogador.
	public void Move (float h, float v) {
		if ((h == 0 && v == 0)||takingKnockback) return;
        
		charControl.Move (new Vector3(h,0,v) * movementSpeed * Time.deltaTime);
	}
    
	public void RotateTowards(float h, float v) {
		if (h == 0 && v == 0)
			return;
        
		Quaternion newRotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (new Vector3 (h, 0, v)), rotationSpeed * Time.deltaTime);
        
		transform.rotation = Quaternion.Euler (0, newRotation.eulerAngles.y, 0);
	}
    
	public void ApplyGravity () {
		charControl.Move (new Vector3 (0, -gravity, 0) * Time.deltaTime);
	}

	public void TakeKnockback(Vector3 direcao, float duracao)
	{
	    if (takingKnockback) return;
		StartCoroutine (DoKnockback (direcao, duracao));
	}

	IEnumerator DoKnockback (Vector3 direction, float duration) {
		takingKnockback = true;

		while (duration>0) {
			charControl.Move (direction * Time.deltaTime);

			duration = duration - Time.deltaTime * 10;

			yield return 0;
		}

		takingKnockback = false;
	}

}
