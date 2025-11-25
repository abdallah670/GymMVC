namespace GymDAL.Entities.External
{
    public class FitnessGoals:BaseEntity
    {
        public int Id { get; set; }
        public string GoalsName { get; set; }
        public string GoalsDescription { get; set; }
        
    }
}