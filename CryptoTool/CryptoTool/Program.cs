using System;

namespace CryptoTool
{
	static class Program
	{
		static void Main(string[] args)
		{
			CryptoTool cryptoTool = new CryptoTool();

			for (int i = 1; i < args.Length; i++)
			{
				if (args[i - 1] == "--password" || args[i - 1] == "-p")
				{
					cryptoTool.Password = args[i];
					continue;
				}

				if (args[i] == "--replace" || args[i] == "-r")
				{
					cryptoTool.Replace = true;
					continue;
				}

				if (args[i - 1] == "--input" || args[i - 1] == "-i")
				{
					cryptoTool.InputDirectoryPath = args[i];
					continue;
				}

				if (args[i - 1] == "--output" || args[i - 1] == "-o")
				{
					cryptoTool.OutputDirectoryPath = args[i];
					continue;
				}
			}

			if (args.Length != 0)
			{
				if (args[0] == "encrypt")
				{
					cryptoTool.Encrypt();
				}
				else if (args[0] == "decrypt")
				{
					cryptoTool.Decrypt();
				}
				else if (args[0] == "--help")
				{
					Logger.LogHelp();
				}
				else
				{
					Logger.LogMessages($"Unknown command: {args[0]}");
					Logger.LogMessages($"See 'cryptotool --help'");
				}
			}
			else
			{
				Logger.LogHelp();
			}
		}
	}
}
