using System.Collections.Generic;
using System.Windows;
using FileTreeHierarhy.Models;

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
			var newNode = new List<Node>()
			{
				new Node()
				{
					Name = "dupa",
					Children =
					{
						new Node()
						{
							Name = "asdasdsd jasiu",
						}
					}
				}
			};

			var cont = new List<string>();
			foreach (var node in newNode)
			{
				cont = AsciiTree.PrintNode(node, "");
			}
		}
	}
}