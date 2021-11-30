using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialShip : MonoBehaviour
{
    private const float MOVE_DUR = 2.0f;
    private Timer timer;
    private float moveDir = 1.0f;

    void Start()
    {
        timer = gameObject.AddComponent<Timer>();
        MoveSideA();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MoveSideA()
    {
        timer.Init(MOVE_DUR, MoveSideB);
    }

    private void MoveSideB()
    {
        timer.Init(MOVE_DUR, MoveSideA);
    }
}
