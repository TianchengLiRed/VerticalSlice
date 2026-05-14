using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttacked : MonoBehaviour
{
    public static PlayerAttacked Instance;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color attackedColor = Color.red;
    [SerializeField] private float flashTime = 0.15f;
    // Start is called before the first frame update
    private Material mat;
    private Color originalColor;

    private void OnEnable()
    {
        LevelSpawn.OnPlayerSpawned += GetRender;
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetRender(PlayerHealth hp)
    {
        if (playerRenderer != null)
        {
            mat = playerRenderer.material;
            originalColor = mat.color;
        }
    }

    public void PlayerAttackedEffect()
    {
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        mat.color = attackedColor;

        yield return new WaitForSeconds(flashTime);

        mat.color = originalColor;
    }
}
