using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SliderUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;

    public VolumeScript volumeChanger;

    public int type;

    private void Start()
    {
        if (type == 0)
        {
            _slider.value = volumeChanger.masterVolume;
            _sliderText.text = _slider.value.ToString("0%");
        }
        if (type == 1)
        {
            _slider.value = volumeChanger.songVolume;
            _sliderText.text = _slider.value.ToString("0%");
        }
        if (type == 2)
        {
            _slider.value = volumeChanger.effectVolume;
            _sliderText.text = _slider.value.ToString("0%");
        }



        _slider.onValueChanged.AddListener((v) =>
        {
            _sliderText.text = v.ToString("0%");//"0.00"

            volumeChanger.ChangeVolume(v, type);
        });
    }
}