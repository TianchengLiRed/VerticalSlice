using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeHover : MonoBehaviour
{

    [SerializeField] private Renderer rend;
    [SerializeField] private Color normalColor = new Color(0f, 0.6f, 1f, 0.35f);
    [SerializeField] private Color hoverColor = new Color(0f, 1f, 0f, 0.6f);
    private Material mat;
    private bool isHovering;
    [SerializeField] private LayerMask playerMask;
    // Start is called before the first frame update
   void Awake()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (rend != null)
        {
            mat = rend.material;
            mat.color = normalColor;
        }
    }

    void Update()
    {
        RayCheckHover();
    }

    private void RayCheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100f, playerMask))
        {
            if (hit.collider.gameObject == gameObject)
        
            {
                if (!isHovering)
                {
                    isHovering = true;
                    mat.color = hoverColor;
                }
            }
            else
            {
                if (isHovering)
                {
                    isHovering = false;
                    mat.color = normalColor;
                }
            }
        

        }
        

    }
}
