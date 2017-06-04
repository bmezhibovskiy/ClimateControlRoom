using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour {

    public SteamVR_TrackedController controller;
    public LineRenderer lineRenderer;

    private GameObject currentObject = null;

    private float range = 100f;

    // Use this for initialization
    void Start() {
        controller.TriggerClicked += triggerDown;
    }

    // Update is called once per frame
    void Update() {

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo)) {
            if (currentObject != null) {
                currentObject = null;
                range = 100f;
            }
            currentObject = hitInfo.collider.gameObject;
            if (currentObject != null) {
                range = hitInfo.distance;
            }
        }
        else {
            currentObject = null;
            range = 100f;
        }

        if (controller.triggerPressed) {
            if (currentObject != null) {
                IPickable pickable = currentObject.GetComponent(typeof(IPickable)) as IPickable;
                if (pickable != null) {
                    pickable.Pick(transform.TransformPoint(Vector3.forward * range));
                }
            }
        }


        lineRenderer.SetPosition(lineRenderer.positionCount - 1, Vector3.forward * range);
    }

    void triggerDown(object sender, ClickedEventArgs e) {
        
    }
}
