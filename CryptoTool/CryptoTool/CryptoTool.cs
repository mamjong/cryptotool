using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoTool
{
	public class CryptoTool
	{
		private string _outputDirectoryPath;

		public string InputDirectoryPath { get; set; } = Environment.CurrentDirectory;
		public string OutputDirectoryPath { get => _outputDirectoryPath ?? InputDirectoryPath; set => _outputDirectoryPath = value; }
		public string Password { private get; set; } = "";
		public bool Replace { get; set; } = false;

		public void Encrypt()
		{
			if (!IsAccessible()) return;
			string[] filePaths = GetFilePaths(InputDirectoryPath);

			foreach (string inputFilePath in filePaths)
			{
				// Initialise Rijndael
				RandomNumberGenerator rng = RandomNumberGenerator.Create();

				byte[] salt = new byte[16];
				byte[] IV = new byte[16];

				rng.GetBytes(salt);
				rng.GetBytes(IV);

				Rijndael rijndael = new RijndaelManaged
				{
					IV = IV
				};

				rijndael.Key = new Rfc2898DeriveBytes(Password, salt).GetBytes(rijndael.KeySize / 8);

				// Create output file (and dirs)
				string outputFilePath = Path.Combine(OutputDirectoryPath, Path.GetRelativePath(InputDirectoryPath, inputFilePath)) + ".encrypted";

				Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

				// Open stream to input
				using (FileStream inputFileStream = File.OpenRead(inputFilePath))
				{

					// Open stream to output with encryptor
					using FileStream outputFileStream = File.Create(outputFilePath);

					using CryptoStream encryptionStream = new CryptoStream(
						outputFileStream,
						rijndael.CreateEncryptor(),
						CryptoStreamMode.Write
					);

					// Write plaintext salt and IV to output
					outputFileStream.Write(salt);
					outputFileStream.Write(IV);

					// Read plaintext from input
					byte[] inputFileContent = new byte[inputFileStream.Length];
					inputFileStream.Read(inputFileContent);

					// Write ciphertext to output
					encryptionStream.Write(inputFileContent);
				}

				// Delete input file if necessary
				if (Replace) File.Delete(inputFilePath);

				Logger.LogMessages(ConsoleColor.Green, $"Successfully encrypted \"{Path.GetFileName(inputFilePath)}\"");
			}
		}

		public void Decrypt()
		{
			if (!IsAccessible()) return;
			string[] filePaths = GetFilePaths(InputDirectoryPath, "*.encrypted");

			foreach (string inputFilePath in filePaths)
			{
				// Open stream to input
				using (FileStream inputFileStream = File.OpenRead(inputFilePath))
				{

					// Read plaintext salt and IV from input
					byte[] salt = new byte[16];
					byte[] IV = new byte[16];

					inputFileStream.Read(salt);
					inputFileStream.Read(IV);

					// Initialise Rijndael using salt and IV
					Rijndael rijndael = new RijndaelManaged
					{
						IV = IV
					};

					rijndael.Key = new Rfc2898DeriveBytes(Password, salt).GetBytes(rijndael.KeySize / 8);

					// Create and add decryptor to input stream
					using CryptoStream decryptionStream = new CryptoStream(
						inputFileStream,
						rijndael.CreateDecryptor(),
						CryptoStreamMode.Read
					);

					// Create output file (with dirs)
					string outputFilePath = Path.Combine(OutputDirectoryPath, Path.GetRelativePath(InputDirectoryPath, Path.ChangeExtension(inputFilePath, null)));

					Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

					// Open stream to output
					using FileStream outputFileStream = File.Create(outputFilePath);

					// Read/Write with decryption
					try
					{
						byte[] inputDecryptedContent = new byte[inputFileStream.Length - 32];
						decryptionStream.Read(inputDecryptedContent);
						outputFileStream.Write(inputDecryptedContent);
						Logger.LogMessages(ConsoleColor.Green, $"Successfully decrypted \"{Path.GetFileName(inputFilePath)}\"");
					}
					catch (CryptographicException)
					{
						Logger.LogMessages(ConsoleColor.Red, $"Failed decrypting \"{Path.GetFileName(inputFilePath)}\"");
					}
				}

				if (Replace) File.Delete(inputFilePath);
			}
		}

		private bool IsAccessible()
		{
			if (Directory.Exists(InputDirectoryPath) || File.Exists(InputDirectoryPath))
			{
				return true;
			}
			else
			{
				Logger.LogMessages(ConsoleColor.Red, "The folder marked for encryption is inaccessible");
				return false;
			}
		}

		private string[] GetFilePaths(string rootPath, string filter = "*")
		{
			FileAttributes fileAttributes = File.GetAttributes(rootPath);

			if (fileAttributes.HasFlag(FileAttributes.Directory))
			{
				return Directory.GetFiles(rootPath, filter, SearchOption.AllDirectories);
			}
			else
			{
				InputDirectoryPath = Path.GetDirectoryName(rootPath);
				return new string[] { rootPath };
			}
		}
	}
}
