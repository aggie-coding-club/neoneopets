using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class Tetris : MonoBehaviour
{
    [SerializeField] public List<GameObject> blocks;

    private Vector3 pos;
    private GameObject controlledObject;
    private Quaternion rot;
    float timer = 0;
    int n = 1;
    int random;

    GameObject child1;
    GameObject child2;
    GameObject child3;
    GameObject child4;

    GameObject holdObject;
    GameObject firstObject;
    GameObject secondObject;
    GameObject thirdObject;

    RaycastHit2D raycastChild1down;
    RaycastHit2D raycastChild2down;
    RaycastHit2D raycastChild3down;
    RaycastHit2D raycastChild4down;

    RaycastHit2D raycastChild1left;
    RaycastHit2D raycastChild2left;
    RaycastHit2D raycastChild3left;
    RaycastHit2D raycastChild4left;

    RaycastHit2D raycastChild1right;
    RaycastHit2D raycastChild2right;
    RaycastHit2D raycastChild3right;
    RaycastHit2D raycastChild4right;

    int holdNumber = -1;
    int currentNum;
    int firstListNum;
    int secondListNum;
    int thirdListNum;

    bool alreadyHeld = false;

    GameObject[] BlocksObjects;
    GameObject[] newBlocksObjects;

    RaycastHit2D[,] array = new RaycastHit2D[20,10];

    int gameScore = 0;
    int gameLevel = 1;
    int gameLines = 0;

    GameObject scoreSpace;
    GameObject levelSpace;
    GameObject lineSpace;

    GameObject loseBackground;
    GameObject gameOver;
    GameObject finalScore;
    GameObject finalScoreNumber;

    RaycastHit2D loseCollider;

    int GameEnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        rot = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z);
        pos = this.transform.position;

        currentNum = Random.Range(0, blocks.Count);
        firstListNum = Random.Range(0, blocks.Count);
        secondListNum = Random.Range(0, blocks.Count);
        thirdListNum = Random.Range(0, blocks.Count);

        controlledObject = Instantiate(blocks[currentNum],pos,rot);
        firstObject = Instantiate(blocks[firstListNum], new Vector3(4.87f, 1.93f,0f), rot);
        secondObject = Instantiate(blocks[secondListNum], new Vector3(4.87f, -0.58f,0f), rot);
        thirdObject = Instantiate(blocks[thirdListNum], new Vector3(4.87f, -3.19f, 0f), rot);

        firstObject.tag = "not";
        secondObject.tag = "not";
        thirdObject.tag = "not";

        child1 = controlledObject.transform.GetChild(0).gameObject;
        child2 = controlledObject.transform.GetChild(1).gameObject;
        child3 = controlledObject.transform.GetChild(2).gameObject;
        child4 = controlledObject.transform.GetChild(3).gameObject;

        loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 4.25f, 136.8307f),Vector2.right,100f);
        
        

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                array[i, j] = Physics2D.Raycast(new Vector3(-1.84f+ (0.41f*j), 3.83f-(0.41f*i), -1.036217f),Vector2.up,0.1f);
            }
        }

        scoreSpace = GameObject.Find("/Canvas/ScoreNumber");
        levelSpace = GameObject.Find("/Canvas/LevelNumber");
        lineSpace = GameObject.Find("/Canvas/LinesNumber");

        loseBackground = GameObject.Find("/Canvas/LoseBackground");
        gameOver = GameObject.Find("/Canvas/GameOver");
        finalScore = GameObject.Find("/Canvas/FinalScore");
        finalScoreNumber = GameObject.Find("/Canvas/FinalScoreNumber");

        loseBackground.SetActive(false);
        gameOver.SetActive(false);
        finalScore.SetActive(false);
        finalScoreNumber.SetActive(false);

        scoreSpace.GetComponent<TextMeshProUGUI>().text = "" + gameScore;
        levelSpace.GetComponent<TextMeshProUGUI>().text = "" + gameLevel;
        lineSpace.GetComponent<TextMeshProUGUI>().text = "" + gameLines;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEnd == 0)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    array[i, j] = Physics2D.Raycast(new Vector3(-1.84f + (0.41f * j), 3.83f - (0.41f * i), -1.036217f), Vector2.up, 0.1f);
                }

                if (array[i, 0].collider != null && array[i, 1].collider != null && array[i, 2].collider != null && array[i, 3].collider != null && array[i, 4].collider != null && array[i, 5].collider != null && array[i, 6].collider != null && array[i, 7].collider != null && array[i, 8].collider != null && array[i, 9].collider != null)
                {
                    Destroy(array[i, 0].collider.gameObject);
                    Destroy(array[i, 1].collider.gameObject);
                    Destroy(array[i, 2].collider.gameObject);
                    Destroy(array[i, 3].collider.gameObject);
                    Destroy(array[i, 4].collider.gameObject);
                    Destroy(array[i, 5].collider.gameObject);
                    Destroy(array[i, 6].collider.gameObject);
                    Destroy(array[i, 7].collider.gameObject);
                    Destroy(array[i, 8].collider.gameObject);
                    Destroy(array[i, 9].collider.gameObject);

                    BlocksObjects = GameObject.FindGameObjectsWithTag("Block");
                    for (int j = 0; j < BlocksObjects.Length; j++)
                    {
                        if (BlocksObjects[j].transform.position.y < 3.83f - (0.41f * i))
                        {
                            BlocksObjects[j] = null;
                        }
                    }
                    for (int j = 0; j < BlocksObjects.Length; j++)
                    {
                        if (BlocksObjects[j] != null)
                        {
                            BlocksObjects[j].transform.position += new Vector3(0f, -0.41f, 0f);
                        }
                    }
                    BlocksObjects = GameObject.FindGameObjectsWithTag("not");
                    for (int j = 0; j < BlocksObjects.Length; j++)
                    {
                        BlocksObjects[j].transform.position += new Vector3(0f, 0.41f, 0f);
                    }
                    gameScore += 1000;
                    gameLevel = (gameScore / 10000) + 1;
                    gameLines += 1;

                    scoreSpace.GetComponent<TextMeshProUGUI>().text = "" + gameScore;
                    levelSpace.GetComponent<TextMeshProUGUI>().text = "" + gameLevel;
                    lineSpace.GetComponent<TextMeshProUGUI>().text = "" + gameLines;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if ((child1.transform.position.x >= -1.8 && child2.transform.position.x >= -1.8 && child3.transform.position.x >= -1.8 && child4.transform.position.x >= -1.8) && (raycastChild1left.collider == null && raycastChild2left.collider == null && raycastChild3left.collider == null && raycastChild4left.collider == null))
                {
                    controlledObject.transform.position += new Vector3(-0.41f, 0, 0);

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if ((child1.transform.position.x < 1.8 && child2.transform.position.x < 1.8 && child3.transform.position.x < 1.8 && child4.transform.position.x < 1.8) && (raycastChild1right.collider == null && raycastChild2right.collider == null && raycastChild3right.collider == null && raycastChild4right.collider == null))
                {
                    controlledObject.transform.position += new Vector3(0.41f, 0, 0);

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (child1.transform.position.y <= -3.6 || child2.transform.position.y <= -3.6 || child3.transform.position.y <= -3.6 || child4.transform.position.y <= -3.6)
                {
                    child1.AddComponent<BoxCollider2D>();
                    child2.AddComponent<BoxCollider2D>();
                    child3.AddComponent<BoxCollider2D>();
                    child4.AddComponent<BoxCollider2D>();

                    currentNum = firstListNum;
                    firstListNum = secondListNum;
                    secondListNum = thirdListNum;
                    thirdListNum = Random.Range(0, blocks.Count);

                    controlledObject = Instantiate(blocks[currentNum], pos, rot);

                    Destroy(firstObject);
                    Destroy(secondObject);
                    Destroy(thirdObject);

                    firstObject = Instantiate(blocks[firstListNum], new Vector3(4.87f, 1.93f, 0f), rot);
                    secondObject = Instantiate(blocks[secondListNum], new Vector3(4.87f, -0.58f, 0f), rot);
                    thirdObject = Instantiate(blocks[thirdListNum], new Vector3(4.87f, -3.19f, 0f), rot);
                    alreadyHeld = false;

                    if (holdObject != null)
                    {
                        holdObject.tag = "not";
                    }
                    firstObject.tag = "not";
                    secondObject.tag = "not";
                    thirdObject.tag = "not";

                    child1 = controlledObject.transform.GetChild(0).gameObject;
                    child2 = controlledObject.transform.GetChild(1).gameObject;
                    child3 = controlledObject.transform.GetChild(2).gameObject;
                    child4 = controlledObject.transform.GetChild(3).gameObject;

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);


                    timer = 0;
                }

                if (raycastChild1down.collider != null || raycastChild2down.collider != null || raycastChild3down.collider != null || raycastChild4down.collider != null)
                {
                    child1.AddComponent<BoxCollider2D>();
                    child2.AddComponent<BoxCollider2D>();
                    child3.AddComponent<BoxCollider2D>();
                    child4.AddComponent<BoxCollider2D>();

                    currentNum = firstListNum;
                    firstListNum = secondListNum;
                    secondListNum = thirdListNum;
                    thirdListNum = Random.Range(0, blocks.Count);

                    controlledObject = Instantiate(blocks[currentNum], pos, rot);

                    Destroy(firstObject);
                    Destroy(secondObject);
                    Destroy(thirdObject);

                    firstObject = Instantiate(blocks[firstListNum], new Vector3(4.87f, 1.93f, 0f), rot);
                    secondObject = Instantiate(blocks[secondListNum], new Vector3(4.87f, -0.58f, 0f), rot);
                    thirdObject = Instantiate(blocks[thirdListNum], new Vector3(4.87f, -3.19f, 0f), rot);
                    alreadyHeld = false;

                    if (holdObject != null)
                    {
                        holdObject.tag = "not";
                    }
                    firstObject.tag = "not";
                    secondObject.tag = "not";
                    thirdObject.tag = "not";

                    child1 = controlledObject.transform.GetChild(0).gameObject;
                    child2 = controlledObject.transform.GetChild(1).gameObject;
                    child3 = controlledObject.transform.GetChild(2).gameObject;
                    child4 = controlledObject.transform.GetChild(3).gameObject;

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);
                }

                else
                {
                    controlledObject.transform.position += new Vector3(0, -0.41f, 0);

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    timer = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                controlledObject.transform.rotation = Quaternion.Euler(0, 0, controlledObject.transform.rotation.z + (90 * n));
                for (int i = 0; i < 4; i++)
                {
                    if (child1.transform.position.x < -1.8 || child2.transform.position.x < -1.8 || child3.transform.position.x < -1.8 || child4.transform.position.x < -1.8)
                    {
                        controlledObject.transform.position += new Vector3(0.41f, 0, 0);
                    }

                    if (child1.transform.position.x > 1.8 || child2.transform.position.x > 1.8 || child3.transform.position.x > 1.8 || child4.transform.position.x > 1.8)
                    {
                        controlledObject.transform.position += new Vector3(-0.41f, 0, 0);
                    }
                }
                n += 1;
            }

            timer += Time.deltaTime;

            if (timer > 1.5/gameLevel)
            {
                if (child1.transform.position.y <= -3.6 || 
                    child2.transform.position.y <= -3.6 || 
                    child3.transform.position.y <= -3.6 || 
                    child4.transform.position.y <= -3.6)
                {
                    child1.AddComponent<BoxCollider2D>();
                    child2.AddComponent<BoxCollider2D>();
                    child3.AddComponent<BoxCollider2D>();
                    child4.AddComponent<BoxCollider2D>();

                    currentNum = Random.Range(0, blocks.Count);
                    controlledObject = Instantiate(blocks[currentNum], pos, rot);
                    alreadyHeld = false;

                    if (holdObject != null)
                    {
                        holdObject.tag = "not";
                    }
                    firstObject.tag = "not";
                    secondObject.tag = "not";
                    thirdObject.tag = "not";

                    child1 = controlledObject.transform.GetChild(0).gameObject;
                    child2 = controlledObject.transform.GetChild(1).gameObject;
                    child3 = controlledObject.transform.GetChild(2).gameObject;
                    child4 = controlledObject.transform.GetChild(3).gameObject;

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);

                    timer = 0;
                }

                if (raycastChild1down.collider != null || raycastChild2down.collider != null || raycastChild3down.collider != null || raycastChild4down.collider != null)
                {
                    child1.AddComponent<BoxCollider2D>();
                    child2.AddComponent<BoxCollider2D>();
                    child3.AddComponent<BoxCollider2D>();
                    child4.AddComponent<BoxCollider2D>();

                    currentNum = Random.Range(0, blocks.Count);
                    controlledObject = Instantiate(blocks[currentNum], pos, rot);

                    Destroy(firstObject);
                    Destroy(secondObject);
                    Destroy(thirdObject);

                    firstObject = Instantiate(blocks[firstListNum], new Vector3(4.87f, 1.93f, 0f), rot);
                    secondObject = Instantiate(blocks[secondListNum], new Vector3(4.87f, -0.58f, 0f), rot);
                    thirdObject = Instantiate(blocks[thirdListNum], new Vector3(4.87f, -3.19f, 0f), rot);

                    alreadyHeld = false;

                    if (holdObject != null)
                    {
                        holdObject.tag = "not";
                    }
                    firstObject.tag = "not";
                    secondObject.tag = "not";
                    thirdObject.tag = "not";

                    child1 = controlledObject.transform.GetChild(0).gameObject;
                    child2 = controlledObject.transform.GetChild(1).gameObject;
                    child3 = controlledObject.transform.GetChild(2).gameObject;
                    child4 = controlledObject.transform.GetChild(3).gameObject;

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);
                }

                else
                {
                    controlledObject.transform.position += new Vector3(0, -0.41f, 0);

                    raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                    raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                    raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                    raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                    raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                    raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                    raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                    raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                    raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                    raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                    raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                    raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                    timer = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (alreadyHeld == false)
                {
                    if (holdNumber == -1)
                    {
                        Destroy(controlledObject);
                        holdNumber = currentNum;
                        currentNum = firstListNum;
                        firstListNum = secondListNum;
                        secondListNum = thirdListNum;
                        thirdListNum = Random.Range(0, blocks.Count);
                        controlledObject = Instantiate(blocks[currentNum], pos, rot);
                        Destroy(firstObject);
                        Destroy(secondObject);
                        Destroy(thirdObject);
                        firstObject = Instantiate(blocks[firstListNum], new Vector3(4.87f, 1.93f, 0f), rot);
                        secondObject = Instantiate(blocks[secondListNum], new Vector3(4.87f, -0.58f, 0f), rot);
                        thirdObject = Instantiate(blocks[thirdListNum], new Vector3(4.87f, -3.19f, 0f), rot);
                        holdObject = Instantiate(blocks[holdNumber], pos + new Vector3(-5, -2, 0), rot);
                        holdObject.tag = "not";
                        firstObject.tag = "not";
                        secondObject.tag = "not";
                        thirdObject.tag = "not";

                        child1 = controlledObject.transform.GetChild(0).gameObject;
                        child2 = controlledObject.transform.GetChild(1).gameObject;
                        child3 = controlledObject.transform.GetChild(2).gameObject;
                        child4 = controlledObject.transform.GetChild(3).gameObject;

                        raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                        raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                        raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                        raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                        raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                        raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                        raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                        raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                        raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                        raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                        raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                        raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                        loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);

                        alreadyHeld = true;
                    }
                    else
                    {
                        Destroy(controlledObject);
                        Destroy(holdObject);
                        int temp = holdNumber;
                        holdNumber = currentNum;
                        currentNum = temp;
                        controlledObject = Instantiate(blocks[currentNum], pos, rot);
                        holdObject = Instantiate(blocks[holdNumber], pos + new Vector3(-5, -2, 0), rot);

                        holdObject.tag = "not";
                        firstObject.tag = "not";
                        secondObject.tag = "not";
                        thirdObject.tag = "not";

                        child1 = controlledObject.transform.GetChild(0).gameObject;
                        child2 = controlledObject.transform.GetChild(1).gameObject;
                        child3 = controlledObject.transform.GetChild(2).gameObject;
                        child4 = controlledObject.transform.GetChild(3).gameObject;

                        raycastChild1down = Physics2D.Raycast(child1.transform.position, Vector2.down, 0.5f);
                        raycastChild2down = Physics2D.Raycast(child2.transform.position, Vector2.down, 0.5f);
                        raycastChild3down = Physics2D.Raycast(child3.transform.position, Vector2.down, 0.5f);
                        raycastChild4down = Physics2D.Raycast(child4.transform.position, Vector2.down, 0.5f);

                        raycastChild1left = Physics2D.Raycast(child1.transform.position, Vector2.left, 0.5f);
                        raycastChild2left = Physics2D.Raycast(child2.transform.position, Vector2.left, 0.5f);
                        raycastChild3left = Physics2D.Raycast(child3.transform.position, Vector2.left, 0.5f);
                        raycastChild4left = Physics2D.Raycast(child4.transform.position, Vector2.left, 0.5f);

                        raycastChild1right = Physics2D.Raycast(child1.transform.position, Vector2.right, 0.5f);
                        raycastChild2right = Physics2D.Raycast(child2.transform.position, Vector2.right, 0.5f);
                        raycastChild3right = Physics2D.Raycast(child3.transform.position, Vector2.right, 0.5f);
                        raycastChild4right = Physics2D.Raycast(child4.transform.position, Vector2.right, 0.5f);

                        loseCollider = Physics2D.Raycast(new Vector3(-2.26f, 3.87f, 136.8307f), Vector2.right, 100f);

                        alreadyHeld = true;
                    }
                }
            }
            if (loseCollider.collider != null)
            {
                GameEnd += 1;
            }
        }
        
        else if (GameEnd == 1)
        {
            BlocksObjects = GameObject.FindGameObjectsWithTag("Block");
            for (int i = 0; i < BlocksObjects.Length; i++)
            {
                Destroy(BlocksObjects[i]);
            }
            loseBackground.SetActive(true);
            gameOver.SetActive(true);
            finalScore.SetActive(true);
            finalScoreNumber.SetActive(true);
            finalScoreNumber.GetComponent<TextMeshProUGUI>().text = "" + gameScore;
            GameEnd += 1;
        }

        else 
        {
            
        }
    }
}
