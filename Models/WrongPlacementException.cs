using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffic.Models
{
    public class WrongPlacementException : Exception
    {
        public WrongPlacementException() : base()
        {

        }

        public WrongPlacementException(String s) : base(s)
        {

        }
    }
}
