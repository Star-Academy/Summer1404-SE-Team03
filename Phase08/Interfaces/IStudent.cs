namespace ScoreManager.Interfaces
{
    public interface IStudent
    {
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<IScore> Scores { get; set; }
    }
}