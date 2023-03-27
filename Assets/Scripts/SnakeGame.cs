using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SnakeGame : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float speed = 1f;

    [SerializeField]
    public int score = 0;

    [SerializeField]
    public int deathCount = 0;
    
    public GameObject foodPrefab;
    public GameObject talePrefab;
    public GameObject wallPrefab;

    private readonly float size = 1;
    private readonly int startCount = 5;

    List<GameObject> snakeTales = new List<GameObject>();
    List<GameObject> foodContainter = new List<GameObject>();

    private Vector2 direction = Vector2.right;

    public TMPro.TextMeshProUGUI ScoreUI;
    public GameObject GameOverUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GameStart()
    {
        Time.timeScale = 1f;
        CreateWall();
        Reset();
    }

    private void Reset()
    {
        CancelInvoke("Move");

        direction = Vector2.right;
        score = 0;
        CreateSnake();
        foreach(GameObject go in foodContainter)
        {
            Destroy(go);
        }
        foodContainter.Clear();
        CreateFood();

        InvokeRepeating("Move", 0.3f, speed);
    }

    
    void CreateSnake()
    {
        foreach(GameObject go in snakeTales)
        {
            Destroy(go);
        }
        snakeTales.Clear();

        Vector3 startPos = new Vector3(10, 10, 0);
        for (int i = 0; i < startCount; i++)
        {
            GameObject tale = GameObject.Instantiate(talePrefab);
            tale.transform.position = startPos - new Vector3(i * size, 0, 0);
            snakeTales.Add(tale);
        }

        snakeTales[0].gameObject.name = "Head";
        Debug.Log(snakeTales[0].transform.position);
    }

    void CreateFood()
    {
        float xPos = Random.Range(1, width-1);
        float yPos = Random.Range(1, height-1);
        Vector3 foodPos = new Vector3(xPos, yPos, 0);

        GameObject food = GameObject.Instantiate(foodPrefab);
        food.transform.position = foodPos;

        foodContainter.Add(food);
    }


    void CreateWall()
    {
        ///けけけけけけけけけけ
        ///け                け
        ///け                け 
        ///け                け 
        ///け                け 
        ///け                け 
        ///けけけけけけけけけけ

        for (int i = 0; i < width; i++)
        {
            GameObject top = GameObject.Instantiate(wallPrefab);
            float xpos = i * size;
            top.transform.position = new Vector3(xpos, 0, 0);

            // bottom
            GameObject bottom = GameObject.Instantiate(wallPrefab);
            bottom.transform.position = new Vector3(xpos, (height * size), 0);
        }

        for (int i = 0; i <= height; i++)
        {
            GameObject leftWall = GameObject.Instantiate(wallPrefab);
            GameObject rightWall = GameObject.Instantiate(wallPrefab);
            float yPos = i;

            // leftWall
            leftWall.transform.position = new Vector3(0, yPos, 0);
            // rightWall
            float xPos = width * size;
            rightWall.transform.position = new Vector3(xPos, yPos, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down)
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up)
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right)
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }

    private void EatFood()
    {
        if(foodContainter != null)
        {
            for(int i = 0; i< foodContainter.Count; i++)
            {
                if (foodContainter[i].transform.position.x == snakeTales[0].transform.position.x &&
                    foodContainter[i].transform.position.y == snakeTales[0].transform.position.y)
                {
                    Destroy(foodContainter[i]);
                    foodContainter.RemoveAt(i);
                    CreateFood();
                    AddTail();
                    score++;
                    Debug.Log("food eat");
                }
            }
        }
    }

    private void AddTail()
    {
        GameObject tale = GameObject.Instantiate(talePrefab);
        snakeTales.Add(tale);
    }
    private void Move()
    {
        for (int i = snakeTales.Count-1 ; i > 0; i--)
        {
            if (i == 0) continue;
            snakeTales[i].transform.position = snakeTales[i - 1].transform.position;
        }
        snakeTales[0].transform.Translate(direction);

        EatFood();

        if (CheckHitHead())
        {
            Debug.Log("GameOver\n Score : " + score);
            ScoreUI.text = "Score : " + score;

            Time.timeScale = 0;

            GameOverUI.SetActive(true);

            //Reset();
        }
        
    }

    private bool CheckHitHead()
    {
        Transform head = snakeTales[0].transform;
        bool isOver = false;
          
        if (head.transform.position.x == width ||
            head.transform.position.y == height ||
            head.transform.position.x == 0 ||
            head.transform.position.y == 0)
        {
            isOver = true;
            Debug.Log("wall hit");
        }

        for(int i = 3; i < snakeTales.Count; i++)
        {
            if (snakeTales[i].transform.position.x == head.transform.position.x &&
                snakeTales[i].transform.position.y == head.transform.position.y)
            {
                isOver = true;
                Debug.Log("tale hit");
                string error = string.Format("index : {0}, position {1}", i, head.transform.position.ToString());
                Debug.Log(error);
            }
        }
        return isOver;
    }
}
