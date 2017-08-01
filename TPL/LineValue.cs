using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class LineValue
    {

    }


    public class LineValueBool : LineValue
    {
        public bool Value;
    }

    public class LineValueByte : LineValue
    {
        public byte Value;        
    }

    public class LineValueShort : LineValue
    {
        public short Value;
        
    }

    public class LineValueInt : LineValue
    {
        public int Value;
        
    }

    public class LineValueLong : LineValue
    {
        public long Value;
        
    }

    public class LineValueFloat : LineValue
    {
        public float Value;
        
    }

    public class LineValueDouble : LineValue
    {
        public double Value;
        
    }

    public class LineValueDecimal : LineValue
    {
        public decimal Value;
        
    }

    public class LineValueChar : LineValue
    {
        public char Value;
    }

    public class LineValueString : LineValue
    {
        public string Value;
        
    }

    public class LineValueMethod : LineValue
    {
        public Method Value;
    }

    public class LineValueList<T> : LineValue
        where T : LineValue
    {
        public List<T> Value;        
        public LineValueList(int capacity = 0)
        {
            Value = new List<T>(capacity);
        }
    }
}
