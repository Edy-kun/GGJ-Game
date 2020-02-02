using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public RandomEnemyPlacement _spawner;
    public PickupSpawner _PickupSpawner;
    [SerializeField] private Player playerPrototype;
    public List<Player> players;
    public SceneSwitcher SceneSwitcher;
    public SettingsConfig Settings;

    public InputActionAsset map;
    private Boat _boat;
    public event Action<int> OnJoined;

    private void Awake()
    {

        Instance = this;
        SceneSwitcher = new SceneSwitcher();
        SceneSwitcher.LoadScene(Scenes.Menu);

     

        SceneManager.sceneLoaded += (arg0, mode) =>
        {
            _boat = FindObjectOfType<Boat>();
            _spawner = FindObjectOfType<RandomEnemyPlacement>();
            if(!_boat)
                return;
            for (var i = 0; i < Settings.MaxPlayers; i++)
            {
                var player = Instantiate(playerPrototype,_boat.transform);
                player.GetComponent<PlayerInput>().actions = map;
                players.Add(player);
                player.character.SetActive(false);
                player.OnJoined += JoinPlayer;
            }

            players.ForEach(p => p.Boat = _boat);
        
    };
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