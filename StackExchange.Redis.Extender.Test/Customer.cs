using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extender.Test
{
    public class Customer
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }

        public override string ToString() => $"Id: {Id} - Age: {Age} - Name: {Name}";
    }
}
