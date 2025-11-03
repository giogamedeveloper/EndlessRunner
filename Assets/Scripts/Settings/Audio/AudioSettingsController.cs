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

    void Start()
    {
        // Establecer valores iniciales
        musicVolumeSlider.value = AudioController.Instance.GetMusicVolume();
        masterVolumeSlider.value = AudioController.Instance.GetGeneralVolume();
        sfxVolumeSlider.value = AudioController.Instance.GetEffectVolume();
    
        // Asignar listeners
        musicVolumeSlider.onValueChanged.AddListener(AudioController.Instance.SetMusicVolume);
        masterVolumeSlider.onValueChanged.AddListener(AudioController.Instance.SetGeneralVolume);
        sfxVolumeSlider.onValueChanged.AddListener(AudioController.Instance.SetEffectVolume);
    }
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
