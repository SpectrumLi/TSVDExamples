using SequelocityDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSVD
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 10;
            double y = 0;
            float z = 0;
            long xx = 0;
            char yy = '1';

       
            Dictionary<int, Type> d = new Dictionary<int, Type>();
            d[1] = x.GetType();
            d[2] = y.GetType(); 
            d[3] = z.GetType(); 
            d[4] = xx.GetType();
            d[5] = yy.GetType(); 

            Parallel.ForEach(d.Keys, (i) =>
            {
                try
                {
                    TypeCacher.GetPropertiesAndFields(d[i]);
                    TypeCacher.GetPropertiesAndFields(d[i]);
                }
                catch (Exception)
                {

                }
            });

        }
    }
}
