// MIT License
// 
// Copyright (c)  Thomas Due
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

using DatabaseBuilder.Attributes;

namespace TestApp.Definitions
{
    [TableDefinition]
    public class Order : CommonTableTraits
    {
        [ColumnDefinition(Name = "Item", Length = 20, Nullable = false, IsUnique = true)]
        public string Code { get; set; }

        [ColumnDefinition(Length = 80)]
        public string Name { get; set; }

        [ColumnDefinition(DefaultValue = "1")]
        public bool Active { get; set; }

        [ColumnDefinition]
        [ForeignKeyDefinition(Table = "Article", Field = "Id", OnUpdate = Actions.Cascade, OnDelete = Actions.Cascade)]
        public int ArticleId { get; set; }
    }
}
