using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class Blockable : MonoBehaviour
{
    [Header("Highlight")]
    [SerializeField] private Color highlightColor = Color.green;

    private Renderer _renderer;
    private Color originColor;
    private bool isInRange = false;

    [Header("BlockLogic")]
    private UnityEngine.AI.NavMeshObstacle _navemob;
    private bool isBlocked = false;

    [Header("DoorAnimation")]
    [SerializeField] private Transform doorAn;
    private float open = 90f;
    private float close = 0f;
    private float rotateSpeed = 4f;
    private bool IsBlocked = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _navemob = GetComponent<UnityEngine.AI.NavMeshObstacle>();

        if (_renderer != null) originColor = _renderer.material.color;
        Vector3 r = doorAn.eulerAngles;
        r.y = 90f;
        doorAn.eulerAngles = r;
        _navemob.enabled = false;
    }

    public void SetHighlight(bool state)
    {
        if (isInRange == state) return;
        isInRange = state;

        if (_renderer != null)
        {
            _renderer.material.color = state ? highlightColor : originColor;
        }
    }

    public void OnBlock()
    {
        if(isBlocked)
        {
            DoorOpen();
        }
        else
        {
            DoorClose();
        }
        

    }

    private void DoorOpen()
    {
        isBlocked = false;
        _navemob.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Blockable");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOpen();
        }
        StartCoroutine(OpenDoorAnim());
        GridManager.Instance.GenerateGrid();
    }

    private void DoorClose()
    {
        isBlocked = true;
        Debug.Log("Block!");
        _navemob.enabled = true;
        _navemob.carving = true;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClose();
        }
        StartCoroutine(CloseDoorAnim());
        GridManager.Instance.GenerateGrid();
    }

    private IEnumerator OpenDoorAnim()
{
    Quaternion beginR = doorAn.rotation;

    Quaternion endR =
        Quaternion.Euler(doorAn.eulerAngles.x, open, doorAn.eulerAngles.z);

    float speed = 0f;

    while (speed < 1f)
    {
        speed += rotateSpeed * Time.deltaTime;
        doorAn.rotation = Quaternion.Lerp(beginR, endR, speed);
        yield return null;
    }

    doorAn.rotation = endR;
}

    private IEnumerator CloseDoorAnim()
    {
        Quaternion beginR = doorAn.rotation;

        Quaternion endR =
            Quaternion.Euler(doorAn.eulerAngles.x, close, doorAn.eulerAngles.z);
        float speed = 0f;
        while(speed < 1f)
        {
            speed += rotateSpeed * Time.deltaTime;

            doorAn.rotation = Quaternion.Lerp(beginR, endR, speed);
            yield return null;
        }

        doorAn.rotation = endR;
    }
}
