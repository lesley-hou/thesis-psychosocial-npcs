namespace SocialSystem
{
    public class RoleGoal
    {
        public string Description { get; private set; }
        public float Priority { get; set; }

        public RoleGoal(string description, float priority = 1.0f)
        {
            Description = description;
            Priority = priority;
        }
    }
}