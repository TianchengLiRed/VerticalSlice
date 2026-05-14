using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorJumpButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("HauntedHouse");
    }
}
