using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class Line
    {
        public LineType Type;
        public LineValue Value;
        public int Index;
        public int ArgumentCount;

        public readonly LineRangeType RangeType;

        public Line()
        {
            if (this is LineArgument)
                RangeType = LineRangeType.Argument;
            else if (this is LineGlobal)
                RangeType = LineRangeType.Global;
            else if (this is LineInstance)
                RangeType = LineRangeType.Instance;            
            else
                RangeType = LineRangeType.None;
        }
    }

    public class LineArgument : Line
    {
        
    }

    public class LineGlobal : Line
    {

    }
    
    public class LineInstance : Line
    {

    }

    public enum LineRangeType
    {
        None,
        Argument,
        Global,
        Instance
    }

    public enum LineType
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Add_Strict,
        Sub_Strict,
        Mul_Strict,
        Div_Strict,
        Mod_Strict,
        Set,
        Read,
        Literal,
        Call,        
        Return,
        Cast,
        Nop,
        New_List,
        Read_List,
        Set_List,
        Add_List,
        Remove_List
    }
    
    public static class Generics
    {
        public const int Bool = 0,
        Byte = 1,
        Short = 2,
        Int = 3,
        Long = 4,
        Float = 5,
        Double = 6,
        Decimal = 7,
        String = 8,
        Char = 9,
        List = 10;
    }
}
