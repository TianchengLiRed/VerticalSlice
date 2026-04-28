using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startGame;
    [SerializeField] private Button quitGame;
    [SerializeField] private Image background1;
    [SerializeField] private TextMeshProUGUI remindText;
    // Start is called before the first frame update
    void Start()
    {
        startGame.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        background1.gameObject.SetActive(true);
        remindText.gameObject.SetActive(true);

        startGame.onClick.AddListener(StartGameOnClicked);
        quitGame.onClick.AddListener(QuitGameOnClicked);
    }
    void Update()
    {
        PressAny();
    }
    private void PressAny()
    {
        if (Input.anyKeyDown)
        {
            MainMenuDisplay();
            return;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            MainMenuDisplay();
            return;
        }
    }
    private void MainMenuDisplay()
    {
        startGame.gameObject.SetActive(true);
        quitGame.gameObject.SetActive(true);
        background1.gameObject.SetActive(false);
    }

    private void StartGameOnClicked()
    {
        SceneManager.LoadScene("Station");
    }

    private void QuitGameOnClicked()
    {
        Application.Quit();
    }


}
