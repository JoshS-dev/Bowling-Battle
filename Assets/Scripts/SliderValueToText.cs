using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueToText : MonoBehaviour
{
    [SerializeField]
    Slider targetSlider;
    private TextMeshProUGUI textValue;

    [SerializeField]
    string suffix;
    [SerializeField]
    float scalar = 1f;
    [SerializeField]
    int decimalPlaces = 0;

    // Start is called before the first frame update
    void Awake(){
        textValue = GetComponent<TextMeshProUGUI>();
        UpdateTextValue();
    }

    public void UpdateTextValue() {
        string sliderVal = (scalar * targetSlider.value).ToString("F"+decimalPlaces.ToString()) + suffix;
        textValue.text = sliderVal;
    }
}
