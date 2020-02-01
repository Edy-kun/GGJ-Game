using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public EnemyManager EnemyManager;
    public List<Player> players;
    public SceneSwitcher SceneSwitcher;
    public SettingsConfig Settings;

    private void Awake()
    {
        Instance = this;
        SceneSwitcher = new SceneSwitcher();
        
        SceneSwitcher.LoadScene(Scenes.Menu);
    }


}

public static class Scenes
{
    public static int 
        Menu = 1,
        Game = 2;

}