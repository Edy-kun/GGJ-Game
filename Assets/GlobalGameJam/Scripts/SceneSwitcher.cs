using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher
{
    private int _activeScene = -1;
    public void LoadScene(int Scene)
    {
        if (_activeScene > 0)
        {
            SceneManager.UnloadSceneAsync(_activeScene);
        }
        else
        {
            
            if (SceneManager.sceneCount == 2)
            {
                Debug.Log( "NOT LOADING TESTING" );
                _activeScene = SceneManager.GetSceneAt(1).buildIndex;
                return;
            }
            
        }

        SceneManager.LoadScene(Scene, LoadSceneMode.Additive);
   
    }
    
}