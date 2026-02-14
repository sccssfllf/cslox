namespace Tools;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output_directory>");
            Environment.Exit(64);
        }

        string outputDir = args[0];

        DefineAst(outputDir, "Expr", new List<string>
        {
            "Binary         : Expr left, Token op, Expr right",
            "Grouping       : Expr expression",
            "Literal        : object value",
            "Unary          : Token op, Expr right"
        });
    }
    
    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = Path.Combine(outputDir, baseName + ".cs");
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("");
            writer.WriteLine("namespace cslox;");
            writer.WriteLine("");
            writer.WriteLine($"public abstract class {baseName}");
            writer.WriteLine("{");

            DefineVisitor(writer, baseName, types);
                
            writer.WriteLine("");
            writer.WriteLine("      public abstract R Accept<R>(IVisitor<R> visitor);");

            foreach (string type in types)
            {
                string className = type.Split(':')[0].Trim();
                string fields = type.Split(':')[1].Trim();
                DefineType(writer, baseName, className, fields);
            }
                
            writer.WriteLine("}");
        }
    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine("");
        writer.WriteLine($"     public class {className} : {baseName}");
        writer.WriteLine("      {");
        writer.WriteLine($"         public {className}({fieldList})");
        writer.WriteLine("              {");
        string[] fields = fieldList.Split(", ");
        foreach (string field in fields)
        {
            string name = field.Split()[1];
            string propName = char.ToUpper(name[0]) + name.Substring(1);
            writer.WriteLine($"                 this.{propName} = {name};");
        }
        writer.WriteLine("              }");
        writer.WriteLine("");
        foreach (string field in fields)
        {
            string type = field.Split(" ")[0];
            string name = field.Split(" ")[1];
            string propName = char.ToUpper(name[0]) + name.Substring(1);
            
            writer.WriteLine($"          public {type} {propName} {{ get; }}");
        }
        writer.WriteLine("");
        writer.WriteLine("          public override R Accept<R>(IVisitor<R> visitor)");
        writer.WriteLine("          {");
        writer.WriteLine($"              return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("          }");
        
        writer.WriteLine("      }");
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("      public interface IVisitor<R>");
        writer.WriteLine("      {");
        foreach (string type in types)
        {
            string typeName = type.Split(':')[0].Trim();
            writer.WriteLine($"         R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }
        writer.WriteLine("      }");
    }
}