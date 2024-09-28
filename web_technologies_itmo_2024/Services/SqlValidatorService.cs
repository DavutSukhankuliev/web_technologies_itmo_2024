using Antlr4.Runtime;

namespace web_technologies_itmo_2024.Services;

public class SqlValidatorService
{
	public List<string> GetValidationErrors(string query)
	{
		var inputStream = new AntlrInputStream(query);
		var lexer = new PostgreSqlLexer(inputStream);
		var commonTokenStream = new CommonTokenStream(lexer);
		var parser = new PostgreSqlParser(commonTokenStream);
		parser.RemoveErrorListeners();
		var errorListener = new SqlErrorListener();
		parser.AddErrorListener(errorListener);

		parser.stmt_execsql();

		return errorListener.HasErrors ? errorListener.GetErrorMessages().ToList() : new List<string>();
	}

	private class SqlErrorListener : IAntlrErrorListener<IToken>
	{
		public bool HasErrors { get; private set; }
		private readonly List<string> _errorMessages = new List<string>();

		public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			HasErrors = true;
			_errorMessages.Add($"line {line}:{charPositionInLine} {msg}");
		}

		public IEnumerable<string> GetErrorMessages() => _errorMessages;
	}
}