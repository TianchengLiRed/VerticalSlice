using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintUI : MonoBehaviour
{
   [SerializeField] private Button tutorialButton;
   [SerializeField] private GameObject tutorialSheet;

   void Start()
   {
    tutorialSheet.SetActive(false);

   }

   public void OnClick()
   {
    tutorialSheet.SetActive(true);
   }

   public void Close()
   {
    tutorialSheet.SetActive(false);
   }
}
