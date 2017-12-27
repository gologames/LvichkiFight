using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public static class StartFight
{
    //Skins range from 0 to 2
    //Inf_MaxCount = 300
    //Cav_MaxCount = 216
    //Art_MaxCount = 100

    public const string LeftSkinKey = "LeftSkin";
    public const string RightSkinKey = "RightSkin";
    public const string LeftInfKey = "LeftInf";
    public const string LeftCavKey = "LeftCav";
    public const string LeftArtKey = "LeftArt";
    public const string RightInfKey = "RightInf";
    public const string RightCavKey = "RightCav";
    public const string RightArtKey = "RightArt";

    public static void LoadFightScene(
        int fightSceneNum,
        int leftSkin, int rightSkin,
        int leftInf, int leftCav, int leftArt,
        int rightInf, int rightCav, int rightArt)
    {
        PlayerPrefs.SetInt(LeftSkinKey, leftSkin);
        PlayerPrefs.SetInt(RightSkinKey, rightSkin);
        PlayerPrefs.SetInt(LeftInfKey, leftInf);
        PlayerPrefs.SetInt(LeftCavKey, leftCav);
        PlayerPrefs.SetInt(LeftArtKey, leftArt);
        PlayerPrefs.SetInt(RightInfKey, rightInf);
        PlayerPrefs.SetInt(RightCavKey, rightCav);
        PlayerPrefs.SetInt(RightArtKey, rightArt);
        SceneManager.LoadScene(fightSceneNum);
    }
}
