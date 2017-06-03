using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphController : MonoBehaviour {

    public GameObject emptyGraphPrefab;

    public GameObject earthHalo;
    public float maxAlpha = 0.16f;
    
    public WMG_Axis_Graph graph;

    public WMG_Series series;
    public List<Vector2> seriesData;

    public WMG_Series goalSeries;
    public List<Vector2> goalSeriesData;

    public const float goalYear = 2100;
    public const float lowestTemp = 37.0f;
    public const float highestTemp = 45.0f;
    public const float idealTemperature = 38.5f;
    private float? projectedTemp;
    private float? goalTemp;

    public string dataPath;

	// Use this for initialization
	void Start () {

        GameObject graphGO = GameObject.Instantiate(emptyGraphPrefab);
        graphGO.transform.SetParent(this.transform, false);
        graph = graphGO.GetComponent<WMG_Axis_Graph>();
        series = graph.addSeries();
        goalSeries = graph.addSeries();
        goalSeries.lineColor = Color.blue;
        
        string csvString = System.IO.File.ReadAllText(Application.dataPath + "\\" + dataPath);

        string[] lines = csvString.Split(new char[] {'\n'});
        
        foreach(string line in lines) {
            string[] row = line.Split(new char[] { ',' });
            seriesData.Add(new Vector2(float.Parse(row[0]), float.Parse(row[1])));
        }
        projectedTemp = seriesData[seriesData.Count - 1].y;
        goalTemp = projectedTemp;

        goalSeriesData.Add(seriesData[seriesData.Count - 2]);
        goalSeriesData.Add(seriesData[seriesData.Count - 1]);

        series.pointValues.SetList(seriesData);
        goalSeries.pointValues.SetList(goalSeriesData);

        Color color = earthHalo.GetComponent<MeshRenderer>().material.GetColor("_Color");
        color.a = maxAlpha * ((goalTemp.Value - lowestTemp) / (highestTemp - lowestTemp));
        earthHalo.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    public float GetProjectedTemperature() {
        if (projectedTemp == null)
        {
            return highestTemp;
        }
        return projectedTemp.Value;
    }

    public void SetProjectedTemperature(float newProjectedTemperature) {
        Vector2 lastRow = seriesData[seriesData.Count - 1];
        lastRow.y = newProjectedTemperature;
        seriesData.RemoveAt(seriesData.Count - 1);
        seriesData.Add(lastRow);
        series.pointValues.SetList(seriesData);
        projectedTemp = newProjectedTemperature;

        Color color = earthHalo.GetComponent<MeshRenderer>().material.GetColor("_Color");
        color.a = maxAlpha * ((newProjectedTemperature - lowestTemp) / (highestTemp - lowestTemp));
        earthHalo.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    public float GetGoalTemperature() {
        if (goalTemp == null) {
            return highestTemp;
        }
        return goalTemp.Value;
    }

    public void SetGoalTemperature(float newGoalTemperature) {
        Vector2 lastRow = goalSeriesData[goalSeriesData.Count - 1];
        lastRow.y = newGoalTemperature;
        goalSeriesData.RemoveAt(goalSeriesData.Count - 1);
        goalSeriesData.Add(lastRow);
        goalSeries.pointValues.SetList(goalSeriesData);
        goalTemp = newGoalTemperature;
    }
}
