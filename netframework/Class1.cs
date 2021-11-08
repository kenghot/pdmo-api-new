using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace netframework
{
    public class Class1
    {
        public void Test ()
        {
            var db = new Model.mofDataContext();
            var g = from gs in db.CE_LOVGroups select gs;

            foreach (netframework.Model.CE_LOVGroup tmp in g)
            {

            }
        }
    }
}
