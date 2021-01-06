using System;
using System.Collections.Generic;

namespace Miscelaneous
{
    public class DescribeAttribute : Attribute
    {
        public string Description { get; set; }

        public DescribeAttribute(string description)
        {
            this.Description = description;
        }

        //public List<object> Parameters { get; set; }
    }
}