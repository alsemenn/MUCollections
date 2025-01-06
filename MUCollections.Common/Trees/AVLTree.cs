using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace MUCollections.Common.Trees
{
	/// <summary>
	/// AVL tree. See https://en.wikipedia.org/wiki/AVL_tree for more info.
	/// </summary>
	/// <remarks>
	/// NOT THREAD SAFE.
	/// </remarks>
	public class AVLTree<TKey, TValue> : IDictionary<TKey, TValue?>
	{
		internal Node? _root;
		private readonly IComparer<TKey> _comparer = Comparer<TKey>.Default;

		public TValue? this[TKey key]
		{
			get
			{
				TValue? value;
				if (!TryGetValue(key, out value))
				{
					throw new KeyNotFoundException();
				}
				return value;
			}
			set
			{
				Add(key, value);
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				if (_root == null)
				{
					return Array.Empty<TKey>();
				}
				return InOrder(_root).Select(n => n.Key).ToArray();
			}
		}

		public ICollection<TValue?> Values
		{
			get
			{
				if (_root == null)
				{
					return Array.Empty<TValue>();
				}
				return InOrder(_root).Select(n => n.Value).ToArray();
			}
		}

		public int Count { get; private set; }

		public bool IsReadOnly => throw new NotImplementedException();

		public void Add(TKey key, TValue? value)
		{
			if (_root == null)
			{
				_root = new Node(key, value);
				Count = 1;
				return;
			}

			var (node, cmp) = FindNode(key, _root);
			if (cmp == 0)
			{
				node.Value = value;
				return;
			}

			if (cmp < 0)
			{
				node.Left = new Node(key, value) { Top = node };
				Balance(node);
			}
			else
			{
				node.Right = new Node(key, value) { Top = node };
				Balance(node);
			}
			++Count;
		}

		public void Add(KeyValuePair<TKey, TValue?> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			_root = null;
			Count = 0;
		}

		public bool Contains(KeyValuePair<TKey, TValue?> item)
		{
			if (_root == null)
			{
				return false;
			}

			var (node, cmp) = FindNode(item.Key, _root);
			if (cmp != 0)
			{
				return false;
			}

			return Object.Equals(node.Value, item.Value);
		}

		public bool ContainsKey(TKey key)
		{
			if (_root == null)
			{
				return false;
			}

			var (node, cmp) = FindNode(key, _root);
			return cmp == 0;
		}

		public void CopyTo(KeyValuePair<TKey, TValue?>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<TKey, TValue?>> GetEnumerator()
		{
			if (_root == null)
			{
				return Enumerable.Empty<KeyValuePair<TKey, TValue?>>().GetEnumerator();
			}

			return InOrder(_root).Select(n => new KeyValuePair<TKey, TValue?>(n.Key, n.Value!)).GetEnumerator();
		}

		public bool Remove(TKey key)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue?> item)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
		{
			if (_root == null)
			{
				value = default;
				return false;
			}
			var val = FindNode(key, _root);
			if (val.cmp != 0)
			{
				value = default;
				return false;
			}
			value = val.node.Value!;
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IEnumerable<Node> InOrder(Node node)
		{
			if (node.Left != null)
			{
				foreach (var n in InOrder(node.Left))
				{
					yield return n;
				}
			}
			yield return node;
			if (node.Right != null)
			{
				foreach (var n in InOrder(node.Right))
				{
					yield return n;
				}
			}
		}

		/// <returns>
		/// Node with the equal value if found. Or parent node where the new node should be added.<br/>
		/// cmp is the result of the comparison of the key with the node key.
		/// </returns>
		private (Node node, int cmp) FindNode(TKey key, Node node)
		{
			int val = _comparer.Compare(key, node.Key);
			if (val == 0)
			{
				return (node, 0);
			}
			if (val < 0)
			{
				if (node.Left == null)
				{
					return (node, -1);
				}
				return FindNode(key, node.Left);
			}

			if (node.Right == null)
			{
				return (node, 1);
			}
			return FindNode(key, node.Right);
		}

		private void Balance(Node node)
		{
			for (Node? cur = node; cur != null; cur = cur?.Top)
			{
				int diff = GetHeight(cur.Left) - GetHeight(cur.Right);
				cur.Height = Math.Max(GetHeight(cur.Left), GetHeight(cur.Right)) + 1;
				if (diff > 1)
				{
					// Left can't be null because it's hight is greater than right.
					if (GetHeight(cur.Left!.Left) < GetHeight(cur.Left.Right))
					{
						RotateLeft(cur.Left);
					}
					cur = RotateRight(cur);
				}
				else if (diff < -1)
				{
					if (GetHeight(cur.Right!.Right) < GetHeight(cur.Right.Left))
					{
						RotateRight(cur.Right);
					}
					cur = RotateLeft(cur);
				}
			}

			// We might need to update root here
			_root = _root?.Top ?? _root;
		}

		/// <summary>
		/// See https://www.geeksforgeeks.org/insertion-in-an-avl-tree/
		/// </summary>
		private Node? RotateRight(Node z)
		{
			Node y = z.Left!;
			Node? t3 = y.Right;

			// Move y.Right to z.Left
			z.Left = t3;
			if (t3 != null)
			{
				t3.Top = z;
			}

			// Move z to y.Right
			y.Right = z;
			if (z.Top != null)
			{
				if (z.Top.Left == z)
				{
					z.Top.Left = y;
				}
				else
				{
					z.Top.Right = y;
				}
			}
			y.Top = z.Top;
			z.Top = y;
			z.Height = Math.Max(GetHeight(z.Left), GetHeight(z.Right)) + 1;
			y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
			return y;
		}

		/// <summary>
		/// see https://www.geeksforgeeks.org/insertion-in-an-avl-tree/
		/// </summary>
		private Node? RotateLeft(Node z)
		{
			Node y = z.Right!;
			Node? t2 = y.Left;

			// Move y.Left to z.Right
			z.Right = t2;
			if (t2 != null)
			{
				t2.Top = z;
			}

			// Move z to y.Left
			y.Left = z;
			if (z.Top != null)
			{
				if (z.Top.Left == z)
				{
					z.Top.Left = y;
				}
				else
				{
					z.Top.Right = y;
				}
			}
			y.Top = z.Top;
			z.Top = y;
			z.Height = Math.Max(GetHeight(z.Left), GetHeight(z.Right)) + 1;
			y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
			return y;
		}

		private int GetHeight(Node? node)
		{
			return node?.Height ?? 0;
		}

		internal class Node
		{
			public Node(TKey key, TValue? value)
			{
				Key = key;
				Value = value;
				Height = 1;
			}
			public TKey Key { get; }
			public TValue? Value { get; set; }
			public Node? Left { get; set; }
			public Node? Right { get; set; }
			public Node? Top { get; set; }
			public int Height { get; set; }

			public override string ToString()
			{
				return $"Key: {Key}";
			}
		}
	}
}
