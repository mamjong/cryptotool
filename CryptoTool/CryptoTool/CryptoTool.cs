using System;
using System.IO;
using System.Linq;
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

				string outputFilePath = Path.Combine(OutputDirectoryPath, Path.GetRelativePath(InputDirectoryPath, inputFilePath)) + ".encrypted";

				Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

				using (FileStream inputFileStream = File.OpenRead(inputFilePath))
				{
					using FileStream outputFileStream = File.Create(outputFilePath);

					using CryptoStream encryptionStream = new CryptoStream(
						outputFileStream,
						rijndael.CreateEncryptor(),
						CryptoStreamMode.Write
					);

					outputFileStream.Write(salt);
					outputFileStream.Write(IV);

					byte[] inputFileContent = new byte[inputFileStream.Length];
					inputFileStream.Read(inputFileContent);

					encryptionStream.Write(inputFileContent);
				}

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
				byte[] salt;
				byte[] IV;
				byte[] encryptedContent;

				using (FileStream inputFileStream = File.OpenRead(inputFilePath))
				{
					salt = new byte[16];
					IV = new byte[16];
					encryptedContent = new byte[inputFileStream.Length - 32];

					inputFileStream.Read(salt);
					inputFileStream.Read(IV);
					inputFileStream.Read(encryptedContent);

				}

				Rijndael rijndael = new RijndaelManaged
				{
					IV = IV
				};

				rijndael.Key = new Rfc2898DeriveBytes(Password, salt).GetBytes(rijndael.KeySize / 8);


				using MemoryStream encryptedContentStream = new MemoryStream(encryptedContent);
				using CryptoStream decryptionStream = new CryptoStream(
					encryptedContentStream,
					rijndael.CreateDecryptor(),
					CryptoStreamMode.Read
				);

				string outputFilePath = Path.Combine(OutputDirectoryPath, Path.GetRelativePath(InputDirectoryPath, Path.ChangeExtension(inputFilePath, null)));

				Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));


				using FileStream outputFileStream = File.Create(outputFilePath);


				try
				{
					byte[] decryptedContent = new byte[encryptedContentStream.Length];
					int bytesRead = decryptionStream.Read(decryptedContent);
					outputFileStream.Write(decryptedContent.Take(bytesRead).ToArray());
					if (Replace) File.Delete(inputFilePath);
					Logger.LogMessages(ConsoleColor.Green, $"Successfully decrypted \"{Path.GetFileName(inputFilePath)}\"");
				}
				catch (CryptographicException)
				{
					Logger.LogMessages(ConsoleColor.Red, $"Failed decrypting \"{Path.GetFileName(inputFilePath)}\"");
				}
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
