using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag
{
    internal class ID_Generator
    {
        public int increment_ID = 0;

        public int next()
        {
            increment_ID++;
            return increment_ID;
        }

        public void reset()
        {
            increment_ID = 0;
        }
    }
}
