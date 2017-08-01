using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class Application
    {
        public List<Model> Models = new List<Model>();
        public Method Main;    

        public LineValue[] GlobalValues;    
        public Method[] GlobalMethods;

        public Application()
        {

        }

        public void Run(params string[] args)
        {
            if (Main == null)
                throw new InvalidProgramException();

            var lvs = new LineValue[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                lvs[i] = new LineValueString() { Value = args[i] };
            }
            Main.Invoke(lvs);
        }
    }
}
