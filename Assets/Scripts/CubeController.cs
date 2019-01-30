using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public int myX, myY;
    GameController myGameController;
    public bool active = false;
    public bool nextcube = false;


    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if (!nextcube)
        {
            GameController.processClick(gameObject, myX, myY, gameObject.GetComponent<Renderer>().material.color, active);
        }
        
    }
}
