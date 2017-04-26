using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour {

    public float degreesPerSecond = 0.06666666667f;

    const float maxDPS = 20f;
    const float minDPS = -20f;
    
    public VRBasics_Slider xSpeedSlider;
    public VRBasics_Grabbable xSpeedSliderGrabbable;

    public VRBasics_Slider zSpeedSlider;
    public VRBasics_Grabbable zSpeedSliderGrabbable;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float xSpeedOffset = 0.0f;
        if(xSpeedSliderGrabbable.GetIsGrabbed()) {
            xSpeedOffset = Mathf.Lerp(minDPS, maxDPS, xSpeedSlider.percentage);
            Debug.Log("X: " + xSpeedSlider.percentage);
        }
        xSpeedSlider.SetSpring();
        transform.Rotate(Vector3.right * (xSpeedOffset + degreesPerSecond) * Time.deltaTime);

        float zSpeedOffset = 0.0f;
        if(zSpeedSliderGrabbable.GetIsGrabbed()) {
            zSpeedOffset = Mathf.Lerp(minDPS, maxDPS, zSpeedSlider.percentage);
            Debug.Log("Z: " + zSpeedSlider.percentage);
        }
        zSpeedSlider.SetSpring();
        transform.Rotate(Vector3.forward * (zSpeedOffset + degreesPerSecond) * Time.deltaTime);
    }
}
