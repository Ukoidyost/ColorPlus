using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab;
    static float gameLength = 60.0f, turnLength = 2.0f;
    static int gridW = 8, gridH = 5;
    float timerLength = 1;
    int turns = 0;
    int score = 0;
    int rainbowPoints = 5;
    int samePoints = 10;
    public GameObject[,] grid;
    GameObject nextCube;
    public GameObject ScoreDisplay;
    public GameObject NextCubeDisplay;
    public GameObject TimeDisplay;
    Color[] randomColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };
    static Vector3 cubePosition;
    Vector3 nextCubePos = new Vector3 (-2.5f, 10.4f, 0);
    static GameObject activeCube = null;
    bool gameOver = false;


    void Start()
    {
        CreateGrid();
    }

    //create the grid
    public void CreateGrid()
    {
        grid = new GameObject[gridW, gridH];

        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                cubePosition = new Vector3(x * 2, y * 2, 0);
                grid[x, y] = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
                grid[x, y].GetComponent<CubeController>().myX = x;
                grid[x, y].GetComponent<CubeController>().myY = y;

            }
        }

    }

    //end the game
    void endTheGame(bool win)
    {
        //if player win
        if (win)
        {
            NextCubeDisplay.GetComponent<UnityEngine.UI.Text>().text = "You Win!";
            print("you win!");
        }
        //player lose
        else
        {
            NextCubeDisplay.GetComponent<UnityEngine.UI.Text>().text = "You Lose!";
            print("you lose!");
        }

        Destroy(nextCube);
        nextCube = null;

        for (int x = 0; x< gridW; x++)
        {
            for(int y = 0; y < gridH; y++)
            {
                grid[x, y].GetComponent<CubeController>().nextcube = true;
            }
        }
        gameOver = true;
    }


    void createNextCube()
    {

        nextCube = Instantiate(cubePrefab, nextCubePos, Quaternion.identity);
        nextCube.GetComponent<Renderer>().material.color = randomColors[Random.Range(0, randomColors.Length)];
        nextCube.GetComponent<CubeController>().nextcube = true;
    }

    GameObject pickWhiteCube (List<GameObject> whiteCubes)
    {
        //no white cubes in the row 
        if (whiteCubes.Count == 0)
        {
            return null;
        }
        //pick a random white cube

        return whiteCubes[Random.Range(0, whiteCubes.Count)];


    }

    GameObject FindOpenCube(int y)
    {
        List<GameObject> whiteCubes = new List<GameObject>();


        //count the number of white cubes
        for (int x = 0; x < gridW; x++)
        {
            if(grid[x,y].GetComponent<Renderer>().material.color == Color.white)
            {
                whiteCubes.Add(grid[x, y]);
            }
        }
        return pickWhiteCube(whiteCubes);
    }

    GameObject FindOpenCube()
    {
        List<GameObject> whiteCubes = new List<GameObject>();


        //count the number of white cubes
        for (int y = 0; y < gridH; y++)
        {
            for (int x = 0; x < gridW; x++)
            {
                if (grid[x, y].GetComponent<Renderer>().material.color == Color.white)
                {
                    whiteCubes.Add(grid[x, y]);
                }
            }
        }
        return pickWhiteCube(whiteCubes);
    }

    void setCubeColor(GameObject myCube, Color color)
    {
        //no open cube in that row, end the game
        if (myCube == null)
        {
            endTheGame(false);
        }
        else
        {
            //assign the next cube's color to this cube
            myCube.GetComponent<Renderer>().material.color = color;
            Destroy(nextCube);
            nextCube = null;
        }
    }

    void placeNextCube(int y)
    {
        GameObject whiteCube = FindOpenCube(y);

        setCubeColor(whiteCube, nextCube.GetComponent<Renderer>().material.color);
        
    }

    void addBlackCube()
    {
        //find a random white cube and turn it black
        //if that is impossible, lose the game
        GameObject whiteCube = FindOpenCube();
        //use a larger color value
        setCubeColor(whiteCube, Color.black);
    }


    //detect keyboard input
    void processKeyboardInput()
    {
        int numKeyPressed = 0;
        //if player press 1-5, and there is a next cube
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            numKeyPressed = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            numKeyPressed = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            numKeyPressed = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            numKeyPressed = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            numKeyPressed = 5;
        }

        //if we still have the next cube, put it in that row, 
        //and subtract 1 since the grid arry has a 0-based index. if that row is full, end the game
        if (nextCube != null && numKeyPressed != 0)
        {
            placeNextCube(numKeyPressed - 1);
        }
        
    }

    public static void processClick(GameObject clickedcube, int x, int y, Color cubeColor, bool active)
    {
        if (cubeColor != Color.white && cubeColor != Color.black)
        {
            //activate and deactivate the clicked cube
            if (active)
            {
                clickedcube.transform.localScale /= 2f;
                clickedcube.GetComponent<CubeController>().active = false;
                activeCube = null;
            }
            else
            {
                if (activeCube != null)
                {
                    activeCube.transform.localScale /= 2f;
                    activeCube.GetComponent<CubeController>().active = false;
                }
                clickedcube.transform.localScale *= 2f;
                clickedcube.GetComponent<CubeController>().active = true;
                activeCube = clickedcube;
            }
        }
        else if(cubeColor == Color.white && activeCube != null)
        {
            int xdiff = clickedcube.GetComponent<CubeController>().myX - activeCube.GetComponent<CubeController>().myX;
            int ydiff = clickedcube.GetComponent<CubeController>().myY - activeCube.GetComponent<CubeController>().myY;
            if (Mathf.Abs(ydiff) <= 1 && Mathf.Abs(xdiff) <= 1)
            {
                //set the clickedcube to be active
                clickedcube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
                clickedcube.GetComponent<CubeController>().active = true;
                clickedcube.transform.localScale *= 2f;

                //set the old cube to white and not active
                activeCube.GetComponent<Renderer>().material.color = Color.white;
                activeCube.GetComponent<CubeController>().active = false;
                activeCube.transform.localScale /= 2f;

                //define new active cube
                activeCube = clickedcube;
            }
        }
    }

    bool rainbowPlus (int x, int y)
    {
        Color a = grid[x, y].GetComponent<Renderer>().material.color;
        Color b = grid[x + 1, y].GetComponent<Renderer>().material.color;
        Color c = grid[x - 1, y].GetComponent<Renderer>().material.color;
        Color d = grid[x, y + 1].GetComponent<Renderer>().material.color;
        Color e = grid[x, y - 1].GetComponent<Renderer>().material.color;

        //make sure colors are not black and white
        if (a == Color.white || a == Color.black ||
            b == Color.white || b == Color.black ||
            c == Color.white || c == Color.black ||
            d == Color.white || d == Color.black ||
            e == Color.white || e == Color.black)
        {
            return false;
        }
        
        //make sure all colors are different
        if (a != b && a != c && a != d && a != e &&
            b != c && b != d && b != e &&
            c != d && c != e &&
            d != e)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    bool sameColorPlus (int x, int y)
    {
        if (grid[x, y].GetComponent<Renderer>().material.color != Color.white &&
            grid[x, y].GetComponent<Renderer>().material.color != Color.black &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x + 1, y].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x - 1, y].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x, y + 1].GetComponent<Renderer>().material.color &&
            grid[x, y].GetComponent<Renderer>().material.color == grid[x, y - 1].GetComponent<Renderer>().material.color)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    void setPlusBlack (int x, int y)
    {
        if (x == 0 || y == 0 || x == gridW -1 || y == gridH -1)
        {
            return;
        }
        grid[x, y].GetComponent<Renderer>().material.color = Color.black;        
        grid[x+1, y].GetComponent<Renderer>().material.color = Color.black;       
        grid[x-1, y].GetComponent<Renderer>().material.color = Color.black;        
        grid[x, y+1].GetComponent<Renderer>().material.color = Color.black;        
        grid[x, y-1].GetComponent<Renderer>().material.color = Color.black;

        if (activeCube != null && activeCube.GetComponent<Renderer>().material.color == Color.black)
        {
            activeCube.GetComponent<CubeController>().active = false;
            activeCube.transform.localScale /= 2f;
        }
    }

    void Scoring()
    {
        for (int x = 1; x < gridW -1; x++)
        {
            for (int y = 1; y < gridH -1; y++)
            {
                if(rainbowPlus(x, y))
                {
                    score += rainbowPoints;
                    setPlusBlack(x,y);
                }
                if (sameColorPlus(x,y))
                {
                    score += samePoints;
                    setPlusBlack(x, y);
                }
            }
        }
        //check for rainbow plus and same color plus
    }
     void timing()
    {
        if (timerLength < Time.time)
        {
            float timeRemain = 60 - Time.time;
            int timer = (int)timeRemain;
            if (timer < 0)
            {
                timer = 0;
            }
            TimeDisplay.GetComponent<UnityEngine.UI.Text>().text = "Time Remain: " + timer;
            timerLength++;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        processKeyboardInput();
        Scoring();
        timing();




        if (Time.time < gameLength)
        {
            if (Time.time > turnLength * turns)
            {
                
                turns++;

                if (nextCube != null)
                {
                    score -= 1;
                    addBlackCube();
                }
                createNextCube();
            }
            if (score < 0)
            {
                score = 0;
            }
            ScoreDisplay.GetComponent<UnityEngine.UI.Text>().text = "Score: " + score;
        }
        else if (!gameOver)
        {
            if (score > 0)
            {
                endTheGame(true);
            }
            else
            {
                endTheGame(false);
            }
        }

        
    }
}
