using MUCollections.Common.Trees;

namespace MUCollections.Common.Test.Trees
{
	public class AVLTreeTest
	{
		[Fact]
		public void Test_Empty()
		{
			var obj = new AVLTree<int, int>();
			Assert.Empty(obj);
		}
	}
}
