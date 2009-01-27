﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitnesse.mtee.engine;
using fitnesse.mtee.model;
using NUnit.Framework;

namespace fitnesse.unitTest.engine {
    [TestFixture] public class SystemUnderTestTest {
        private SystemUnderTest systemUnderTest;

        [SetUp] public void SetUp() {
            systemUnderTest = new SystemUnderTest();
        }

        [Test] public void TypeIsFoundInCurrentAssembly() {
            RuntimeType sample = GetType("fitnesse.unitTest.engine.SampleClass");
            Assert.AreEqual(typeof(SampleClass), sample.Type);
        }

        private RuntimeType GetType(string name) {
            return systemUnderTest.FindType(new IdentifierName(name));
        }

        [Test] public void TypeWithoutNamespaceIsFound() {
            RuntimeType sample = GetType("AnotherSampleClass");
            Assert.AreEqual(typeof(AnotherSampleClass), sample.Type);
        }

        [Test] public void TypeIsFoundUsingNamespaces() {
            systemUnderTest.AddNamespace("fitnesse.unitTest.engine");
            RuntimeType sample = GetType("SampleClass");
            Assert.AreEqual(typeof(SampleClass), sample.Type);
        }

        [Test] public void NamespaceIsTrimmed() {
            systemUnderTest.AddNamespace(" fitnesse.unitTest.engine\n");
            RuntimeType sample = GetType("SampleClass");
            Assert.AreEqual(typeof(SampleClass), sample.Type);
        }

        [Test] public void TypeIsFoundInLoadedAssembly() {
            systemUnderTest.AddAssembly("sample.dll");
            RuntimeType sample = GetType("fitnesse.sample.SampleDomain");
            Assert.AreEqual("fitnesse.sample.SampleDomain", sample.Type.FullName);
        }

        [Test] public void ReloadingAssemblyIsIgnored() {
            systemUnderTest.AddAssembly("sample.dll");
            systemUnderTest.AddAssembly("sample.dll");
            RuntimeType sample = GetType("fitnesse.sample.SampleDomain");
            Assert.AreEqual("fitnesse.sample.SampleDomain", sample.Type.FullName);
        }

        [Test] public void TypeIsFoundInDefaultNamespace() {
            RuntimeType sample = systemUnderTest.FindType(new IdentifierName(typeof(SystemUnderTest).Name));
            Assert.AreEqual(typeof(SystemUnderTest), sample.Type);
        }
    }

}
public class AnotherSampleClass {}
