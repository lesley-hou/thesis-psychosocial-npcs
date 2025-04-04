using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using AspectSystem;
using PersonalitySystemModels;
using CharacterSystem;

public class TraitAdjustmentUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown traitDropdown;
    [SerializeField] private Slider traitSlider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Toggle roleToggle;
    [SerializeField] private DialogueController dialogueController;

    private FFMPersonalityModel ffmModel;

    private readonly string[] traitNames = {
        "Extraversion", "Agreeableness", "Neuroticism",
        "Openness", "Conscientiousness"
    };

    private IEnumerator Start()
    {
        while (dialogueController == null || !dialogueController.IsReady())
            yield return null;

        ffmModel = dialogueController.GetAliceFFM();
        if (ffmModel == null) yield break;

        traitDropdown.ClearOptions();
        traitDropdown.AddOptions(new List<string>(traitNames));

        UpdateUIForTrait(traitNames[traitDropdown.value]);

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)traitSlider.transform);

        traitDropdown.onValueChanged.AddListener(OnTraitChanged);
        traitSlider.onValueChanged.AddListener(OnSliderChanged);
        roleToggle.onValueChanged.AddListener(OnRoleToggleChanged);

        SetInitialRole();
    }

    private void OnTraitChanged(int index)
    {
        UpdateUIForTrait(traitNames[index]);
    }

    private void OnSliderChanged(float value)
    {
        if (ffmModel == null) return;

        string trait = traitNames[traitDropdown.value];
        if (!ffmModel.Traits.ContainsKey(trait)) return;

        ffmModel.SetTraitValue(trait, value);
        if (valueText != null)
            valueText.text = value.ToString("0.00");
    }

    private void UpdateUIForTrait(string trait)
    {
        if (ffmModel == null || !ffmModel.Traits.ContainsKey(trait)) return;

        float value = ffmModel.GetTraitValue(trait);
        traitSlider.SetValueWithoutNotify(value);
        if (valueText != null)
            valueText.text = value.ToString("0.00");
    }

    private void OnRoleToggleChanged(bool isRival)
    {
        string role = isRival ? "Rival" : "Friend";
        dialogueController.SetAliceRole(role);
    }

    private void SetInitialRole()
    {
        string initialRole = roleToggle.isOn ? "Rival" : "Friend";
        dialogueController.SetAliceRole(initialRole);
    }
}