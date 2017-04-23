using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpController : MonoBehaviour {

    public VRBasics_Slider button;
    public GameObject[] helpScreens;

    private bool pushed = false;    
    private bool lit = false;

    // Use this for initialization
    void Start () {
        ToggleHelp(false);
    }
	
	// Update is called once per frame
	void Update () {
        //after push, has the button returned to up
        if(pushed) {
            if(button.percentage < 0.5f) {
                pushed = false;
            }
        }

        if(button.percentage > 0.5f && !pushed && !lit) {
            ToggleHelp(true);
        }

        if(button.percentage > 0.5f && !pushed && lit) {
            ToggleHelp(false);
        }
    }

    private void ToggleHelp(bool on) {
        pushed = true;
        foreach(GameObject helpScreen in helpScreens) {
            helpScreen.SetActive(on);
        }
        lit = on;
    }
}
