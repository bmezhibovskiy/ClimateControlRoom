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
        tax = newTax;
        carbonTaxText.text = tax.ToString("C");
    }

    public float UpdateTax(float percentage, float maxCarbonThatCanBeRemoved) {
        float adjustedMaxTax = Mathf.Clamp((float)(0.0000054*(double)maxCarbonThatCanBeRemoved - 4.0), minTax, maxTax);
        float newTax = Mathf.Clamp(Mathf.Lerp(minTax, maxTax, percentage), minTax, adjustedMaxTax);
        if(newTax != GetTax()) {
            SetTax(newTax);
        }
        return 182926.8f * GetTax() + 813008.1f;
    }
}
