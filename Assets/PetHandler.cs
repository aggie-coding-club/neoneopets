using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PetHandler : MonoBehaviour
{
    public List<Sprite> pets;
    public Button petImage;
    public int index;
    public Button rightButton;
    public Button leftButton;
    public Button selectButton;
    public List<string> petNames;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        rightButton.onClick.AddListener(OnRightClick);
        leftButton.onClick.AddListener(OnLeftClick);
        selectButton.onClick.AddListener(Select);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRightClick()
    {
        if (index < pets.Count - 1)
        {
            index += 1;
            petImage.GetComponent<Image>().sprite = pets[index];
        } else
        {
            index = 0;
            petImage.GetComponent<Image>().sprite = pets[index];
        }
    }
    void OnLeftClick()
    {
        if (index > 0)
        {
            index -= 1;
            petImage.GetComponent<Image>().sprite = pets[index];
        } else
        {
            index = pets.Count - 1;
            petImage.GetComponent<Image>().sprite = pets[index];
        }
    }
    void Select()
    {
        string selectedPet = petNames[index];
        /*send to mongo*/
        SceneManager.LoadSceneAsync("WelcomeScreen");
    }
}
