using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings
{
	public int currentLevel;
}

public class DataManager : MonoBehaviour
{
    static int BestScore = -1;
    static int Gems = -1;

    static LevelSettings LevelSettings = new LevelSettings();


    public static bool firstTimeOpened = false;

    public void Awake()
    {

        BestScore = InitInteger("BestScore", BestScore);
        Gems = InitInteger("Gems", Gems);

        LevelSettings = InitLevelSettings();
    }


    //BestScore
    public static void SetBestScore(int NewBestScore)
    {
        BestScore = NewBestScore;
        PlayerPrefs.SetInt("BestScore", BestScore);
    }

    public static int GetBestScore()
    {
        return BestScore;
    }

    //Coins
    public static void SetGems(int NewGems)
    {
        Gems = NewGems;
        PlayerPrefs.SetInt("Gems", Gems);
    }

    public static int GetGems()
    {
        return Gems;
    }

    // UpgradeSettings
    public static void SetLevelSettings(LevelSettings NewLevelSettings)
    {
        string json = JsonUtility.ToJson(NewLevelSettings);
        PlayerPrefs.SetString("LevelSettings", json);
        LevelSettings = NewLevelSettings;
    }

    public static LevelSettings GetLevelSettings()
    {
        return LevelSettings;
    }


    private int InitInteger(string Name, int CurrentValue)
    {
        if (CurrentValue == -1)
        {
            CurrentValue = PlayerPrefs.GetInt(Name);
            if (CurrentValue == null)
            {
                CurrentValue = 0;
                PlayerPrefs.SetInt(Name, CurrentValue);
            }
        }
        return CurrentValue;
    }



    private LevelSettings InitLevelSettings()
    {
        string json = PlayerPrefs.GetString("LevelSettings");

        if (json == null || json == string.Empty)
        {
            json = JsonUtility.ToJson(LevelSettings);

            PlayerPrefs.SetString("LevelSettings", json);
        }
        else
        {
            LevelSettings = JsonUtility.FromJson<LevelSettings>(json);
        }

        json = JsonUtility.ToJson(LevelSettings);
        PlayerPrefs.SetString("LevelSettings", json);

        return LevelSettings;
    }



}
