using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class APIandroid
{

    // Piter Amstrong

    private PlayerStats playerStats;



    public PlayerStats getPlayerStats()
    {
#if UNITY_ANDROID

        String stringAndroid = null;

        try
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("com.dsaproject.piterarmstrong_android.APIUnity");
            stringAndroid = javaClass.CallStatic<String>("getPlayerStats");   //funcion de nuestro android
            Debug.Log("Stats: ");
            Debug.Log(stringAndroid);
        }
        catch (Exception ex)
        {
            Debug.Log("Error Unity, method getPlayerStats");
            Debug.Log(ex);
        }

        if (stringAndroid == null)
            stringAndroid = "1,1";

#else
        string stringAndroid = "1,1";
#endif

        string[] playerStatsVector = stringAndroid.Split(',');

        PlayerStats player = new PlayerStats(Int32.Parse(playerStatsVector[0]), Int32.Parse(playerStatsVector[1])
            );

        this.playerStats = player;
        return this.playerStats;
    }

    public void setPlayerStats(PlayerStats stats)
    {
        this.playerStats = stats;
    }



    public void sendPlayerStats(PlayerStats playerStats)
    {
        string stringUnity = "";

        stringUnity += playerStats.getLevel() + ",";
        stringUnity += playerStats.getLife() + ",";

        stringUnity = stringUnity.TrimEnd(',');

#if UNITY_ANDROID
        try
        {
            AndroidJavaClass javaClass = new AndroidJavaClass("com.dsaproject.piterarmstrong_android.APIUnity");
            javaClass.CallStatic("sendStats", stringUnity);     //funcion de nuestro andro
            Debug.Log("stringStats: ");
            Debug.Log(stringUnity);

        }
        catch (Exception ex)
        {
            Debug.Log("Error Unity, method getPlayerStats");
            Debug.Log(ex);
        }
#endif
    }



}

