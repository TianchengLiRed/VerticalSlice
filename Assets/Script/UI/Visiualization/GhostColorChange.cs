using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostColorChange : MonoBehaviour
{

    [SerializeField] private GhostHealth ghostHealth;
    [SerializeField] private Renderer ghostRenderer;

    private Material mat;

    private void Awake()
    {
        mat = ghostRenderer.material;
    }

    public void ShaderHealth(float currentHealth, float maxHealth)
    {
        float percent = currentHealth / maxHealth;

        mat.SetFloat("_HealthPercent", percent);
    }

}
