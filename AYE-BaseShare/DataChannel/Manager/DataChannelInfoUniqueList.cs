#region << License >>
// MIT License
// 
// 2024 - 上位机软件
//
// Copyright (c) @ Daniel大妞（guanhu）. All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

namespace Microsoft.Extensions.DataChannel.Manager;

public partial class DataChannelInfoUniqueList : UniqueList<string, DataChannelInfo>
{
    public DataChannelInfoUniqueList()
        : base(x => x.Code, StringComparer.InvariantCultureIgnoreCase)
    {
    }
}

public static partial class DataChannelInfoUniqueListExtensions
{
    public static void ThrowIfDuplicateCode(this IEnumerable<DataChannelInfo> items)
    {
        if (items.HasDuplicate(x => x.Code, out IEnumerable<string> keys))
        {
            throw new InvalidOperationException($"编码重复出现 ：{keys.Select(x => x.ToString()).ToList().MergeStrings(", ")} ");
        }
    }

    public static DataChannelInfoUniqueList AsUniqueList(this IEnumerable<DataChannelInfo> items)
    {
        items.ThrowIfDuplicateCode();
        DataChannelInfoUniqueList list = [];
        items.ForEach(x => list.Add(x));
        return list;
    }
}