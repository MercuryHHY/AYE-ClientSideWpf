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


namespace Microsoft.Extensions.DataChannel.Manager.Extensions;

public static class ProviderSourceFactoryExtensions
{
    public static bool IsSupportprotocol(this IProviderSourceFactory providerSourceFactory, string protocol)
    {
        return providerSourceFactory.GetSupportprotocol().Contains(protocol);
    }

    public static bool IsSupportReadStrategy(this IProviderSourceFactory providerSourceFactory, string readStrategy)
    {
        return providerSourceFactory.GetSupportReadStrategy().Contains(readStrategy);
    }

    public static bool IsSupportWriterStrategy(this IProviderSourceFactory providerSourceFactory, string writerStrategy)
    {
        return providerSourceFactory.GetSupportWriterStrategy().Contains(writerStrategy);
    }
}
