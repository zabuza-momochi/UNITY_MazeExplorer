using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMngr : Singleton<UIMngr>
{
    public List<Texture> ItemList;

    public RawImage ItemBox;
    public RawImage PowerUpBox;

    public TextMeshProUGUI Stage;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Timer;

    public Image PanelGameOver;
    public Image PanelStageComplete;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ShowInInventory(Item item)
    {
        if (item == Item.Key)
        {
            ItemBox.texture = ItemList[(int)item];

            ItemBox.enabled = true;
        }

        if (item == Item.PowerUpSpeed)
        {
            PowerUpBox.texture = ItemList[(int)item];

            PowerUpBox.enabled = true;

        }
        if (item == Item.PowerUpShield)
        {
            PowerUpBox.texture = ItemList[(int)item];

            PowerUpBox.enabled = true;

        }
    }

    public void HideInInventory(Item item)
    {
        if (item == Item.Key)
        {
            ItemBox.texture = null;

            ItemBox.enabled = false;
        }

        if (item == Item.PowerUpSpeed)
        {
            PowerUpBox.texture = null;

            PowerUpBox.enabled = false;
        }

        if (item == Item.PowerUpShield)
        {
            PowerUpBox.texture = null;

            PowerUpBox.enabled = false;
        }
    }

    public void Restart()
    {
        ItemBox.enabled = false;

        PowerUpBox.enabled = false;
    }
}

public enum Item
{
    Key, PowerUpSpeed, PowerUpShield,LAST
}
