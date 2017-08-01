using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public static class CodeFile
    {
        public static void Build(Application application, params string[] paths)
        {            
            for (int i = 0; i < paths.Length; i++)
            {
                AnalyseSourceFromPath(application, paths[i]);
            }
        }

        public static void AnalyseSourceFromPath(Application application, string path)
        {
            AnalyseSource(application, System.IO.File.ReadAllText(path));            
        }

        public static Application AnalyseSource(Application application, string source)
        {
            source = PreParse(source);
            if (string.IsNullOrWhiteSpace(source))
                throw new InvalidProgramException();
            List<Model> models = new List<Model>();
            
            foreach (var spec in GetClasses(source))
            {
                var model = new Model(application);

                GenerateModelFromSource(model, spec.Source);
                models.Add(model);
            }
            if(models.Count > 0)
                application.Models.AddRange(models);

            return application;
        }

        private static void GenerateModelFromSource(Model model, string source)
        {
            List<Model> models = new List<Model>();

            foreach (var spec in GetClasses(source))
            {
                var model_nested = new Model(model.Application);

                GenerateModelFromSource(model_nested, spec.Source);

                models.Add(model);
            }
            if(models.Count > 0)
                model.NestedModels = models.ToArray();
        }
        
        public static string GetInnerCode(int start, string source)
        {
            int currentLevel = 0;
            bool inQuote = false;
            var builder = new StringBuilder();
            bool foundend = false;
            bool HasOpened = false;
            for (int i = start; i < source.Length; i++)
            {
                if(source[i] == '"')
                {
                    inQuote = !inQuote;
                }
                builder.Append(source[i]);
                if (inQuote)
                    continue;                

                if (source[i] == '{')
                {
                    currentLevel++;
                    HasOpened = true;
                }
                else if (source[i] == '}')
                {
                    currentLevel--;
                }
                
                if (currentLevel == 0 && HasOpened)
                {
                    foundend = true;                
                    break;
                }                
            }

            if (!foundend)
            {
                throw new InvalidProgramException();
            }
            else
                return builder.ToString();
        }
        
        private static bool IsValidName(string name)
        {
            // is keyword???
            if(string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            if(name.Length > 0)
            {
                if(!(char.IsLetter(name[0]) || name[0] == '_'))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            for (int i = 1; i < name.Length; i++)
            {
                if (!(char.IsLetterOrDigit(name[0]) || name[0] == '_'))
                {
                    return false;
                }
            }
            return true;
        }

        private static string TrimEdges(string source)
        {
            source = (source + "").Trim();

            if(source.FirstOrDefault() == '{' && source.LastOrDefault() == '}')
            {                
                source = source.Substring(1, source.Length - 2).Trim();
            }
            return source;
        }

        private static ClassSpec[] GetClasses(string source)
        {
            source = TrimEdges(source);
            var specs = new List<ClassSpec>();
            bool inQuote = false;

            var builder = new StringBuilder();
            var CollectedWords = new List<string>();
            int currentLevel = 0;

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '"')
                {
                    inQuote = !inQuote;
                }
                if (inQuote)
                    continue;

                if (source[i] == '{')
                {
                    currentLevel++;
                    CollectedWords = new List<string>();
                    builder = new StringBuilder();
                }
                else if (source[i] == '}')
                {
                    currentLevel--;
                    CollectedWords = new List<string>();
                    builder = new StringBuilder();
                }
                if(currentLevel == 0)
                {
                    if (source[i] == ' ' )
                    {
                        string currentWord = builder.ToString();
                        if (currentWord.Length == 0)
                            continue; // cunt found ya;

                        CollectedWords.Add(currentWord);

                        if (CollectedWords.Count > 3)
                        {
                            CollectedWords = new List<string>();
                        }
                        else if (CollectedWords.Count > 1)
                        {
                            string shouldBeName = CollectedWords.LastOrDefault();
                            if (IsValidName(shouldBeName))
                            {
                                if(CollectedWords[CollectedWords.Count - 2] == "class")
                                {
                                    var spec = new ClassSpec();
                                    spec.Name = shouldBeName;
                                    if (CollectedWords.Count == 3)
                                    {
                                        spec.Modifier = CollectedWords[0];
                                    }

                                    spec.Source = GetInnerCode(i, source);

                                    i += spec.Source.Length;

                                    specs.Add(spec);

                                    CollectedWords = new List<string>();
                                    builder = new StringBuilder();
                                }
                            }
                        }

                        builder = new StringBuilder();
                    }
                    else
                    {
                        if (source[i] == '{' || source[i] == '}')
                            continue;
                        builder.Append(source[i]);
                    }
                }                                
            }

            return specs.ToArray();
        }

        private class ClassSpec
        {
            public string Name;
            public string Modifier;
            public string Source;
        }

        public static string PreParse(string data)
        {
            var builder = new StringBuilder();
            int length = data.Length;

            bool inQuote = false;
            bool prevWhitespace = false;
            bool prevSep = false;
            bool inComment = false;
            char prevChar = ' ';

            for (int i = 0; i < length; i++)
            {
                char current = data[i];
                bool isWhiteSpace;
                bool isSep;
                if(!inComment && current == '"')
                {
                    inQuote = !inQuote;
                    isWhiteSpace = false;
                    isSep = false;
                }
                else
                {
                     isWhiteSpace = char.IsWhiteSpace(current);
                    bool isNewLine = current == '\r' || current == '\n';
                    isSep = current == ';' || isNewLine;

                    if(isNewLine && inComment)
                    {
                        inComment = false;
                        prevWhitespace = true;
                    }
                    else if(inComment)
                    {
                        continue;
                    }
                }

                if (inQuote)
                {
                    builder.Append(current);
                }
                else
                {                    
                    if ((isWhiteSpace && prevWhitespace) || (isSep && prevSep))
                        continue;
                    // we are in code;
                    if(current != '/')
                    {
                        builder.Append(isWhiteSpace ? ' ' : current);
                    }
                    else if(prevChar == '/')
                    {
                        inComment = true;
                    }
                }

                prevWhitespace = isWhiteSpace;
                prevSep = isSep;
                prevChar = current;
            }
            
            return builder.ToString();
        }
    }
}
