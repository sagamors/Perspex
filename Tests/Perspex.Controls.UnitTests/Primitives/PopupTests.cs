﻿// -----------------------------------------------------------------------
// <copyright file="PopupTests.cs" company="Steven Kirk">
// Copyright 2013 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls.UnitTests.Primitives
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using Layout;
    using Moq;
    using Perspex.Controls.Presenters;
    using Perspex.Controls.Primitives;
    using Perspex.LogicalTree;
    using Perspex.Platform;
    using Perspex.Styling;
    using Perspex.VisualTree;
    using Splat;
    using Templates;
    using Xunit;

    public class PopupTests
    {
        [Fact]
        public void Setting_Child_Should_Set_Child_Controls_LogicalParent()
        {
            var target = new Popup();
            var child = new Control();

            target.Child = child;

            Assert.Equal(child.Parent, target);
            Assert.Equal(((ILogical)child).LogicalParent, target);
        }

        [Fact]
        public void Clearing_Child_Should_Clear_Child_Controls_Parent()
        {
            var target = new Popup();
            var child = new Control();

            target.Child = child;
            target.Child = null;

            Assert.Null(child.Parent);
            Assert.Null(((ILogical)child).LogicalParent);
        }

        [Fact]
        public void Child_Control_Should_Appear_In_LogicalChildren()
        {
            var target = new Popup();
            var child = new Control();

            target.Child = child;

            Assert.Equal(new[] { child }, target.GetLogicalChildren());
        }

        [Fact]
        public void Clearing_Child_Should_Remove_From_LogicalChildren()
        {
            var target = new Popup();
            var child = new Control();

            target.Child = child;
            target.Child = null;

            Assert.Equal(new ILogical[0], ((ILogical)target).LogicalChildren.ToList());
        }

        [Fact]
        public void Setting_Child_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new Popup();
            var child = new Control();
            var called = false;

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Add;

            target.Child = child;

            Assert.True(called);
        }

        [Fact]
        public void Clearing_Child_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new Popup();
            var child = new Control();
            var called = false;

            target.Child = child;

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) =>
                called = e.Action == NotifyCollectionChangedAction.Remove;

            target.Child = null;

            Assert.True(called);
        }

        [Fact]
        public void Changing_Child_Should_Fire_LogicalChildren_CollectionChanged()
        {
            var target = new Popup();
            var child1 = new Control();
            var child2 = new Control();
            var called = false;

            target.Child = child1;

            ((ILogical)target).LogicalChildren.CollectionChanged += (s, e) => called = true;

            target.Child = child2;

            Assert.True(called);
        }

        [Fact]
        public void Setting_Child_Should_Not_Set_Childs_VisualParent()
        {
            var target = new Popup();
            var child = new Control();

            target.Child = child;

            Assert.Null(((IVisual)child).VisualParent);
        }

        [Fact]
        public void PopupRoot_Should_Initially_Be_Null()
        {
            using (CreateServices())
            {
                var target = new Popup();

                Assert.Null(target.PopupRoot);
            }
        }

        [Fact]
        public void PopupRoot_Should_Have_Null_VisualParent()
        {
            using (CreateServices())
            {
                var target = new Popup();

                target.Open();

                Assert.Null(target.PopupRoot.GetVisualParent());
            }
        }

        [Fact]
        public void PopupRoot_Should_Have_Popup_As_LogicalParent()
        {
            using (CreateServices())
            {
                var target = new Popup();

                target.Open();

                Assert.Equal(target, target.PopupRoot.Parent);
                Assert.Equal(target, target.PopupRoot.GetLogicalParent());
            }
        }

        [Fact]
        public void PopupRoot_Should_Have_Template_Applied()
        {
            using (CreateServices())
            {
                var target = new Popup();
                var child = new Control();

                target.Open();

                Assert.Equal(1, target.PopupRoot.GetVisualChildren().Count());

                var templatedChild = target.PopupRoot.GetVisualChildren().Single();
                Assert.IsType<ContentPresenter>(templatedChild);
                Assert.Equal(target.PopupRoot, ((IControl)templatedChild).TemplatedParent);
            }
        }

        [Fact]
        public void PopupRoot_Should_Have_Child_As_LogicalChild()
        {
            using (CreateServices())
            {
                var target = new Popup();
                var child = new Control();

                target.Child = child;
                target.Open();

                Assert.Equal(new[] { child }, target.PopupRoot.GetLogicalChildren());
            }
        }

        [Fact]
        public void Templated_Control_With_Popup_In_Template_Should_Set_TemplatedParent()
        {
            using (CreateServices())
            {
                PopupContentControl target;
                var root = new TestRoot
                {
                    Child = target = new PopupContentControl
                    {
                        Content = new Border(),
                        Template = new ControlTemplate<PopupContentControl>(PopupContentControlTemplate),
                    }
                };

                target.ApplyTemplate();
                var popup = target.GetTemplateChild<Popup>("popup");
                popup.Open();
                var popupRoot = popup.PopupRoot;

                var children = popupRoot.GetVisualDescendents().ToList();
                var types = children.Select(x => x.GetType().Name).ToList();

                Assert.Equal(
                    new[]
                    {
                        "ContentPresenter",
                        "ContentPresenter",
                        "Border",
                    },
                    types);

                var templatedParents = children
                    .OfType<IControl>()
                    .Select(x => x.TemplatedParent).ToList();

                Assert.Equal(
                    new object[]
                    {
                        popupRoot,
                        target,
                        null,
                    },
                    templatedParents);
            }
        }

        private static IDisposable CreateServices()
        {
            var result = Locator.Current.WithResolver();

            var styles = new Styles
            {
                new Style(x => x.OfType<PopupRoot>())
                {
                    Setters = new[]
                    {
                        new Setter(TemplatedControl.TemplateProperty, new ControlTemplate<PopupRoot>(PopupRootTemplate)),
                    }
                },
            };

            var globalStyles = new Mock<IGlobalStyles>();
            globalStyles.Setup(x => x.Styles).Returns(styles);

            Locator.CurrentMutable.Register(() => globalStyles.Object, typeof(IGlobalStyles));
            Locator.CurrentMutable.Register(() => new LayoutManager(), typeof(ILayoutManager));
            Locator.CurrentMutable.Register(() => new Mock<IPlatformThreadingInterface>().Object, typeof(IPlatformThreadingInterface));
            Locator.CurrentMutable.Register(() => new Mock<IPopupImpl>().Object, typeof(IPopupImpl));
            Locator.CurrentMutable.Register(() => new Styler(), typeof(IStyler));
            return result;
        }

        private static IControl PopupRootTemplate(PopupRoot control)
        {
            return new ContentPresenter
            {
                Name = "contentPresenter",
                [~ContentPresenter.ContentProperty] = control[~PopupRoot.ContentProperty],
            };
        }

        private static IControl PopupContentControlTemplate(PopupContentControl control)
        {
            return new Popup
            {
                Name = "popup",
                Child = new ContentPresenter
                {
                    [~ContentPresenter.ContentProperty] = control[~PopupRoot.ContentProperty],
                }
            };
        }

        private class PopupContentControl : ContentControl
        {
        }
    }
}
