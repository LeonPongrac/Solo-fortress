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

    private int target;
    void Awake()
    {
        hp = _MAXHP;
    }
    private void Update()
    {
        target = GameObject.Find("Main Camera").GetComponent<Setup>().target;
        if (hp > _MAXHP) hp = _MAXHP;
    }

    public virtual void Ability_Basic()
    {
        GameObject.Find("Main Camera").GetComponent<Setup>().players[target].GetComponent<PlayerScript>().hp--;         //nisam trijezan dok ovo pisem, ak neko moze malo bolje / ljepse slobodno
    }

    public virtual void Ability_1()
    {
        ++hp;
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
