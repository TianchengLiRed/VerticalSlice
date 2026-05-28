using System.Collections;
using UnityEngine;

public class GhostAttacked : MonoBehaviour
{
    public static GhostAttacked Instance;

    [SerializeField] private Renderer ghostRenderer;
    [SerializeField] private Color attackedColor = Color.red;
    [SerializeField] private float flashTime = 0.15f;

    private Material mat;
    private Color originalColor;

    // Shader Graph 里的 Fresnel Color Reference
    private readonly string colorProperty = "_Fresnel_Color";
    private readonly string colorProperty2 = "_Fresnel_Color2";

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (ghostRenderer != null)
        {
            mat = ghostRenderer.material;

            // 获取原本颜色
            if (mat.HasProperty(colorProperty))
            {
                originalColor = mat.GetColor(colorProperty);
            }
            else
            {
                Debug.LogError("Shader does not contain property: " + colorProperty);
            }
        }
    }

    public void GhostAttackedEffect()
    {
        if (mat != null)
        {
            StartCoroutine(DamageFlash());
        }
    }

    IEnumerator DamageFlash()
    {
        mat.SetColor(colorProperty, attackedColor);
        mat.SetColor(colorProperty2, attackedColor);

        yield return new WaitForSeconds(flashTime);

        mat.SetColor(colorProperty, originalColor);
        mat.SetColor(colorProperty2, originalColor);
    }
}