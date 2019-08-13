using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace TSVD
{
    class Program
    {
        static void Main(string[] args)
        {
            const int numOfTasks = 15;

            var properties = new[] { new DynamicProperty("prop1", typeof(string)) };

            var tasks = new List<Task>(numOfTasks);

            for (var i = 0; i < numOfTasks; i++)

            {
                tasks.Add(Task.Factory.StartNew(() => DynamicExpression.CreateClass(properties)));

            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}
