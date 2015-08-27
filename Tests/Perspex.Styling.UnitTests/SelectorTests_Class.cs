﻿// -----------------------------------------------------------------------
// <copyright file="SelectorTests_Class.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Styling.UnitTests
{
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using Moq;
    using Perspex.Styling;
    using Xunit;

    public class SelectorTests_Class
    {
        [Fact]
        public async Task Class_Matches_Control_With_Class()
        {
            var control = new Control1
            {
                Classes = new Classes { "foo" },
            };

            var target = new Selector().Class("foo");
            var activator = target.Match(control).ObservableResult;

            Assert.True(await activator.Take(1));
        }

        [Fact]
        public async Task Class_Doesnt_Match_Control_Without_Class()
        {
            var control = new Control1
            {
                Classes = new Classes { "bar" },
            };

            var target = new Selector().Class("foo");
            var activator = target.Match(control).ObservableResult;

            Assert.False(await activator.Take(1));
        }

        [Fact]
        public async Task Class_Matches_Control_With_TemplatedParent()
        {
            var control = new Control1
            {
                Classes = new Classes { "foo" },
                TemplatedParent = new Mock<ITemplatedControl>().Object,
            };

            var target = new Selector().Class("foo");
            var activator = target.Match(control).ObservableResult;

            Assert.True(await activator.Take(1));
        }

        [Fact]
        public async Task Class_Tracks_Additions()
        {
            var control = new Control1();

            var target = new Selector().Class("foo");
            var activator = target.Match(control).ObservableResult;

            Assert.False(await activator.Take(1));
            control.Classes.Add("foo");
            Assert.True(await activator.Take(1));
        }

        [Fact]
        public async Task Class_Tracks_Removals()
        {
            var control = new Control1
            {
                Classes = new Classes { "foo" },
            };

            var target = new Selector().Class("foo");
            var activator = target.Match(control).ObservableResult;

            Assert.True(await activator.Take(1));
            control.Classes.Remove("foo");
            Assert.False(await activator.Take(1));
        }

        [Fact]
        public async Task Multiple_Classes()
        {
            var control = new Control1();
            var target = new Selector().Class("foo").Class("bar");
            var activator = target.Match(control).ObservableResult;

            Assert.False(await activator.Take(1));
            control.Classes.Add("foo");
            Assert.False(await activator.Take(1));
            control.Classes.Add("bar");
            Assert.True(await activator.Take(1));
            control.Classes.Remove("bar");
            Assert.False(await activator.Take(1));
        }

        public class Control1 : TestControlBase
        {
        }
    }
}
