using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarathonSkills2016.Database
{
    internal class ConnectionString
    {
        public static DoneEntities connection { get { return new DoneEntities(); } }
    }
}
