using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WarmingFactor {
    string type;
    VRBasics_Lever lever;
    VRBasics_Grabbable grabbable;
    VRBasics_Hinge hinge;
    public WarmingFactor(string type, VRBasics_Lever lever, VRBasics_Grabbable grabbable, VRBasics_Hinge hinge) {
        this.type = type;
        this.lever = lever;
        this.grabbable = grabbable;
        this.hinge = hinge;
    }
}

public class LeverController : MonoBehaviour {

    public CarbonTaxController carbonTaxController;
    public GraphController graphController;
    
    public int taxPriority;
    public VRBasics_Lever taxLever;
    public VRBasics_Grabbable taxLeverGrabbable;
    private VRBasics_Hinge taxHinge;

    public int coalPriority;
    public VRBasics_Lever coalLever;
    public VRBasics_Grabbable coalLeverGrabbable;
    private VRBasics_Hinge coalHinge;

    public VRBasics_Lever goalLever;
    public VRBasics_Grabbable goalLeverGrabbable;
    private VRBasics_Hinge goalHinge;

    private List<WarmingFactor> factors;

    private const int numFactors = 2; //Change this every time a factor is added



    void Start () {
        taxHinge = taxLever.hinge.GetComponent<VRBasics_Hinge>();
        coalHinge = coalLever.hinge.GetComponent<VRBasics_Hinge>();
        goalHinge = goalLever.hinge.GetComponent<VRBasics_Hinge>();

        factors = new List<WarmingFactor>(numFactors);
        factors.Insert(taxPriority, new WarmingFactor("tax", taxLever, taxLeverGrabbable, taxHinge));
        factors.Insert(coalPriority, new WarmingFactor("coal", coalLever, coalLeverGrabbable, coalHinge));
    }

	void LateUpdate () {
        if(goalLeverGrabbable.GetIsGrabbed()) {
            float? goalTemp = graphController.GetGoalTemperature();
            float percentage = goalHinge.percentage;
            float newGoalTemperature = Mathf.Lerp(GraphController.lowestTemp, GraphController.highestTemp, percentage);
            if(goalTemp == null || newGoalTemperature != goalTemp.Value) {
                graphController.SetGoalTemperature(newGoalTemperature);
            }
            
            float newTax = -15.5f * (newGoalTemperature) + 676.0f;
            carbonTaxController.SetTax(newTax);
            float leverPercentage = (Mathf.Max(newTax, 0f) - CarbonTaxController.minTax) / (CarbonTaxController.maxTax - CarbonTaxController.minTax);
            float newAngle = Mathf.Lerp(taxHinge.limitMin, taxHinge.limitMax, leverPercentage);
            if(newAngle != taxHinge.angle) {
                taxHinge.SetAngle(newAngle);
            }

        }
        else if(taxLeverGrabbable.GetIsGrabbed()) {
            float percentage = taxLever.hinge.GetComponent<VRBasics_Hinge>().percentage;
            float newTax = Mathf.Lerp(CarbonTaxController.minTax, CarbonTaxController.maxTax, percentage);
            float tax = carbonTaxController.GetTax();
            if(newTax != tax) {
                carbonTaxController.SetTax(newTax);
            }

            float newGoalTemp = -0.062f * newTax + 43.55f;
            graphController.SetGoalTemperature(newGoalTemp);
            float leverPercentage = (newGoalTemp - GraphController.lowestTemp) / (GraphController.highestTemp - GraphController.lowestTemp);
            float newAngle = Mathf.Lerp(goalHinge.limitMin, goalHinge.limitMax, leverPercentage);
            if(newAngle != goalHinge.angle) {
                goalHinge.SetAngle(newAngle);
            }
        }
    }

    //WIP
    private void UpdateFactors() {
        for(int i = 0; i < numFactors; ++i) {
            WarmingFactor factor = factors[i];
        }
    }
}
