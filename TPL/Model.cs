using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class Model
    {
        public string Path;
        public string Source;        
        public Method[] Methods;        
        public Application Application;
        public LineValue[] InstanceValues;
        public Model[] NestedModels;

        public Model(Application application)
        {
            if (application == null)
                throw new InvalidProgramException();

            Application = application;
        }        
    }
}
