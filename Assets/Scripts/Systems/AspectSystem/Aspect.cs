using System.Collections.Generic;

namespace AspectSystem
{
    public abstract class Aspect
    {
        public string Name { get; protected set; }
        public Dictionary<string, AspectModel> Models { get; private set; } = new();

        public abstract void InitializeModels(List<AspectModel> models);

        public void AddModel(string modelName, AspectModel model)
        {
            if (!Models.ContainsKey(modelName))
                Models[modelName] = model;
        }

        public bool HasModel(string modelName) => Models.ContainsKey(modelName);

        public T GetModel<T>() where T : AspectModel
        {
            foreach (var model in Models.Values)
                if (model is T tModel)
                    return tModel;
            return null;
        }

        public AspectModel GetModel(string modelName) =>
            Models.TryGetValue(modelName, out var model) ? model : null;
    }
}