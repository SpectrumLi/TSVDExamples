using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thunderstruck.Runtime;

namespace TSVD
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionStringBuffer csb = new ConnectionStringBuffer();
            int n = 20;
            int[] x = new int[n];
            for (int i = 0; i < n; i++) x[i] = i;
            Parallel.ForEach(x, (y) =>
            {
                try
                {
                    
                    csb.Get(y.ToString());
                }
                catch (Exception)
                {

                }
            });
        }
    }
}
