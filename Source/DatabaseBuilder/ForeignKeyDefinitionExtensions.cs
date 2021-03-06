﻿// MIT License
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
using DatabaseBuilder.Attributes;

namespace DatabaseBuilder
{
    public static class ForeignKeyDefinitionExtensions
    {
        public static string ToScript(this ForeignKeyDefinition definition, string constraint)
        {
            string GetAction(Actions onUpdate)
            {
                switch (onUpdate)
                {
                    case Actions.Cascade:    return "CASCADE";
                    case Actions.SetNull:    return "SET NULL";
                    case Actions.SetDefault: return "SET DEFAULT";
                    case Actions.NoAction:   return "NO ACTION";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(onUpdate), onUpdate, null);
                }
            }

            return $"CONSTRAINT fk_{constraint}_{string.Join("_", definition.FieldNames)} " +
                   $"FOREIGN KEY ([{string.Join("],[", definition.FieldNames)}]) " +
                   $"REFERENCES [{definition.Schema}].[{definition.Table}] ([{string.Join("],[", definition.ReferenceFields)}]) " +
                   $"ON UPDATE {GetAction(definition.OnUpdate)} ON DELETE {GetAction(definition.OnDelete)}";
        }
    }
}
