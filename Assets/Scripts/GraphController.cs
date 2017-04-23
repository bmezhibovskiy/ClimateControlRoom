using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour {

    public GameObject emptyGraphPrefab;
    public CarbonTaxController carbonTaxController;

    public GameObject earthHalo;
    public float maxAlpha = 0.16f;

    public VRBasics_Lever goalLever;
    public VRBasics_Grabbable goalLeverGrabbable;

    public WMG_Axis_Graph graph;

    public WMG_Series series;
    public List<Vector2> seriesData;

    public const float goalYear = 2100;
    public const float lowestTemp = 37.0f;
    public const float highestTemp = 45.0f;
    private float? goalTemp;

    private VRBasics_Hinge hinge;

    public string dataPath;

	// Use this for initialization
	void Start () {
        hinge = goalLever.hinge.GetComponent<VRBasics_Hinge>();

        GameObject graphGO = GameObject.Instantiate(emptyGraphPrefab);
        graphGO.transform.SetParent(this.transform, false);
        graph = graphGO.GetComponent<WMG_Axis_Graph>();
        series = graph.addSeries();
        
        string csvString = System.IO.File.ReadAllText(Application.dataPath + "\\" + dataPath);

        string[] lines = csvString.Split(new char[] {'\n'});
        
        foreach(string line in lines) {
            string[] row = line.Split(new char[] { ',' });
            seriesData.Add(new Vector2(float.Parse(row[0]), float.Parse(row[1])));
        }
        goalTemp = seriesData[seriesData.Count - 1].y;

        series.pointValues.SetList(seriesData);

        Color color = earthHalo.GetComponent<MeshRenderer>().material.GetColor("_Color");
        color.a = maxAlpha * ((goalTemp.Value - lowestTemp) / (highestTemp - lowestTemp));
        earthHalo.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }   

    public float? GetGoalTemperature() {
        return goalTemp;
    }

    public void SetGoalTemperature(float newGoalTemperature) {
        Vector2 lastRow = seriesData[seriesData.Count - 1];
        lastRow.y = newGoalTemperature;
        seriesData.RemoveAt(seriesData.Count - 1);
        lastRow.y = newGoalTemperature;
        seriesData.Add(lastRow);
        series.pointValues.SetList(seriesData);
        goalTemp = newGoalTemperature;

        Color color = earthHalo.GetComponent<MeshRenderer>().material.GetColor("_Color");        
        color.a = maxAlpha * ((newGoalTemperature - lowestTemp) / (highestTemp - lowestTemp));
        earthHalo.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }
}
