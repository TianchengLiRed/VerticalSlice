using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public event Action<int> OnTurnStarted;

    [Header("回合基本逻辑")]
    private float turnTime = 5f;
    private float timer;
    private int currentRound = 0;
    private bool playerActed = false;

    public float TimerPercent => timer / turnTime;
    public float CurrentTimer => timer;
    //实例使用
    void Awake()
    {
        Instance = this;
    }
    
    //开始计时,后期可以放到真正开始游戏后
    void Start()
    {
        StartTurn();
    }

    private void Update()
    {
        //倒计时
        Timer();
    }
    void StartTurn()
    {
        //开始计时
        timer = turnTime;
        playerActed = false;
        OnTurnStarted?.Invoke(currentRound);
    }

    void EndTurn()
    {
        //Turn结束重新开始计时
        currentRound++;
        StartTurn();
    }

    public void PlayerFinishedAction()
    {
        //玩家行动后自动下一个回合
        playerActed = true;
        EndTurn();
    }
    void Timer()
    {
        //倒计时
        timer -= Time.deltaTime;

        if (timer <= 0f && !playerActed)
        {
            //到点了自动下一个回合
            Debug.Log("Time up! Auto next round.");
            EndTurn();
        }
    }
}
