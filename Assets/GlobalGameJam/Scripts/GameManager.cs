using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public EnemyManager EnemyManager;
    [SerializeField] private Player playerPrototype;
    public List<Player> players;
    public SceneSwitcher SceneSwitcher;
    public SettingsConfig Settings;

    public InputActionAsset map;
    public event Action<int> OnJoined;

    private void Awake()
    { 

        Instance = this;
        SceneSwitcher = new SceneSwitcher();
        SceneSwitcher.LoadScene(Scenes.Menu);
        for (var i = 0; i < Settings.MaxPlayers; i++)
        {
            var player = Instantiate(playerPrototype);
            player.GetComponent<PlayerInput>().actions = map;
            players.Add(player);
            player.OnJoined += JoinPlayer;
        }
    }

 

    public void JoinPlayer(object sender, Player player)
    {
        Debug.Log($"{player} joined. Update the UI.");
        player.OnJoined -= JoinPlayer;
        player.OnLeave += LeavePlayer;
    }

    public void LeavePlayer(object sender, Player player)
    {
        Debug.Log($"{player} left. Update the UI.");
        player.OnLeave -= LeavePlayer;
        player.OnJoined += JoinPlayer;
    }


}