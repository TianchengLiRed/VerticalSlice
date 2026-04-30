using System.Collections;
using System.Collections.Generic;
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
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _navemob = GetComponent<UnityEngine.AI.NavMeshObstacle>();

        if (_renderer != null) originColor = _renderer.material.color;
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
        Debug.Log("Block!");
        _navemob.enabled = true;
        _navemob.carving = true;

    }
}
