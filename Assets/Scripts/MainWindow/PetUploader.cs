using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetUploader : MonoBehaviour
{
    public Button petObject;
    public Sprite pet;
    // Start is called before the first frame update
    void Start()
    {
        /* get the pet of the user
         * set the image of the user to that pet
         */

        petObject.GetComponent<Image>().sprite = pet;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
