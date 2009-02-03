﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.Text;
using fitnesse.mtee.model;

namespace fitnesse.slim {
    public class Document {
        public Document(Tree<string> content) {
            Content = content;
        }

        public Tree<string> Content { get; private set; }

        public override string ToString() { return Content.Serialize(new SlimWriter()).ToString(); }

        private class SlimWriter: TreeWriter<string> {
            private readonly StringBuilder output = new StringBuilder();

            public void WritePrefix(Tree<string> tree) {
                if (!tree.IsLeaf) output.AppendFormat("[{0:000000}:", tree.Branches.Count);
                else output.Append(tree.Value == null ? "null" : tree.Value.ToString());
            }

            public void WriteBranch(Tree<string> tree, int index) {
                string item = tree.Serialize(new SlimWriter()).ToString();
                output.AppendFormat("{0:000000}:{1}:", item.Length, item);
            }

            public void WriteSuffix(Tree<string> tree) {
                if (!tree.IsLeaf) output.Append(']');
            }

            public override string ToString() { return output.ToString(); }
        }

        public static Document Parse(string input) { return new Document(Read(input)); }

        private static Tree<string> Read(string input) {
            if (input.StartsWith("[") && input.EndsWith("]")) return ReadList(input.Substring(1, input.Length - 2));
            return new TreeLeaf<string>(input);
        }

        private static TreeList<string> ReadList(string input) {
            int length = int.Parse(input.Substring(0, 6));
            var result = new TreeList<string>();
            int start = 7;
            for (int i = 0; i < length; i++) {
                int itemLength = int.Parse(input.Substring(start, 6));
                start += 7;
                result.AddBranch(Read(input.Substring(start, itemLength)));
                start += itemLength + 1; 
            }
            return result;
        }
    }
}
