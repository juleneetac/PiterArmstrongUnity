using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Piter Amstrong

    private int level;
    private int life;


    public PlayerStats(int level, int life)
    {
        this.level = level;
        this.life = life;

    }

    public int getLevel()
    {
        return this.level;
    }

    public void setLevel(int level)
    {
        this.level = level;
    }




    public int getLife()
    {
        return this.life;
    }

    public void setLife(int life)
    {
        this.life = life;
    }
}