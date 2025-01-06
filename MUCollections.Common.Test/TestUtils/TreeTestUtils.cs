using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUCollections.Common.Test.TestUtils
{
	internal static class TreeTestUtils
	{
		/// <summary>
		///	Checks that each node has a correct parent reference.
		/// </summary>
		public static void ValidateTreeParentRefs<T> (T? r, Func<T, T?> top, Func<T, T?> left, Func<T, T?> right)
		{
			if (r == null)
			{
				return;
			}

			if (left(r) != null)
			{
				Assert.Equal(r, top(left(r)!));
				ValidateTreeParentRefs(left(r)!, top, left, right);
			}

			if (right(r) != null)
			{
				Assert.Equal(r, top(right(r)!));
				ValidateTreeParentRefs(right(r)!, top, left, right);
			}
		}
	}
}
