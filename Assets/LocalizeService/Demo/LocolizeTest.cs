using System;
using Localization;
using UnityEngine;
using UnityEngine.UI;

// For DEMO VIEW
public class LocolizeTest : MonoBehaviour
{
    public Toggle Localization1;
    public Toggle Localization2;
    public Toggle Localization3;

    private void Start()
    {
        Localization1.onValueChanged.AddListener(OnLocalization1Set);
        Localization2.onValueChanged.AddListener(OnLocalization2Set);
        Localization3.onValueChanged.AddListener(OnLocalization3Set);

        CheckLocalization();
    }

    private void CheckLocalization()
    {
        if (LocalizationService.Instance == null) return;

        switch (LocalizationService.Instance.Localization)
        {
            case "Russian":
                Localization2.isOn = true;
                break;
            case "English":
                Localization1.isOn = true;
                break;
            case "French":
                Localization3.isOn = true;
                break;
        }
    }

    private void OnLocalization1Set(bool value)
    {
        if (!value) return;
        LocalizationService.Instance.Localization = "English";
    }
    private void OnLocalization2Set(bool value)
    {
        if (!value) return;
        LocalizationService.Instance.Localization = "Russian";
    }
    private void OnLocalization3Set(bool value)
    {
        if (!value) return;
        LocalizationService.Instance.Localization = "French";
    }

}