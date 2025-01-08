namespace MUCollections.Common.Trees.RangeTrees
{
	/// <summary>
	/// A read-only structure that can return all values in the given range. </br>
	/// Leaf nodes will contain (TKey, TValue). All leaf nodes will be connected into a single-linked-list.
	/// Structure of a mid node is:
	///     A
	///   / | \
	///  L  M  R
	///  
	/// Where L &lt A &lt R. L will contain all keys less then A. R will contain all keys greater than A. And M is a list containing all keys equal to A.
	/// </summary>
	public class RangeTree1D<TKey, TValue>
	{
		private readonly IComparer<TKey> _comparer = Comparer<TKey>.Default;

		private readonly Node? _root;

		public RangeTree1D(IEnumerable<(TKey, TValue)> values)
		{
			var list = new LinkedList<(TKey key, TValue value)>(values);
			
			if (list.Count == 0)
			{
				return;
			}
			var queue = new Queue<Node>();
			Node? prev = null;
			for (var cur = list.First; cur != null; cur = cur.Next)
			{
				if (prev == null)
				{
					prev = new Node
					{
						Key = cur.Value.key,
						Leaf = cur
					};
					queue.Enqueue(prev);
					continue;
				}
				if (_comparer.Compare(prev.Key, cur.Value.key) == 0)
				{
					continue;
				}
				prev = new Node
				{
					Key = cur.Value.key,
					Leaf = cur
				};
				queue.Enqueue(prev);
			}

			while (queue.Count == 1)
			{
				var left = queue.Dequeue();
				var t = queue.Dequeue();
				var right = (queue.Count > 0) ? queue.Dequeue() : null;
				var node = new Node
				{
					Key = t.Key,
					Leaf = t.Leaf,
					Left = left,
					Right = right
				};
			}
		}


		/// <summary>
		/// Returns all values in the given closed range. </br>
		/// </summary>
		/// <param name="from"> Range start included. </param>
		/// <param name="to"> Range end included. </param>
		/// <returns></returns>
		public IEnumerable<(TKey key, TValue value)> GetValues(TKey from, TKey to)
		{
			yield break;
		}

		internal class Node
		{
			public required TKey Key { get; init; }
			public required LinkedListNode<(TKey key, TValue value)> Leaf { get; init; }

			public Node? Left { get; init; }
			public Node? Right { get; init; }
		}
	}
}
