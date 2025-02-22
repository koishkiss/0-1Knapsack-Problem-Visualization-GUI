using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag
{
    internal class RowException : Exception
    {
        public string msg;
        public RowException(string message) {
            this.msg = message;
        }
    }
}
