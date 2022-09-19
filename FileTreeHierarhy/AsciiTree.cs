using System;
using System.Collections.Generic;
using FileTreeHierarhy.Models;

namespace FileTreeHierarhy
{
	public static class AsciiTree
	{
		// Constants for drawing lines and spaces
		private const string _cross = " |-";//" ├─";
		private const string _corner = " L";//" └─";
		private const string _vertical = " | ";//" │ ";
		private const string _space = "   ";

		public static List<string> TreeRows { get; set; } = new();
		
		public static List<string> PrintNode(Node node, string indent)
		{
			TreeRows[^1] += node.Name;
			TreeRows.Add("");

			Console.WriteLine(node.Name);

			// Loop through the children recursively, passing in the
			// indent, and the isLast parameter
			var numberOfChildren = node.Children.Count;
			for (var i = 0; i < numberOfChildren; i++)
			{
				var child = node.Children[i];
				var isLast = (i == (numberOfChildren - 1));
				PrintChildNode(child, indent, isLast);
			}

			return TreeRows;
		}

		private static void PrintChildNode(Node node, string indent, bool isLast)
		{
			// Print the provided pipes/spaces indent
			TreeRows[^1] += indent;
			Console.Write(indent);

			// Depending if this node is a last child, print the
			// corner or cross, and calculate the indent that will
			// be passed to its children
			if (isLast)
			{
				TreeRows[^1] += _corner;
				Console.Write(_corner);
				indent += _space;
			}
			else
			{
				TreeRows[^1] += _cross;
				Console.Write(_cross);
				indent += _vertical;
			}

			PrintNode(node, indent);
		}
	}
}