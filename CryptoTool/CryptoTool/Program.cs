namespace CryptoTool
{
	class Program
	{
		static void Main(string[] args)
		{
			CryptoTool cryptoTool = new CryptoTool();

			for (int i = 1; i < args.Length; i++)
			{
				if (args[i] == "--password" || args[i] == "-p")
				{
					cryptoTool.Password = args[++i];
					continue;
				}

				if (i == args.Length - 2)
				{
					cryptoTool.InputDirectoryPath = args[i];
					continue;
				}

				if (i == args.Length - 1)
				{
					cryptoTool.OutputDirectoryPath = args[i];
					continue;
				}
			}

			if (args[0] == "encrypt")
			{
				// Example: dotnet CryptoTool.dll encrypt --password "geheim123" "../../../../../ExampleFolderToEncrypt" "../../../../../ExampleFolderEncrypted"
				cryptoTool.Encrypt();
			}
			else if (args[0] == "decrypt")
			{
				// Example: dotnet CryptoTool.dll decrypt --password "geheim123" "../../../../../ExampleFolderEncrypted" "../../../../../ExampleFolderDecrypted"
				cryptoTool.Decrypt();
			}
		}
	}
}
