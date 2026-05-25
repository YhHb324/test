using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    public Image previewImage;

    public TMP_Text worldNameText;

    public TMP_Text descriptionText;

    WorldData worldData;

    public void Setup(WorldData data)
    {
        worldData = data;

        previewImage.sprite =
            data.previewImage;

        worldNameText.text =
            data.worldName;

        descriptionText.text =
            data.description;
    }

    public void OnClick()
    {
        StageManager.Instance.SelectWorld(
            worldData);
    }
}