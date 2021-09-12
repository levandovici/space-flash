using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int bestScore;

    public int coins;

    public bool[] rockets;

    public int selectedRocketID;

    public int SelectedDriveID;

    public int age;

    public int year_of_age_setup;


    public float musicVolume;

    public float sfxVolume;

    public SystemLanguage language;



    public PlayerData()
    {
        this.bestScore = 0;
        this.coins = 0;
        this.rockets = Rockets();
        this.musicVolume = 0.5f;
        this.sfxVolume = 0.5f;
        this.selectedRocketID = 0;
        this.SelectedDriveID = 0;
        this.language = SystemLanguage.English;

        this.year_of_age_setup = System.DateTime.Now.Year;
        this.age = -1;
    }


    private static bool[] Rockets()
    {
        return new bool[] { true, false, false };
    }

    public static int[] RocketsPrice()
    {
        return new int[] { 0, 10000, 100000};
    }

    public static int RocketsCount()
    {
        return Rockets().Length;
    }
}
