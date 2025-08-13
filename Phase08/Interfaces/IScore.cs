namespace ScoreManager.Interfaces
{
    public interface IScore
    {
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public string Lesson { get; set; }
        public double Value { get; set; }
        public IStudent? Student { get; set; }        
    }    
}