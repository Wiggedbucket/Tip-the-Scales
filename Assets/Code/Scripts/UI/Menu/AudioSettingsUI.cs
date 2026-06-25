using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField]
    private UIDocument document;

    private VisualElement container;

    private Slider masterSlider;
    private Slider SFXSlider;
    private Slider musicSlider;

    private Button backButton;

    private void Awake()
    {
        container = document.rootVisualElement.Q<VisualElement>("Container");

        masterSlider = container.Q<Slider>("MasterSlider");
        SFXSlider = container.Q<Slider>("SFXSlider");
        musicSlider = container.Q<Slider>("MusicSlider");

        backButton = container.Q<Button>("BackButton");

        masterSlider.RegisterValueChangedCallback(evt =>
        {
            AudioMixerManager.instance.SetMasterVolume(evt.newValue);
        });
        SFXSlider.RegisterValueChangedCallback(evt =>
        {
            AudioMixerManager.instance.SetSoundFXVolume(evt.newValue);
        });
        musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioMixerManager.instance.SetMusicVolume(evt.newValue);
        });

        backButton.clicked += CloseMenu;
    }

    private void CloseMenu()
    {
        container.AddToClassList("hidden");
    }
}
