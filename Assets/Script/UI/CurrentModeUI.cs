using TMPro;
using UnityEngine;

public class CurrentModeUI : MonoBehaviour
{
    [SerializeField] private AgentController player;
    [SerializeField] private TextMeshProUGUI modeText;
    
    private void Start()
    {
        LevelSpawn.OnPlayerSpawned += SetPlayer;
    }

    private void SetPlayer(PlayerHealth health)
    {
        player = health.GetComponent<AgentController>();
    }

    private void Update()
    {
        if(player == null) return;
        modeText.text = "Mode: " + player.state.ToString();
    }
}
