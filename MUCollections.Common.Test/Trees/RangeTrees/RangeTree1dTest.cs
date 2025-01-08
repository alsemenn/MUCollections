using MUCollections.Common.Trees.RangeTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUCollections.Common.Test.Trees.RangeTrees
{
	public class RangeTree1dTest
	{
		[Fact]
		public void Test_EmptyTree()
		{
			var tree = new RangeTree1D<int, int>(Array.Empty<(int, int)>());
			Assert.Empty(tree.GetValues(0, 1000));
		}

		[Fact]
		public void Test_Single()
		{
			var testObj = new RangeTree1D<int, int>(new[] { (1, 1) });
			Assert.Equal(new[] { (1, 1) }, testObj.GetValues(0, 100));
		}
	}
}
