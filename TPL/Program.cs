namespace TPL
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeFile.AnalyseSource(new Application(),
@"public class Apple
{
    // This is a comment
    public void HelloWorld()
    {
        MessageBox.Show(""Hello World"");    
    }

    public class AppleHelper
    {

    }
}");




            var methHead = new Method(null)
            {
                Locals = new LineValue[] {

                },
                Lines = new Line[] {
                    new Line() { Type = LineType.Literal, Value = new LineValueInt() { Value = 1 } },
                    new Line() { Type = LineType.Literal, Value = new LineValueFloat() { Value = 100 } },
                    new Line() { Type = LineType.Cast, Index = Generics.Int },
                    new Line() { Type = LineType.Add_Strict },
                    new Line() { Type = LineType.Return }
                }
            };            
            
            var value = methHead.Invoke(null);
            
            //if (args == null || args.Length == 0)
            //    return;
            //var app = new Application();
            //app.CreateFromParsedCodeFiles(CodeFile.Create(app, args))?.
            //    Run();
        }
    }
}
