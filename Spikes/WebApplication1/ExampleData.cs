using System;
using System.Collections.Generic;

using WebApplication1.Models;

namespace WebApplication1
{
    public static class ExampleData
    {
        private static List<ExampleModel> examples = new List<ExampleModel>();

        static ExampleData()
        {
            for (var n = 1; n <= 10; n++)
            {
                examples.Add(new ExampleModel { Id = n, Name = $"Title #{n}", Description = $"Description of Title #{n}" });
            }
        }

        public static ICollection<ExampleModel> Get()
        {
            return examples;
        }

        public static ExampleModel Get(int id)
        {
            return examples[id];
        }
    }
}