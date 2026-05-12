using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private Image blackPanel;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] Button nextButton;
    private bool clicked;
    [SerializeField] private DialogueData introDialogue;
    [SerializeField] private float typeSpeed = 0.04f;

    private void Start()
    {
        blackPanel.gameObject.SetActive(true);
        introText.text = "";
        StartCoroutine(PlayCutScene());
    }

    private IEnumerator PlayCutScene()
    {
        yield return new WaitForSeconds(1f);

        foreach (DialogueLine line in introDialogue.lines)
        {
            yield return StartCoroutine(TypeText(line.content));

            clicked = false;
            yield return new WaitUntil(() => clicked);
        }
        introText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("station");
    }

    private IEnumerator TypeText(string content)
    {
        introText.text = "";

        foreach (char c in content)
        {
            introText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    public void OnClick()
    {
        clicked = true;
    }
}
