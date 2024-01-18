/*
 * po jedna skripta za svakog igraca,
 * svaki igrac moze gledat svoj hp, attack power, status effects, abilities i sta cemo sve dodat
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int _NUMBER;
    public int _MAXHP = 20;
    public int hp;

    public int ability_block = 0;

    private SpriteRenderer spriteRenderer;
    public PlayerColors color;

    private int target;
    void Awake()
    {
        hp = _MAXHP;

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        target = GameObject.Find("Main Camera").GetComponent<Setup>().target;
        if (hp > _MAXHP) hp = _MAXHP;

    }

    // Postavlja sprite na normal ovisno o hp
    public void SetNormalSprite()
    {
        float percentage = (float)hp / (float)_MAXHP;
        string health = "3_health";
        if (percentage <= (2f / 3f) && percentage > (1f/3f))
        {
            health = "2_health";
        }
        if (percentage <= (1f / 3f) && percentage > 0f)
        {
            health = "1_health";
        }
        if (hp <= 0)
        {
            health = "0_health";
        }
        string spriteName = color.ToString() + "_Rook(" + health + ")";
        string path = "Sprites/All_Rook_Sprites/Rook_(" + health + ")/" + spriteName;
        Sprite newSprite = Resources.Load<Sprite>(path);
        if (newSprite != null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newSprite;
            }          
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + path);
        }
    }

     // Postavlja sprite na target ovisno o hp
    public void SetTargetSprite()
    {
        float percentage = (float)hp / (float)_MAXHP;
        string health = "3_health";
        if (percentage <= (2f / 3f) && percentage > (1f / 3f))
        {
            health = "2_health";
        }
        if (percentage <= (1f / 3f) && percentage > 0f)
        {
            health = "1_health";
        }
        string spriteName = color.ToString() + "_Rook_targeted(" + health + ")";
        string path = "Sprites/All_Rook_Sprites/Rook_(" + health + ")/" + spriteName;
        Sprite newSprite = Resources.Load<Sprite>(path);
        if (newSprite != null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newSprite;
            }
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + path);
        }
    }

    // Postavlja sprite na turn ovisno o hp 

    public void SetTurnSprite()
    {
        float percentage = (float)hp / (float)_MAXHP;
        string health = "3_health";
        if (percentage <= (2f / 3f) && percentage > (1f / 3f))
        {
            health = "2_health";
        }
        if (percentage <= (1f / 3f) && percentage > 0f)
        {
            health = "1_health";
        }
        string spriteName = color.ToString() + "_Rook_turn(" + health + ")";
        string path = "Sprites/All_Rook_Sprites/Rook_(" + health + ")/" + spriteName;
        Sprite newSprite = Resources.Load<Sprite>(path);
        if (newSprite != null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = newSprite;
            }
        }
        else
        {
            Debug.LogError("Sprite not found at path: " + path);
        }
    }

    public void UpdateHealthBar()
    {
        // get gameobject that is child of this gameobject with name HealthBar, which is an Image type filed horizontal, set its fill amount to hp/maxhp
        GameObject healthBar = GameObject.Find("Player "+ _NUMBER).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        healthBar.GetComponent<UnityEngine.UI.Image>().fillAmount = (float)hp / (float)_MAXHP;
    }

    public virtual void Ability_Basic()
    {
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().hp--;         //nisam trijezan dok ovo pisem, ak neko moze malo bolje / ljepse slobodno **  ma bit ce 
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().UpdateHealthBar();
    }


    public virtual void Ability_1()
    {
        ++hp;
        UpdateHealthBar();
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().hp++;
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().UpdateHealthBar();
    }

    public virtual void Ability_2()
    {
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().ability_block++;
    }

    public void turn_end()
    {
        if(ability_block > 0) ability_block--;
    }

}
