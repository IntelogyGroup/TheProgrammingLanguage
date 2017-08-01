using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class Method
    {
        public string Name;
        public LineValue[] Locals;
        public Line[] Lines;
        public Model ClassModel;
        public int MaxStack = 2;
        public bool IsVoid;

        public Method(Model classModel)
        {
            ClassModel = classModel;
        }

        public LineValue Invoke(params LineValue[] arguments)
        {            
            using (var lineManager = new LineManager(Lines))
            {
                if(!lineManager.AtEnd())
                {

                    // Value 1 is added to stack
                    // Value 2 is added to stack
                    // Add - Pop Value 2 off stack, Pop Value 1 off stack = length = 0;
                    // Push Add Result on stack
                    var stack = new Stack<LineValue>(MaxStack);
                    LineValue _local = null;
                    LineValue _local2 = null;
                    IList ilist;
                    dynamic type;
                    do
                    {
                        
                        var active = Lines[lineManager.index];
                        switch (active.Type)
                        {
                            #region "Math State Machine, Enter if your dare!!!##$#$"
                            case LineType.Add:                                
                                _local = stack.Pop();
                                type = ((dynamic)_local).Value.GetType();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));                                
                                ((dynamic)_local2).Value = (((dynamic)_local).Value + Convert.ChangeType(((dynamic)stack.Pop()).Value, type));

                                stack.Push(_local2);
                                break;
                            case LineType.Sub:
                                _local = stack.Pop();
                                type = ((dynamic)_local).Value.GetType();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value - Convert.ChangeType(((dynamic)stack.Pop()).Value, type));

                                stack.Push(_local2);
                                break;
                            case LineType.Mul:
                                _local = stack.Pop();
                                type = ((dynamic)_local).Value.GetType();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value * Convert.ChangeType(((dynamic)stack.Pop()).Value, type));

                                stack.Push(_local2);
                                break;
                            case LineType.Div:
                                _local = stack.Pop();
                                type = ((dynamic)_local).Value.GetType();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value / Convert.ChangeType(((dynamic)stack.Pop()).Value, type));

                                stack.Push(_local2);
                                break;
                            case LineType.Mod:
                                _local = stack.Pop();
                                type = ((dynamic)_local).Value.GetType();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value % Convert.ChangeType(((dynamic)stack.Pop()).Value, type));

                                stack.Push(_local2);
                                break;
                            case LineType.Add_Strict:
                                _local = stack.Pop();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value + ((dynamic)stack.Pop()).Value);

                                stack.Push(_local2);
                                break;
                            case LineType.Sub_Strict:
                                _local = stack.Pop();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value - ((dynamic)stack.Pop()).Value);

                                stack.Push(_local2);
                                break;
                            case LineType.Mul_Strict:
                                _local = stack.Pop();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value * ((dynamic)stack.Pop()).Value);

                                stack.Push(_local2);
                                break;
                            case LineType.Div_Strict:
                                _local = stack.Pop();                                
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value / ((dynamic)stack.Pop()).Value);

                                stack.Push(_local2);
                                break;
                            case LineType.Mod_Strict:
                                _local = stack.Pop();
                                _local2 = ((LineValue)Activator.CreateInstance(_local.GetType()));
                                ((dynamic)_local2).Value = (((dynamic)_local).Value % ((dynamic)stack.Pop()).Value);

                                stack.Push(_local2);
                                break;
                            #endregion
                            case LineType.Set:
                            case LineType.Read:                                
                                LineValue[] list = null;
                                
                                switch (active.RangeType)
                                {
                                    case LineRangeType.None:
                                        list = Locals;
                                        break;
                                    case LineRangeType.Argument:
                                        list = arguments;
                                        break;
                                    case LineRangeType.Global:
                                        list = ClassModel.Application.GlobalValues;
                                        break;
                                    case LineRangeType.Instance:
                                        list = ClassModel.InstanceValues;
                                        break;                                    
                                }

                                if(active.Type == LineType.Read)
                                {
                                    stack.Push(list[active.Index]);
                                }
                                else
                                {
                                    list[active.Index] = stack.Pop();                                    
                                }
                                break;                            
                            case LineType.Literal:
                                stack.Push(active.Value);
                                break;
                            case LineType.Call:
                                LineValueMethod call = null;
                                switch (active.RangeType)
                                {
                                    case LineRangeType.None:
                                        call = (LineValueMethod)Locals[active.Index];
                                        break;
                                    case LineRangeType.Argument:
                                        call = (LineValueMethod)arguments[active.Index];
                                        break;
                                    case LineRangeType.Global:
                                        call = new LineValueMethod() { Value = ClassModel.Application.GlobalMethods[active.Index] };
                                        break;
                                    case LineRangeType.Instance:
                                        call = new LineValueMethod() { Value = ClassModel.Methods[active.Index] };
                                        break;
                                }
                                int length = active.ArgumentCount;
                                if (length == 0)
                                    _local = call.Value.Invoke();
                                else
                                {                                    
                                    var args = new LineValue[length];
                                    for (int i = 0; i < length; i++)
                                    {
                                        args[i] = stack.Pop();
                                    }

                                    _local = call.Value.Invoke(args);
                                }
                                if(!call.Value.IsVoid)
                                {
                                    stack.Push(_local);
                                }

                                break;
                            case LineType.Cast:                                
                                switch (active.Index)
                                {
                                    case Generics.Bool:
                                        stack.Push(new LineValueBool() { Value = (bool)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Byte:
                                        stack.Push(new LineValueByte() { Value = (byte)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Short:
                                        stack.Push(new LineValueShort() { Value = (short)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Int:
                                        stack.Push(new LineValueInt() { Value = (int)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Long:
                                        stack.Push(new LineValueLong() { Value = (long)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Float:
                                        stack.Push(new LineValueFloat() { Value = (float)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Double:
                                        stack.Push(new LineValueDouble() { Value = (double)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Decimal:
                                        stack.Push(new LineValueDecimal() { Value = (decimal)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.String:
                                        stack.Push(new LineValueString() { Value = (string)((dynamic)stack.Pop()).Value });
                                        break;
                                    case Generics.Char:
                                        stack.Push(new LineValueChar() { Value = (char)((dynamic)stack.Pop()).Value });
                                        break;
                                }
                                break;
                            case LineType.Return:
                                return stack.Pop();
                            default:
                            case LineType.Nop:                            
                                break;
                            case LineType.New_List:
                                switch (active.Index)
                                {
                                    case Generics.Bool:
                                        stack.Push(new LineValueList<LineValueBool>(active.ArgumentCount));
                                        break;
                                    case Generics.Byte:
                                        stack.Push(new LineValueList<LineValueByte>(active.ArgumentCount));
                                        break;
                                    case Generics.Short:
                                        stack.Push(new LineValueList<LineValueShort>(active.ArgumentCount));
                                        break;
                                    case Generics.Int:
                                        stack.Push(new LineValueList<LineValueInt>(active.ArgumentCount));
                                        break;
                                    case Generics.Long:
                                        stack.Push(new LineValueList<LineValueLong>(active.ArgumentCount));
                                        break;
                                    case Generics.Float:
                                        stack.Push(new LineValueList<LineValueFloat>(active.ArgumentCount));
                                        break;
                                    case Generics.Double:
                                        stack.Push(new LineValueList<LineValueDouble>(active.ArgumentCount));
                                        break;
                                    case Generics.Decimal:
                                        stack.Push(new LineValueList<LineValueDecimal>(active.ArgumentCount));
                                        break;
                                    case Generics.String:
                                        stack.Push(new LineValueList<LineValueString>(active.ArgumentCount));
                                        break;
                                    case Generics.Char:
                                        stack.Push(new LineValueList<LineValueChar>(active.ArgumentCount));
                                        break;
                                }                                
                                break;
                            case LineType.Read_List:
                                ilist = (IList)((dynamic)stack.Pop()).Value;

                                stack.Push((LineValue)ilist[active.Index]);

                                break;
                            case LineType.Set_List:
                                ilist = (IList)((dynamic)stack.Pop()).Value;
                                ilist[active.Index] = stack.Pop();

                                break;
                            case LineType.Add_List:
                                ilist = (IList)((dynamic)stack.Pop()).Value;
                                ilist.Add(stack.Pop());

                                break;
                            case LineType.Remove_List:
                                ilist = (IList)((dynamic)stack.Pop()).Value;
                                ilist.RemoveAt(active.Index);

                                break;
                        }
                    } while (lineManager.MoveNext());
                }
            }

            return null;
        }
    }
}
