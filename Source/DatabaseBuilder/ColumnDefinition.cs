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

namespace DatabaseBuilder
{
    public class ColumnDefinition
    {
        public string TableName    { get; set; }
        public string ColumnName   { get; set; }
        public Type   ColumnType   { get; set; }
        public int    Length       { get; set; }
        public int    Precision    { get; set; }
        public string DefaultValue { get; set; }
        public bool   Nullable     { get; set; }
        public bool   Unique       { get; set; }
        public bool   Identity     { get; set; }
        public int    Seed         { get; set; }
        public int    Increment    { get; set; }
        public bool   PrimaryKey   { get; set; }
        public int    Scale        { get; set; }
        public int    Order        { get; set; }
    }
}
