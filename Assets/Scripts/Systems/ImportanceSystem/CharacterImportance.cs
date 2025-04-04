using UnityEngine;
using System;

[Serializable]
public class ImportanceFactors
{
    public float playerFocus;
    public float designerImportance;
    public float roleImportance;
    public float contextImportance;

    public float alpha;
    public float beta;
    public float gamma;
    public float delta;
}

[Serializable]
public class PriorityFactors
{
    public float starvationFactor;
    public float runCounter;
    public float completionProgress;

    public float epsilon;
    public float zeta;
    public float eta;
    public float theta;
}

public class CharacterImportance : MonoBehaviour
{
    [SerializeField] private ImportanceFactors importanceFactors;
    [SerializeField] private PriorityFactors priorityFactors;

    public event Action OnImportanceUpdated;
    public event Action OnPriorityUpdated;

    private float starvationRate = 0.05f;
    private float completionDecay = 0.02f;

    private void Update()
    {
        UpdateDynamicFactors();
    }

    // Dynamically change factors over time (starvation, completion progress)
    private void UpdateDynamicFactors()
    {
        priorityFactors.starvationFactor = Mathf.Clamp(
            priorityFactors.starvationFactor + starvationRate * Time.deltaTime, 0f, 1f);

        priorityFactors.completionProgress = Mathf.Clamp(
            priorityFactors.completionProgress - completionDecay * Time.deltaTime, 0f, 1f);

        OnImportanceUpdated?.Invoke();
        OnPriorityUpdated?.Invoke();
    }

    public float CalculateOverallImportance()
    {
        float importance = (
            importanceFactors.alpha * importanceFactors.playerFocus +
            importanceFactors.beta * importanceFactors.designerImportance +
            importanceFactors.gamma * importanceFactors.roleImportance +
            importanceFactors.delta * importanceFactors.contextImportance) / 4;

        return Mathf.Clamp(importance, 0f, 1f);
    }

    public float CalculatePriority()
    {
        float priorityScore =
            priorityFactors.epsilon * CalculateOverallImportance() +
            priorityFactors.zeta * priorityFactors.starvationFactor -
            priorityFactors.eta * priorityFactors.runCounter +
            priorityFactors.theta * priorityFactors.completionProgress;

        return Mathf.Clamp(priorityScore, 0f, 1f);
    }

    public void SetImportanceFactors(ImportanceFactors newFactors)
    {
        importanceFactors = newFactors;
        OnImportanceUpdated?.Invoke();
    }

    public void SetPriorityFactors(PriorityFactors newFactors)
    {
        priorityFactors = newFactors;
        OnPriorityUpdated?.Invoke();
    }
}