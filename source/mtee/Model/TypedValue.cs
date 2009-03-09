﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitnesse.mtee.model {
    public struct TypedValue {
        public static readonly TypedValue Void = new TypedValue(null, typeof(void));

        public static TypedValue MakeInvalid(Exception exception) { return new TypedValue(exception, typeof(void)); }

        public object Value { get; private set; }
        public Type Type { get; private set; }

        public TypedValue(object value, Type type) : this() {
            Value = value;
            Type = type;
        }

        public TypedValue(object value): this(value, value != null ? value.GetType() : typeof(object)) {}

        public bool IsVoid { get { return Type == typeof (void) && Value == null; } }
        public bool IsValid { get { return Type != typeof (void) || Value == null; } }

        public void ThrowExceptionIfNotValid() {
            if (!IsValid) throw (Exception) Value;
        }
    }
}
