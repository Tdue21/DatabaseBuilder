// MIT License
// 
// Copyright (c) 2020 Thomas Due
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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DatabaseBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ColumnDefinitionAttribute : Attribute
    {
        public ColumnDefinitionAttribute([CallerLineNumber] int order = 0) => Order = order;

        public string Name { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        [DefaultValue(false)] public bool PrimaryKey { get; set; }

        [DefaultValue(false)] public bool Identity { get; set; }

        [DefaultValue(1)] public int Seed { get; set; }

        [DefaultValue(1)] public int Increment { get; set; }

        [DefaultValue(true)] public bool Nullable { get; set; }

        [DefaultValue(1)] public int Version { get; set; }

        [DefaultValue(false)] public bool IsUnique { get; set; }

        public string DefaultValue { get; set; }

        public int Order { get; set; }
    }
}
