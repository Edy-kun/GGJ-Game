using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerPanel : UIPanel
{
    public PlayerSelectPanel playerSelectPanelPrototype;
    public List<PlayerSelectPanel> PlayerSelectPanels = new List<PlayerSelectPanel>();
    public TMP_Text EmptyLabel;
    public Transform Container;

    private void Awake()
    {
        playerSelectPanelPrototype = GetComponentInChildren<PlayerSelectPanel>();
        playerSelectPanelPrototype.gameObject.SetActive(false);
        EmptyLabel.text = "Press Start To join!";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnPlayerJoined(PlayerSelectPanels.Count);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnPlayerLeft(PlayerSelectPanels.Count - 1);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            HandleStart();
        }

        if (PlayersReady())
        {
            HandleStart();
        }
    }

    private bool PlayersReady()
    {
        return PlayerSelectPanels.Count > 0 && PlayerSelectPanels.TrueForAll(item => item.Ready);
    }

    public void HandleStart()
    {
        GameManager.Instance.SceneSwitcher.LoadScene(Scenes.Game);
    }


    public void OnPlayerLeft(int id)
    {
        var obj = PlayerSelectPanels.FirstOrDefault(item => item.Id == id);
        if (obj == null)
            return;
        
        PlayerSelectPanels.Remove(obj);
        Destroy(obj.gameObject);
        EmptyLabel.gameObject.SetActive(PlayerSelectPanels.Count == 0);
    }

    public void OnPlayerJoined(int id)
    {
        if (PlayerSelectPanels.Count >= GameManager.Instance.Settings.MaxPlayers)
            return;
        var p = Instantiate(playerSelectPanelPrototype, Container);
        p.SetId(id);
        PlayerSelectPanels.Add(p);
        EmptyLabel.gameObject.SetActive(false);
      


    }
}
