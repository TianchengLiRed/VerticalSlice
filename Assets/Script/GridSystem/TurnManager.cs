using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public event Action<int> OnTurnStarted;
    public GameObject TimeBarUI;

    [Header("�غϻ����߼�")]
    private float turnTime = 5f;
    private float timer;
    public int currentRound = 0;
    private bool playerActed = false;
    private bool isStarted = false;

    public float CurrentTimer => timer;
    //ʵ��ʹ��
    void Awake()
    {
        Instance = this;
    }
    
    //��ʼ��ʱ,���ڿ��Էŵ�������ʼ��Ϸ��
    void Start()
    {
        currentRound = 0;
    }

    private void Update()
    {
        if (!isStarted) return;
        //����ʱ
        Timer();
    }
   public void StartTurn()
    {
        //��ʼ��ʱ
        timer = turnTime;
        playerActed = false;
        isStarted = true;
        OnTurnStarted?.Invoke(currentRound);
        CustomEvent.Trigger(TimeBarUI,"RoundUpdate");
        Debug.Log("Triggered!");
    }

    void EndTurn()
    {
        //Turn�������¿�ʼ��ʱ
        currentRound++;
        StartTurn();
    }

    public void PlayerFinishedAction()
    {
        if (!isStarted) return;
        //����ж����Զ���һ���غ�
        playerActed = true;
        EndTurn();
    }
    void Timer()
    {
        if (!isStarted) return;
        //����ʱ
        timer -= Time.deltaTime;

        if (timer <= 0f && !playerActed)
        {
            //�������Զ���һ���غ�
            Debug.Log("Time up! Auto next round.");
            EndTurn();
        }
    }

    public float GetTimerPercent()
   {
     return timer / turnTime;
   }
}
