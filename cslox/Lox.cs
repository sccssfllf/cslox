namespace cslox;

public class Lox
{
    static bool hadError = false;
    
    public static void RunFile(string path)
    {
        string source = File.ReadAllText(path);
        Run(source);

        if (hadError) Environment.Exit(65);
    }

    public static void RunPrompt()
    {
        for (;;)
        {
            Console.Write("> ");
            string line = Console.ReadLine();
            if (line == null) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        // List all tokens
        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line: {line} ] Error {where}: {message}");

        hadError = true;
    }
}