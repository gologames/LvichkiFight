﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//здесь метод для запуска сцены с боем
public static class StartFight
{
    //Skins range from 0 to 2
    //Inf_MaxCount = 300
    //Cav_MaxCount = 216
    //Art_MaxCount = 100
    //WinKey = "Left" or "Right"

    //это ключи для PlayerPrefs
    public const string WinKey = "IsLeftWin";
    public const string LeftWinKey = "Left";
    public const string RightWinKey = "Right";
    public const string AfterSceneNumKey = "AfterSceneNum";
    public const string LeftSkinKey = "LeftSkin";
    public const string RightSkinKey = "RightSkin";
    public const string LeftInfKey = "LeftInf";
    public const string LeftCavKey = "LeftCav";
    public const string LeftArtKey = "LeftArt";
    public const string RightInfKey = "RightInf";
    public const string RightCavKey = "RightCav";
    public const string RightArtKey = "RightArt";

    //через этот статический метод запускать бой
    //fightSceneNum - номер в порядке билда сцены с боем
    //afterFightSceneNum - номер в порядке билда сцены
    //  которую нужно запустить после боя
    //leftSkin - номер скина левой армии
    //rightSkin - номер скина правой армии
    //leftInf - количество юнитов пехоты левой армии
    //leftCav - количество юнитов кавалерии левой армии
    //leftArt - количество юнитов артиллерии левой армии
    //rightInf - количество юнитов пехоты правой армии
    //rightCav - количество юнитов кавалерии правой армии
    //rightArt - количество юнитов артиллерии правой армии
    public static void LoadFightScene(
        int fightSceneNum, int afterFightSceneNum,
        int leftSkin, int rightSkin,
        int leftInf, int leftCav, int leftArt,
        int rightInf, int rightCav, int rightArt)
    {
        PlayerPrefs.SetInt(AfterSceneNumKey, afterFightSceneNum);
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
