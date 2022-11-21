using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] GameObject TextBox;
    Color newColor;
    Color red;
    Color green;
    Color blue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //newColor = TextBox.GetComponent<TextMeshProUGUI>().color;
        //TextBox.GetComponent<TextMeshProUGUI>().color = new Color(newColor.r+Random.Range(-1.0f*Time.deltaTime,1.0f * Time.deltaTime), newColor.g + Random.Range(-1.0f * Time.deltaTime, 1.0f * Time.deltaTime), newColor.b + Random.Range(-1.0f * Time.deltaTime, 1.0f * Time.deltaTime), newColor.a);
    }
}
