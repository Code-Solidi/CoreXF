using System;

namespace CoreXF.Store.Data.Entities
{
    public class QuestionAndAnswer
    {
        public Guid Id { get; set; }

        public string Question { get; set; }

        public AppUser AskedBy { get; set; }

        public DateTime AskedOnUtc { get; set; } = DateTime.UtcNow;

        public string Answer { get; set; }

        public DateTime AnsweredOnUtc { get; set; }

        public Product Product { get; set; }
    }
}