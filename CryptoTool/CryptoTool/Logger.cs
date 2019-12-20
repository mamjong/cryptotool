using System;

namespace CryptoTool
{
	static class Logger
	{
		internal static void LogMessages(params string[] messages)
		{
			foreach (string message in messages)
			{
				Console.WriteLine(message);
			}
		}

		internal static void LogMessages(ConsoleColor foregroundColor, params string[] messages)
		{
			Console.ForegroundColor = foregroundColor;
			foreach (string message in messages)
			{
				Console.WriteLine(message);
			}
			Console.ResetColor();
		}

		internal static void LogHelp()
		{
			Console.WriteLine();
			Console.WriteLine("Usage: cryptotool [COMMAND] [OPTIONS]");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			Console.WriteLine("\tencrypt\t\tEncrypt one or multiple files");
			Console.WriteLine("\tdecrypt\t\tDecrypt one or multiple encrypted files");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("\t-p,\t--password string\tSet a password used for encryption/decryption");
			Console.WriteLine("\t-r,\t--replace\t\tReplace plaintext files for encryptions / encryptions for decryptions");
			Console.WriteLine("\t-i,\t--input path\t\tSet the execution path for encryption/decryption");
			Console.WriteLine("\t-o,\t--output dir\t\tSet the directory for encryption/decryption output");
		}
	}
}
