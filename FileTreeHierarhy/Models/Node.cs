using System.Collections.Generic;

namespace FileTreeHierarhy.Models
{
	public class Node
	{
		public string Name { get; set; }
		public List<Node> Children { get; set; } = new();
	}
}