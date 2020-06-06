namespace Travelling
{
    using System.Collections.Generic;
    internal static class Distances
    {
        internal static Dictionary<string, int> Matrix =
            new Dictionary<string, int>
            {
                ["1->2"] = 4,
                ["1->3"] = 3,
                ["1->4"] = 6,
                ["1->5"] = 5,
                ["2->1"] = 4,
                ["2->3"] = 5,
                ["2->4"] = 7,
                ["2->5"] = 9,
                ["3->1"] = 3,
                ["3->2"] = 5,
                ["3->4"] = 3,
                ["3->5"] = 4,
                ["4->1"] = 6,
                ["4->2"] = 7,
                ["4->3"] = 3,
                ["4->5"] = 5,
                ["5->1"] = 5,
                ["5->2"] = 9,
                ["5->3"] = 4,
                ["5->4"] = 5,
            };

        internal static int From(ushort senderId, ushort id)
        {
            var key = senderId + "->" + id;
            return Matrix[key];
        }
    }
}
