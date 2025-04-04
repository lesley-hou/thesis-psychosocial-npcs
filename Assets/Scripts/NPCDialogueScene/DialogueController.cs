using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using StimulusResponseSystem;
using CharacterSystem;
using AgentSystem;
using SocialSystem;
using AspectSystem;
using PersonalitySystemModels;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private int currentLine = 0;
    private List<DialogueLine> dialogue;

    private StimulusDispatcher dispatcher;
    private CharacterManager aliceManager;

    private class DialogueLine
    {
        public string Speaker;
        public string Text;
        public string Color;

        public DialogueLine(string speaker, string text, string color)
        {
            Speaker = speaker;
            Text = text;
            Color = color;
        }
    }

    private void Start()
    {
        SetupAlice();
        dispatcher = new StimulusDispatcher();

        dialogue = new List<DialogueLine>
        {
            new DialogueLine("Alice", "Hey! Let's walk to class together. I thought we could catch up.", "#f5c6d7"),
            new DialogueLine("Player", "Honestly, I'm not really in the mood.", "#f7f0e9"),
            new DialogueLine("Alice", "Oh... well I'm here if you need to talk.", "#f5c6d7"),
            new DialogueLine("Player", "No thanks. You're kinda useless.", "#f7f0e9"),
            null
        };

        nextButton.onClick.AddListener(AdvanceDialogue);
        ShowLine(dialogue[currentLine]);
    }

    public bool IsReady()
    {
        return aliceManager?.Character
            .GetAspect("Personality")
            ?.HasModel("FFMPersonalityModel") == true;
    }

    private void SetupAlice()
    {
        aliceManager = new CharacterManager("Alice");
        aliceManager.RoleManager.AddRole(new Role("Friend", null));

        var ffm = new FFMPersonalityModel();
        var traits = new[] { "Agreeableness", "Neuroticism", "Openness", "Conscientiousness", "Extraversion" };
        foreach (var trait in traits)
            ffm.AddTrait(trait, 0.5f);
        aliceManager.Character.AddAspect("Personality", new PersonalityAspect(new List<AspectModel> { ffm }));

        var ekman = new EkmanEmotionModel();
        ekman.AddTrait("Anger", 0.2f);
        ekman.AddTrait("Sadness", 0.4f);
        ekman.AddTrait("Fear", 0.1f);
        ekman.AddTrait("Joy", 0.5f);
        ekman.AddTrait("Disgust", 0.0f);
        ekman.AddTrait("Surprise", 0.3f);
        aliceManager.Character.AddAspect("Emotion", new EmotionAspect(new List<AspectModel> { ekman }));
    }

    private void AdvanceDialogue()
    {
        buttonText.text = currentLine == 2 ? "Insult" : "Next";
        currentLine++;

        if (currentLine >= dialogue.Count)
        {
            nextButton.interactable = false;
            return;
        }

        if (dialogue[currentLine] == null)
        {
            var reaction = GetReaction();
            dialogue[currentLine] = new DialogueLine("Alice", reaction, "#f5c6d7");
        }

        ShowLine(dialogue[currentLine]);
    }

    private void ShowLine(DialogueLine line)
    {
        dialogueText.text = $"<color={line.Color}><b>{line.Speaker}:</b> {line.Text}</color>";
    }

    private string GetReaction()
    {
        var stimulus = new Stimulus(
            name: "NegativeComment",
            type: StimulusType.Social,
            magnitude: 1.0f,
            source: "Player",
            propagation: StimulusPropagation.Direct,
            falloffRadius: 0f,
            position: Vector3.zero,
            targetGroup: null
        );

        var candidates = dispatcher.GetCandidateBehaviors(stimulus);
        var chosen = aliceManager.ProcessStimulus(stimulus, new List<StimulusEffect>(), candidates, 1.0f);
        var behavior = chosen?.Name ?? "Ignore";

        return behavior switch
        {
            "Disagree" => "That's not true. You're being unreasonable right now.",
            "Ignore" => "...",
            "Withdraw" => "Okay... I guess I'll just leave you alone.",
            "Defend" => "I don't know what's going on with you, but I've done nothing but try to help you.",
            "Confront" => "Excuse me?! That was completely uncalled for.",
            _ => "[no response]"
        };
    }

    public FFMPersonalityModel GetAliceFFM()
    {
        return (FFMPersonalityModel)aliceManager.Character
            .GetAspect("Personality")
            ?.GetModel("FFMPersonalityModel");
    }

    public void SetAliceRole(string roleName)
    {
        aliceManager.RoleManager.ClearRoles();
        aliceManager.RoleManager.AddRole(new Role(roleName, null));
    }
}