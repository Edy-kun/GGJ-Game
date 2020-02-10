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
    public Boat _boat;
    public HoverCraftUI _ui;
    public Team _team;
    public event Action<int> OnJoined;

    private void Awake()
    {

        Instance = this;
        
        /*SceneSwitcher = new SceneSwitcher();
        SceneSwitcher.LoadScene(Scenes.Game);*/

/*

        SceneManager.sceneLoaded += (arg0, mode) =>
        {

            _ui = FindObjectOfType<HoverCraftUI>();
            if (_ui == null)
                Debug.Log("NO UI");
            _boat = FindObjectOfType<Boat>();
            _team = new Team()
            {
                Boat = _boat,
                Score = 0
            };

         
           // _spawner = FindObjectOfType<RandomEnemyPlacement>();
            if (!_boat)
                return;
            for (var i = 0; i < 2; i++)
            {
                var player = Instantiate(playerPrototype, _boat.transform);
                player.GetComponent<PlayerInput>().actions = map;

                players.Add(player);
                player.character.SetActive(false);
                player.OnJoined += JoinPlayer;
            }

            _team.Players = players;
            players.ForEach(p => p.Boat = _boat);

        };
        */
    }

    private void HandleInventoryUI(Inventory obj)
    {
     _ui.OnInventoryChanged(obj);
    }

    private void HandleUI(int obj)
    {
        _ui.OnScoreChange(obj);
    }
    /*


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
    */


}