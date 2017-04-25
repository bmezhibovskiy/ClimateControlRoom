using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarbonTaxController : MonoBehaviour {
    
    public Text carbonTaxText;
    
    public const float minTax = 0.0f;
    public const float maxTax = 100.0f;
    private float tax = minTax;


    // Use this for initialization
    void Start () {       
        carbonTaxText.text = tax.ToString("C");
    }

    public float GetTax() {
        return tax;
    }

    public void SetTax(float newTax) {
        tax = System.Math.Max(newTax, 0);
        carbonTaxText.text = tax.ToString("C");
    }
}
