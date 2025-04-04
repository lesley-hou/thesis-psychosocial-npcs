namespace SocialSystem
{
    public class RoleAction
    {
        public string ActionName { get; private set; }
        public string Description { get; private set; }

        public RoleAction(string actionName, string description = "")
        {
            ActionName = actionName;
            Description = description;
        }
    }
}