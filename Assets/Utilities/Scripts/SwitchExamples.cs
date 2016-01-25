using UnityEngine;
using System.Collections;

public class SwitchExamples : MonoBehaviour
{
    public GameObject[] Example1;
    public GameObject[] Example2;
    private bool switched = false;
    Color mColor;
    // Use this for initialization
    void Start ()
    {
        mColor = RenderSettings.ambientLight;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyUp(KeyCode.B))
	    {
	        switched = !switched;
	        for (int i = 0; i < Example1.Length; i++)
	        {
	            Example1[i].SetActive(switched);
	        }
            for (int i = 0; i < Example2.Length; i++)
            {
                Example2[i].SetActive(!switched);
            }
	        RenderSettings.ambientLight = switched ? Color.grey : mColor;
	    }
	}
}
