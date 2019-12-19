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

				if (args[i] == "--replace" || args[i] == "-r")
				{
					cryptoTool.Replace = true;
					continue;
				}

				if (args[i] == "--input" || args[i] == "-i")
				{
					cryptoTool.InputDirectoryPath = args[++i];
					continue;
				}

				if (args[i] == "--output" || args[i] == "-o")
				{
					cryptoTool.OutputDirectoryPath = args[++i];
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
