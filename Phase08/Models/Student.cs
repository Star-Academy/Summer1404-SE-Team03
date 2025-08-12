﻿namespace Phase01
{
    public class Student
    {
        public int StudentNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
