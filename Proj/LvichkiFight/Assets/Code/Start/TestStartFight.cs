using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestStartFight : MonoBehaviour
{

    [SerializeField] private Slider leftSkinSlider;
    [SerializeField] private Slider rightSkinSlider;
    [SerializeField] private Slider leftInfSlider;
    [SerializeField] private Slider leftCavSlider;
    [SerializeField] private Slider leftArtSlider;
    [SerializeField] private Slider rightInfSlider;
    [SerializeField] private Slider rightCavSlider;
    [SerializeField] private Slider rightArtSlider;
    public void OnLeftSkinSlider()
    { leftSkinSlider.transform.Find("Text").GetComponent<Text>().text = "Left Skin: " + (int)leftSkinSlider.value; }
    public void OnRightSkinSlider()
    { rightSkinSlider.transform.Find("Text").GetComponent<Text>().text = "Right Skin: " + (int)rightSkinSlider.value; }
    public void OnLeftInfSlider()
    { leftInfSlider.transform.Find("Text").GetComponent<Text>().text = "Left Inf: " + (int)leftInfSlider.value; }
    public void OnLeftCavSlider()
    { leftCavSlider.transform.Find("Text").GetComponent<Text>().text = "Left Cav: " + (int)leftCavSlider.value; }
    public void OnLeftArtSlider()
    { leftArtSlider.transform.Find("Text").GetComponent<Text>().text = "Left Art: " + (int)leftArtSlider.value; }
    public void OnRightInfSlider()
    { rightInfSlider.transform.Find("Text").GetComponent<Text>().text = "Right Inf: " + (int)rightInfSlider.value; }
    public void OnRightCavSlider()
    { rightCavSlider.transform.Find("Text").GetComponent<Text>().text = "Right Cav: " + (int)rightCavSlider.value; }
    public void OnRightArtSlider()
    { rightArtSlider.transform.Find("Text").GetComponent<Text>().text = "Right Art: " + (int)rightArtSlider.value; }
    public void Fight()
    {
            StartFight.LoadFightScene(1, 2,
                (int)leftSkinSlider.value,
                (int)rightSkinSlider.value,
                (int)leftInfSlider.value,
                (int)leftCavSlider.value,
                (int)leftArtSlider.value,
                (int)rightInfSlider.value,
                (int)rightCavSlider.value,
                (int)rightArtSlider.value);
    }
}
