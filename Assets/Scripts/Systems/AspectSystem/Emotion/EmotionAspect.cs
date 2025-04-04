using System;
using System.Collections.Generic;
using System.Linq;
using AspectSystem;

public class EmotionAspect : Aspect
{
    public EmotionAspect(List<AspectModel> models)
    {
        Name = "Emotion";
        InitializeModels(models);
    }

    public override void InitializeModels(List<AspectModel> models)
    {
        foreach (var model in models.OfType<EmotionModel>())
        {
            AddModel(model.ModelName, model);
        }
    }
}