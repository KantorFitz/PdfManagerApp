using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
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

		public static List<string> TreeRows { get; } = new();
		
		public static void PrintNode(Node node, string indent)
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

		public static Node BuildSegments(string path, bool isLast = false)
		{
			if (isLast)
				return null;
			
			var segments = path.Split('\\').ToList();
			var element = segments.First();
			
			if (path == segments.Last()) isLast = true;
			segments.RemoveAt(0);


			return new Node
			{
				Name = element,
				Children =
				{
					BuildSegments(Path.Combine(segments.ToArray()), isLast)
				}
			};
		}

		public static void AppendHierarhy(Node node, string path, bool islast = false)
		{
			if (islast)
				return;
			
			var segments = path.Split("\\").ToList();

			if (node.Children.Any(x=>x?.Name == segments[1]))
			{
				foreach (var nodeChild in node?.Children)
				{
					AppendHierarhy(nodeChild, Path.Combine(segments.Skip(1).ToArray()), segments.Count == 2);
				}
			}
			else
			{
				node.Children.Add(BuildSegments(Path.Combine(segments.Skip(1).ToArray())));
			}
			
			
			
			

			// if (segments[0] == node.Name)
			// {
			// 	if (segments.Count == 1) islast = true;
			// 	var thisNode = node.Children.SingleOrDefault(x => x.Name == segments[0]);
			// 	
			// 	AppendHierarhy(thisNode, Path.Combine(segments.Skip(1).ToArray()), islast);
			// }
			// else
			// {
			// 	if (node.Children.Any(x=>x.Name == segments[0]))
			// 		AppendHierarhy(node, Path.Combine(segments.Skip(1).ToArray()), islast);
			// 	node.Children.Add(BuildSegments(Path.Combine(segments.ToArray())));
			// }
		}
		
		public static IEnumerable<Node> ToListOfNodes(IEnumerable<string> paths)
		{
			return paths.Select(x => BuildSegments(x)).ToList();
		}
	}
}