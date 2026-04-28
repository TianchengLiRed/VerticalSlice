using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVcrash : MonoBehaviour
{
    public Vector3 targetPosition; 
    public float speed = 20f;
    private bool isMoving = true;
    public static event Action OnGameStart;//动画结束事件
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CrashMove();
    }

    void CrashMove()
    {
        if (!isMoving) return;//如果moving结束,那么不再移动
        transform.position = Vector3.MoveTowards(
           transform.position,
           targetPosition,
           speed * Time.deltaTime
       );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            OnArrived();
        }
    }

    void OnArrived()
    {
        Debug.Log("RV arrived!");

        //音效
        //特效
        //动画
        //后面衔接
        OnGameStart?.Invoke();
    }
}
