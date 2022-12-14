using System.IO;
using System.Linq;
using System.Windows;

namespace FileTreeHierarhy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var dirs = Directory.GetDirectories(@"D:\Programowanie\Workspace", "*",
				SearchOption.AllDirectories).ToList();
			//
			// var allFiles = new List<string>();
			// foreach (var dir in dirs)
			// {
			// 	allFiles.AddRange(Directory.GetFiles(dir));
			// 	allFiles.Add(dir);
			// }
			// var res = AsciiTree.ToListOfNodes(dirs);
			//
			// var result = AsciiTree.TreeRows;

			// var paths = new List<string>
			// {
			// 	// @"D:\1\",
			// 	// @"D:\1\2\3",
			// 	// @"D:\2\2\3",
			// 	@"D:\2\2\3\",
			// 	@"D:\1\2\3\4\5\6",
			// };

			var orderedPaths = dirs.OrderByDescending(x => x.Count(x => x == '\\'));

			var testPath0 = orderedPaths.First();

			var test1 = AsciiTree.BuildSegments(testPath0);
			foreach (var path in orderedPaths.Skip(1))
				AsciiTree.AppendHierarhy(test1, path);

			var i = 0;



		}

		
	}
}