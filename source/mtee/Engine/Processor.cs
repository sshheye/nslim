﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitnesse.mtee.model;
using fitnesse.mtee.Model;
using fitnesse.mtee.operators;

namespace fitnesse.mtee.engine {
    public class Processor: Copyable { //todo: add setup and teardown
        private readonly List<List<Operator>> operators = new List<List<Operator>>();
        public ApplicationUnderTest ApplicationUnderTest { get; set; }

        public Processor(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
            AddOperator(new DefaultParse());
            AddOperator(new ParseType());
            AddOperator(new DefaultRuntime());
            AddOperator(new DefaultMemory());
        }

        public Processor(): this(new ApplicationUnderTest()) {}

        public Processor(Processor other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            operators.Clear();
            foreach (List<Operator> list in other.operators) {
                operators.Add(new List<Operator>(list));
            }
        }

        public void AddOperator(string operatorName) {
            AddOperator((Operator)Create(operatorName));
        }

        public void AddOperator(Operator anOperator) { AddOperator(anOperator, 0); }

        public void AddOperator(Operator anOperator, int priority) {
            while (operators.Count <= priority) operators.Add(new List<Operator>());
            operators[priority].Add(anOperator);
        }

        public void RemoveOperator(string operatorName) {
            foreach (List<Operator> list in operators)
                foreach (Operator item in list)
                    if (item.GetType().FullName == operatorName) {
                        list.Remove(item);
                        return;
                    }
        }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public object Execute(Tree<object> input) {
            var state = State.MakeTree(input);
            return FindOperator<ExecuteOperator>(state).Execute(this, state);
        }

        public object ParseTree(Type type, Tree<object> input) {
            var state = State.MakeTree(type, input);
            return FindOperator<ParseOperator>(state).Parse(this, state);
        }

        public object Parse(Type type, object input) {
            var state = State.MakeParameter(type, input);
            return FindOperator<ParseOperator>(state).Parse(this, state);
        }

        public T ParseTree<T>(Tree<object> input) {
            return (T) ParseTree(typeof (T), input);
        }

        public T Parse<T>(object input) {
            return (T) Parse(typeof (T), input);
        }

        public object Compose(object result, Type type) {
            var state = State.MakeInstance(result, type);
            return FindOperator<ComposeOperator>(state).Compose(this, state);
        }

        public TypedValue Invoke(object instance, string member, Tree<object> parameters) {
            var state = new State(instance, instance.GetType(), member, parameters);
            return FindOperator<RuntimeOperator>(state).Invoke(this, state);
        }

        public object Create(string typeName, Tree<object> parameters) {
            var state = State.MakeNew(typeName, parameters);
            return FindOperator<RuntimeOperator>(state).Create(this, state);
        }

        public object Create(string typeName) {
            return Create(typeName, new TreeList<object>());
        }

        public void Store(string variableName, object instance) {
            var state = State.MakeInstance(instance, variableName);
            FindOperator<MemoryOperator>(state).Store(this, state);
        }

        public object Load(string variableName) {
            var state = State.MakeName(variableName);
            return FindOperator<MemoryOperator>(state).Load(this, state);
        }

        public bool Contains(string variableName) {
            var state = State.MakeName(variableName);
            return FindOperator<MemoryOperator>(state).Contains(this, state);
        }

        private T FindOperator<T> (State state) where T: class, Operator{
            for (int priority = operators.Count - 1; priority >= 0; priority--) {
                for (int i = operators[priority].Count - 1; i >= 0; i--) {
                    var candidate = operators[priority][i] as T;
                    if (candidate != null && candidate.IsMatch(this, state)) {
                        return candidate;
                    }
                }
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }

        Copyable Copyable.Copy() {
            return new Processor(this);
        }
    }
}