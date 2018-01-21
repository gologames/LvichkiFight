using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//часть кода для демонстрации
//запускается после битвы, показывает результат
//и кнопку рестарт
public class AfterFight : MonoBehaviour
{
    public void OnGUI()
    {
        //получение информации после битвы

        //победила левая или правая сторона?
        bool isLeftWin = PlayerPrefs.GetString(StartFight.WinKey) ==
            StartFight.LeftWinKey;

        //получение количества оставшихся войск
        int leftInf = PlayerPrefs.GetInt(StartFight.LeftInfKey);
        int leftCav = PlayerPrefs.GetInt(StartFight.LeftCavKey);
        int leftArt = PlayerPrefs.GetInt(StartFight.LeftArtKey);
        int rightInf = PlayerPrefs.GetInt(StartFight.RightInfKey);
        int rightCav = PlayerPrefs.GetInt(StartFight.RightCavKey);
        int rightArt = PlayerPrefs.GetInt(StartFight.RightArtKey);

        //далее вывод этой информации
        string winString = isLeftWin ? "Left win" : "Right win";
        GUI.Label(new Rect(0, 0, 100, 50), winString);

        GUI.Label(new Rect(0, 100, 100, 50), "Left inf: " + leftInf);
        GUI.Label(new Rect(0, 150, 100, 50), "Left cav: " + leftCav);
        GUI.Label(new Rect(0, 200, 100, 50), "Left art: " + leftArt);
        GUI.Label(new Rect(200, 100, 100, 50), "Right inf: " + rightInf);
        GUI.Label(new Rect(200, 150, 100, 50), "Right cav: " + rightCav);
        GUI.Label(new Rect(200, 200, 100, 50), "Right art: " + rightArt);

        if (GUI.Button(new Rect(300, 100, 100, 50), "Restart"))
        { SceneManager.LoadScene(0); }
    }
}
