using System;
using System.Collections.Generic;
using System.Linq;
using AspectSystem;

public class PersonalityAspect : Aspect
{
    public Dictionary<string, PersonalityModel> PersonalityModels { get; private set; } = new();

    public PersonalityAspect(List<AspectModel> models)
    {
        Name = "Personality";
        InitializeModels(models);
    }

    public override void InitializeModels(List<AspectModel> models)
    {
        foreach (var model in models.OfType<PersonalityModel>())
        {
            AddModel(model.ModelName, model);
            PersonalityModels[model.ModelName] = model;
        }
    }
}