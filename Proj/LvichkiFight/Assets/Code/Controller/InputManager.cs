using UnityEngine;
using System.Collections;

//интерфейс обработки ввода
//наследовать и реализовать его для
//реализации обработки джойстика или еще чего
//еще нужно будет в BattleGUIScript.cs создать
//этот новый класс вместо InputManager
public interface IInputManager
{
    //была ли перемещена камера вправо
    bool IsMoveRight();
    //была ли перемещена камера влево
    bool IsMoveLeft();
    //была ли перемещена камера наверх
    bool IsMoveUp();
    //была ли перемещена камера вниз
    bool IsMoveDown();
    //какое изменение масштаба камеры прозошло
    float GetScroll();
}

//реализация IInputManager для работы с мышью
public class InputManager : IInputManager
{
    //граница в пикселях от граней экрана
    //при преодолении которой происходит
    //движение камеры
    private const int edgeScreenBound = 20;
    
    //реализация пора ли двигаться камере вправо
    public bool IsMoveRight()
    {
        return Input.mousePosition.x >
            Screen.width - edgeScreenBound;
    }

    //реализация пора ли двигаться камере влево
    public bool IsMoveLeft()
    { return Input.mousePosition.x < edgeScreenBound; }

    //реализация пора ли двигаться камере наверх
    public bool IsMoveUp()
    {
        return Input.mousePosition.y >
            Screen.height - edgeScreenBound;
    }

    //реализация пора ли двигаться камере вниз
    public bool IsMoveDown()
    { return Input.mousePosition.y < edgeScreenBound; }

    //реализация насколько нужно масштабировать камеру
    //на основе кручения колесика мыши
    public float GetScroll()
    { return Input.GetAxis("Mouse ScrollWheel"); }
}
