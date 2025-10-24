using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [SerializeField]
    private TextMeshProUGUI descriptionMusic;

    [SerializeField]
    private TextMeshProUGUI descriptionSFX;

    [SerializeField]
    private TextMeshProUGUI descriptionGeneral;

    public void TextDescriptionMusic()
    {
        descriptionSFX.gameObject.SetActive(true);
        descriptionMusic.gameObject.SetActive(false);
        descriptionGeneral.gameObject.SetActive(false);
    }

    public void TextDescriptionSFX()
    {
        descriptionSFX.gameObject.SetActive(false);
        descriptionMusic.gameObject.SetActive(true);
        descriptionGeneral.gameObject.SetActive(false);

    }

    public void TextDescriptionGeneral()
    {
        descriptionGeneral.gameObject.SetActive(true);
        descriptionSFX.gameObject.SetActive(false);
        descriptionMusic.gameObject.SetActive(false);
    }
}
