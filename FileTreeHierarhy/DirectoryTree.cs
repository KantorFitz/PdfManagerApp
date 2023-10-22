using System;
using System.IO;

namespace FileTreeHierarhy
{
	public class DirectoryTree
	{
		public static void BuildDirectoryTree(string directory, int indentation)
		{
			try
			{
				var files = Directory.GetFiles(directory);
				var subDirectories = Directory.GetDirectories(directory);

				var totalSize = 0.0;

				foreach (var file in files)
				{
					var fileInfo = new FileInfo(file);
					totalSize += fileInfo.Length;

					var fileString = new string(' ', indentation + 2) + fileInfo.Name + " (" + fileInfo.Length +
					                 " bytes)";
					
					File.AppendAllLines(@"C:\Users\Kantor\Desktop\dupa.txt", new []{fileString});

					Console.WriteLine(fileString);
				}

				var dirString = new string(' ', indentation) + Path.GetFileName(directory) + " (" + files.Length +
				                " files, " + totalSize + " bytes)";
				
				File.AppendAllLines(@"C:\Users\Kantor\Desktop\dupa.txt", new []{dirString});

				Console.WriteLine(dirString);
				foreach (var subDirectory in subDirectories)
				{
					BuildDirectoryTree(subDirectory, indentation + 2);
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				throw;
			}
		}
	}
}