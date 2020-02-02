using System;
using System.Collections.Generic;

public class Team
{ 
   
    private int _score;
    public int Score
    {
        get =>_score;
        set
        {
            if (value == _score) 
                return;
            _score = value;
            OnScoreChanged?.Invoke(_score);

        }
    }
    public event Action<int> OnScoreChanged;
    
    public Boat Boat;
    public List<Player> Players;
   
    
  
}