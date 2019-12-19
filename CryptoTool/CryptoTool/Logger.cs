using System;

namespace CryptoTool
{
	static class Logger
	{
		internal static void LogMessages(ConsoleColor color, params string[] messages)
		{
			Console.ForegroundColor = color;
			foreach (string message in messages)
			{
				Console.WriteLine(message);
			}
			Console.ResetColor();
		}

		internal static void LogHelp()
		{
			Console.WriteLine();
			Console.WriteLine("Usage: cryptotool <encrypt/decrypt> [OPTIONS]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine($"-p,\t--password string\tUse given password for encryption/decryption");
			Console.WriteLine($"-r,\t--replace\t\tReplace encrypted/decrypted files instead of creating new files");
			Console.WriteLine($"-i,\t--input path\t\tSpecify path to encrypt/decrypt");
			Console.WriteLine($"-o,\t--output path\t\tSpecify path for encryption/decryption output");
		}
	}
}
