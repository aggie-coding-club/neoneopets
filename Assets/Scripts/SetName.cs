using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetName : MonoBehaviour
{
    [SerializeField] GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        text.GetComponent<TextMeshProUGUI>().text = SceneChanger.Instance.username;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
