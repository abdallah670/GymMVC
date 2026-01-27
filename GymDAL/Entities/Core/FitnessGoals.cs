namespace GymDAL.Entities.Core
{
    public class FitnessGoals:BaseEntity
    {
        public int Id { get; set; }
        public string GoalName { get; set; }
        public string GoalDescription { get; set; }
        
    }
}