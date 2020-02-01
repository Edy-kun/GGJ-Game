
using TMPro;
using UnityEngine;

public class PlayerSelectPanel : UIPanel
{
    [SerializeField] private TMP_Text label;
    public int Id { get; set; }
    public bool Ready { get; set; }

    public void SetId(int id)
    {
        Id = id;
        label.text = $"P{id + 1}";
        this.gameObject.SetActive(true);
    }
}