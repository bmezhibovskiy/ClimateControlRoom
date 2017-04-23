using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour {

    public GameObject emptyGraphPrefab;

    public VRBasics_Lever goalLever;
    public VRBasics_Grabbable goalLeverGrabbable;

    public WMG_Axis_Graph graph;

    public WMG_Series series;
    public List<Vector2> seriesData;

    public float goalYear = 2100;
    public float lowestTemp = 37.0f;
    public float highestTemp = 45.0f;
    private float? goalTemp;

    public string dataPath;

	// Use this for initialization
	void Start () {
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
    }

    void LateUpdate() {
        if(goalLeverGrabbable.GetIsGrabbed()) {
            float percentage = goalLever.hinge.GetComponent<VRBasics_Hinge>().percentage;
            float newGoalTemperature = Mathf.Lerp(lowestTemp, highestTemp, percentage);            
            if(goalTemp == null || newGoalTemperature != goalTemp.Value) {
                Vector2 lastRow = seriesData[seriesData.Count - 1];
                lastRow.y = newGoalTemperature;
                seriesData.RemoveAt(seriesData.Count - 1);
                lastRow.y = newGoalTemperature;
                seriesData.Add(lastRow);
                series.pointValues.SetList(seriesData);
                goalTemp = newGoalTemperature;
            }
        }
    }

    public float? GetGoalTemperature() {
        return goalTemp;
    }
}
