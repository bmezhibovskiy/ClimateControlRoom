using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WarmingFactor : System.IComparable<WarmingFactor> {
    public string type;
    public int priority;
    public VRBasics_Lever lever;
    public VRBasics_Grabbable grabbable;
    public VRBasics_Hinge hinge;
    public WarmingFactorUpdateDelegate update;
    public delegate float WarmingFactorUpdateDelegate(float carbonAlreadyRemoved);
    public WarmingFactor(string type, int priority, VRBasics_Lever lever, VRBasics_Grabbable grabbable, VRBasics_Hinge hinge, WarmingFactorUpdateDelegate update) {
        this.type = type;
        this.lever = lever;
        this.grabbable = grabbable;
        this.hinge = hinge;
        this.update = update;
        this.priority = priority;
    }
    public int CompareTo(WarmingFactor other) {
        return this.priority.CompareTo(other.priority);
    }
}

public class LeverController : MonoBehaviour {

    public CarbonTaxController carbonTaxController;
    public PowerPlantTableController powerPlantController;
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

    void Start() {
        taxHinge = taxLever.hinge.GetComponent<VRBasics_Hinge>();
        coalHinge = coalLever.hinge.GetComponent<VRBasics_Hinge>();
        goalHinge = goalLever.hinge.GetComponent<VRBasics_Hinge>();

        factors = new List<WarmingFactor>(numFactors);
        factors.Add(new WarmingFactor("tax", taxPriority, taxLever, taxLeverGrabbable, taxHinge, (float carbonAlreadyRemoved) => {
            const float maxCO2TonsPerYearCausedByNoTax = 13333333333f; //~13B            
            return carbonTaxController.UpdateTax(taxHinge.percentage, Mathf.Min(TotalTonsOfCO2InAtmospherePerYear() - carbonAlreadyRemoved, maxCO2TonsPerYearCausedByNoTax));
        }));
        factors.Add(new WarmingFactor("coal", coalPriority, coalLever, coalLeverGrabbable, coalHinge, (float carbonAlreadyRemoved) => {
            const float maxCO2TonsPerYearEmittedByCoal = 7143416583f; //~7B                
            return powerPlantController.UpdatePowerPlants(coalHinge.percentage, Mathf.Min(TotalTonsOfCO2InAtmospherePerYear() - carbonAlreadyRemoved, maxCO2TonsPerYearEmittedByCoal));
        }));
        factors.Sort();
    }

    void LateUpdate() {
        if (goalLeverGrabbable.GetIsGrabbed()) {
            float newGoalTemperature = Mathf.Lerp(GraphController.lowestTemp, GraphController.highestTemp, goalHinge.percentage);
            if (newGoalTemperature != graphController.GetGoalTemperature()) {
                graphController.SetGoalTemperature(newGoalTemperature);
            }
        }
        UpdateFactors();
    }

    private void UpdateFactors() {
        float carbonRemoved = 0f;
        for (int i = 0; i < numFactors; ++i) {
            WarmingFactor factor = factors[i];
            carbonRemoved += factor.update(carbonRemoved);
        }
        graphController.SetProjectedTemperature(NewProjectedTemperature(carbonRemoved));
    }

    private float TotalTonsOfCO2InAtmospherePerYear() {
        float anomaly = graphController.GetGoalTemperature() - GraphController.idealTemperature;
        return (-3333333f * anomaly + 16666670f) * 1000f;
    }

    //Approximated by linear regression
    private float NewProjectedTemperature(float tonsOfCO2RemovedPerYear) {
        return GraphController.idealTemperature + -3e-10f * tonsOfCO2RemovedPerYear + 5.0f;
    }
}
