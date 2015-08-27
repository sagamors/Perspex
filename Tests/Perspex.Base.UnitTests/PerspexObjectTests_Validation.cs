﻿// -----------------------------------------------------------------------
// <copyright file="PerspexObjectTests_Validation.cs" company="Steven Kirk">
// Copyright 2015 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Base.UnitTests
{
    using System;
    using Xunit;

    public class PerspexObjectTests_Validation
    {
        [Fact]
        public void SetValue_Causes_Validation()
        {
            var target = new Class1();

            target.SetValue(Class1.QuxProperty, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => target.SetValue(Class1.QuxProperty, 25));
            Assert.Equal(5, target.GetValue(Class1.QuxProperty));
        }

        [Fact]
        public void SetValue_Causes_Coercion()
        {
            var target = new Class1();

            target.SetValue(Class1.QuxProperty, 5);
            Assert.Equal(5, target.GetValue(Class1.QuxProperty));
            target.SetValue(Class1.QuxProperty, -5);
            Assert.Equal(0, target.GetValue(Class1.QuxProperty));
            target.SetValue(Class1.QuxProperty, 15);
            Assert.Equal(10, target.GetValue(Class1.QuxProperty));
        }

        [Fact]
        public void Revalidate_Causes_Recoercion()
        {
            var target = new Class1();

            target.SetValue(Class1.QuxProperty, 7);
            Assert.Equal(7, target.GetValue(Class1.QuxProperty));
            target.MaxQux = 5;
            target.Revalidate(Class1.QuxProperty);
        }

        [Fact]
        public void Validation_Can_Be_Overridden()
        {
            var target = new Class2();
            Assert.Throws<ArgumentOutOfRangeException>(() => target.SetValue(Class1.QuxProperty, 5));
        }

        [Fact]
        public void Validation_Can_Be_Overridden_With_Null()
        {
            var target = new Class3();
            target.SetValue(Class1.QuxProperty, 50);
            Assert.Equal(50, target.GetValue(Class1.QuxProperty));
        }

        private class Class1 : PerspexObject
        {
            public static readonly PerspexProperty<int> QuxProperty =
                PerspexProperty.Register<Class1, int>("Qux", validate: Validate);

            public Class1()
            {
                this.MaxQux = 10;
                this.ErrorQux = 20;
            }

            public int MaxQux { get; set; }

            public int ErrorQux { get; set; }

            private static int Validate(Class1 instance, int value)
            {
                if (value > instance.ErrorQux)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return Math.Min(Math.Max(value, 0), ((Class1)instance).MaxQux);
            }
        }

        private class Class2 : PerspexObject
        {
            public static readonly PerspexProperty<int> QuxProperty =
                Class1.QuxProperty.AddOwner<Class2>();

            static Class2()
            {
                QuxProperty.OverrideValidation<Class2>(Validate);
            }

            private static int Validate(Class2 instance, int value)
            {
                if (value < 100)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return value;
            }
        }

        private class Class3 : Class2
        {
            static Class3()
            {
                QuxProperty.OverrideValidation<Class3>(null);
            }
        }
    }
}
