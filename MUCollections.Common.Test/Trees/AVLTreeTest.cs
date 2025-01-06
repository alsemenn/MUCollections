using MUCollections.Common.Trees;
using MUCollections.Common.Test.TestUtils;

namespace MUCollections.Common.Test.Trees
{
	public class AVLTreeTest : IDisposable
	{
		private AVLTree<int, int> _testObj = new AVLTree<int, int>();

		public void Dispose()
		{
			TreeTestUtils.ValidateTreeParentRefs(_testObj._root, i => i.Top, i => i.Left, i => i.Right);
			TestHeights(_testObj._root);
		}


		[Fact]
		public void Test_Empty()
		{
			Assert.Empty(_testObj);
		}

		[Fact]
		public void Test_AddSingle()
		{
			_testObj.Add(1, 1);
			Assert.Single(_testObj);
			Assert.Equal(1, _testObj[1]);
		}

		[Theory]
		[InlineData(new int[] {1, 2})]
		[InlineData(new int[] { 1, 2, 3 })]
		[InlineData(new[] {1, 2, 3, 4, 5, 6, 7})]
		public void Test_AddMultipleKeys(int[] keys)
		{
			foreach (var key in keys)
			{
				_testObj.Add(key, key);
			}
			var expected = keys.Distinct().OrderBy(i => i).ToArray();
			Assert.Equal(expected.Length, _testObj.Count);
			Assert.Equal(expected, _testObj.Keys);
			Assert.Equal(expected, _testObj.Values);
			foreach (var key in expected)
			{
				Assert.Equal(key, _testObj[key]);
			}
		}

		private void TestHeights<T, K>(AVLTree<T, K>.Node? node)
		{
			if (node == null)
			{
				return;
			}

			int left = GetHeight(node.Left);
			int right = GetHeight(node.Right);
			Assert.True(Math.Abs(left - right) <= 1);
			Assert.Equal(1 + Math.Max(left, right), node.Height);
			TestHeights(node.Left);
			TestHeights(node.Right);
		}

		private int GetHeight<T, K>(AVLTree<T, K>.Node? node)
		{
			if (node == null)
			{
				return 0;
			}
			return node.Height;
		}
	}
}
