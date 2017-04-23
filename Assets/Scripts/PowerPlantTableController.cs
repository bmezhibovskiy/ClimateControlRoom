using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tacticsoft;

class PPRowData : System.IEquatable<PPRowData>, System.IComparable<PPRowData> {
    public string pp_name;
    public float pp_tonsCarbon;
    public float pp_priority;

    public PPRowData(string pp_name, float pp_tonsCarbon, float pp_priority) {
        this.pp_name = pp_name;
        this.pp_tonsCarbon = pp_tonsCarbon;
        this.pp_priority = pp_priority;
    }

    public bool Equals(PPRowData other) {
        return other.pp_name == pp_name;
    }

    public int CompareTo(PPRowData other) {
        int compareValue = pp_priority.CompareTo(other.pp_priority);
        if(compareValue == 0) {
            return pp_name.CompareTo(other.pp_name);
        }
        return compareValue;
    }

}

//An example implementation of a class that communicates with a TableView
public class PowerPlantTableController: MonoBehaviour, ITableViewDataSource {
    public PowerPlantCell m_cellPrefab;
    public TableView m_tableView;
    public GraphController graphController;

    public string dataPath;

    private int m_numInstancesCreated = 0;
    private List<PPRowData> rows = new List<PPRowData>();
    private double? oldGoalTemp = null;
    private List<PPRowData> filteredRows;

    //Register as the TableView's delegate (required) and data source (optional)
    //to receive the calls
    void Start() {
        string csvString = System.IO.File.ReadAllText(Application.dataPath + "\\" + dataPath);

        string[] lines = csvString.Split(new char[] { '\n' });

        foreach(string line in lines) {
            string[] row = line.Split(new char[] { ',' });
            if(row.Length > 6) {
                try {                    
                    float priority = float.Parse(row[6]);
                    float tonsCarbon = float.Parse(row[4]);                    
                    PPRowData rowData = new PPRowData(row[0], tonsCarbon, priority);
                    rows.Add(rowData);
                }
                catch (System.FormatException fe) {
                    Debug.Log(fe);
                }
            }
        }

        m_tableView.dataSource = this;
    }

    void Update() {
        m_tableView.ReloadData();        
    }

    #region ITableViewDataSource

    //Will be called by the TableView to know how many rows are in this table
    public int GetNumberOfRowsForTableView(TableView tableView) {
        return GetFilteredRows().Count + 1; //+1 for totals row
    }

    //Will be called by the TableView to know what is the height of each row
    public float GetHeightForRowInTableView(TableView tableView, int row) {
        return (m_cellPrefab.transform as RectTransform).rect.height;
    }

    //Will be called by the TableView when a cell needs to be created for display
    public TableViewCell GetCellForRowInTableView(TableView tableView, int rowIndex) {
        PowerPlantCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as PowerPlantCell;
        if(cell == null) {            
            cell = (PowerPlantCell)GameObject.Instantiate(m_cellPrefab);
            cell.name = "PowerPlantCellInstance_" + (++m_numInstancesCreated).ToString();
        }
        if(rowIndex == 0) {
            cell.pp_name.text = "Total";
            cell.pp_carbon.text = GetFilteredRows().Count.ToString();
        }
        else {
            PPRowData row = GetFilteredRows()[rowIndex - 1];
            cell.pp_name.text = row.pp_name;
            const float kTonsToMegatons = 0.000001f;
            string formattedMegatonsCarbon = (row.pp_tonsCarbon * kTonsToMegatons).ToString("N1") + " Mt";
            cell.pp_carbon.text = formattedMegatonsCarbon;
        }
        return cell;
    }

    private List<PPRowData> GetFilteredRows() {        
        double? goalTemperature = (double?)graphController.GetGoalTemperature();
        if(goalTemperature != null && (oldGoalTemp == null || oldGoalTemp != goalTemperature)) {
            oldGoalTemp = goalTemperature;   
            rows.Sort();
            rows.Reverse();

            //TODO: Move this somewhere more global
            const double idealTemperature = 38.5;

            double threshold = System.Math.Max(-3333333000.0 * (goalTemperature.Value - idealTemperature) + 16666670000.0, 0);
            double carbonRemoved = 0;

            filteredRows = new List<PPRowData>();
            foreach(PPRowData row in rows) {
                if(carbonRemoved >= System.Math.Abs(threshold)) {
                    break;
                }                
                filteredRows.Add(row);
                carbonRemoved += (double)row.pp_tonsCarbon;
            }
        }        
        return filteredRows;
    }

    #endregion
}