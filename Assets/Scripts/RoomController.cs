using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    public float degreesPerSecond = 0.06666666667f;

    const float maxDPS = 20f;
    const float minDPS = -20f;
    
    public VRBasics_Slider speedSlider;
    public VRBasics_Grabbable speedSliderGrabbable;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float speedOffset = 0.0f;
        if(speedSliderGrabbable.GetIsGrabbed()) {
            speedOffset = Mathf.Lerp(minDPS, maxDPS, speedSlider.percentage);
        }
        speedSlider.SetSpring();
        transform.Rotate(Vector3.right * (speedOffset + degreesPerSecond) * Time.deltaTime);
	}
}
