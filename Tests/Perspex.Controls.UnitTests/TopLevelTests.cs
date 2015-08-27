﻿// -----------------------------------------------------------------------
// <copyright file="TopLevelTests.cs" company="Steven Kirk">
// Copyright 2015 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls.UnitTests
{
    using System.Reactive;
    using System.Reactive.Subjects;
    using Moq;
    using Perspex.Controls.Presenters;
    using Perspex.Controls.Templates;
    using Perspex.Input;
    using Perspex.Input.Raw;
    using Perspex.Layout;
    using Perspex.Platform;
    using Perspex.Rendering;
    using Perspex.Styling;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.AutoMoq;
    using Splat;
    using Xunit;

    public class TopLevelTests
    {
        [Fact]
        public void ClientSize_Should_Be_Set_On_Construction()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                var target = new TestTopLevel(impl.Object);

                Assert.Equal(new Size(123, 456), target.ClientSize);
            }
        }

        [Fact]
        public void Width_Should_Not_Be_Set_On_Construction()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                var target = new TestTopLevel(impl.Object);

                Assert.Equal(double.NaN, target.Width);
            }
        }

        [Fact]
        public void Height_Should_Not_Be_Set_On_Construction()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                var target = new TestTopLevel(impl.Object);

                Assert.Equal(double.NaN, target.Height);
            }
        }

        [Fact]
        public void Layout_Pass_Should_Not_Be_Automatically_Scheduled()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                var target = new TestTopLevel(impl.Object);

                // The layout pass should be scheduled by the derived class.
                var layoutManagerMock = Mock.Get(target.LayoutManager);
                layoutManagerMock.Verify(x => x.ExecuteLayoutPass(), Times.Never);
            }
        }

        [Fact]
        public void Bounds_Should_Be_Set_After_Layout_Pass()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();
                Locator.CurrentMutable.RegisterConstant(new LayoutManager(), typeof(ILayoutManager));

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupProperty(x => x.ClientSize);
                impl.SetupProperty(x => x.Resized);

                var target = new TestTopLevel(impl.Object)
                {
                    Template = new ControlTemplate<TestTopLevel>(x =>
                        new ContentPresenter
                        {
                            [~ContentPresenter.ContentProperty] = x[~TestTopLevel.ContentProperty],
                        }),
                    Content = new TextBlock
                    {
                        Width = 321,
                        Height = 432,
                    }
                };

                target.LayoutManager.ExecuteLayoutPass();

                Assert.Equal(new Rect(0, 0, 321, 432), target.Bounds);
            }
        }

        [Fact]
        public void Impl_ClientSize_Should_Be_Set_After_Layout_Pass()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();
                Locator.CurrentMutable.RegisterConstant(new LayoutManager(), typeof(ILayoutManager));

                var impl = new Mock<ITopLevelImpl>();

                var target = new TestTopLevel(impl.Object)
                {
                    Template = new ControlTemplate<TestTopLevel>(x =>
                        new ContentPresenter
                        {
                            [~ContentPresenter.ContentProperty] = x[~TestTopLevel.ContentProperty],
                        }),
                    Content = new TextBlock
                    {
                        Width = 321,
                        Height = 432,
                    }
                };

                target.LayoutManager.ExecuteLayoutPass();

                impl.VerifySet(x => x.ClientSize = new Size(321, 432));
            }
        }

        [Fact]
        public void Width_And_Height_Should_Not_Be_Set_After_Layout_Pass()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                var target = new TestTopLevel(impl.Object);
                target.LayoutManager.ExecuteLayoutPass();

                Assert.Equal(double.NaN, target.Width);
                Assert.Equal(double.NaN, target.Height);
            }
        }

        [Fact]
        public void Render_Should_Be_Scheduled_After_Layout_Pass()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();
                var completed = new Subject<Unit>();
                var layoutManagerMock = Mock.Get(Locator.Current.GetService<ILayoutManager>());
                layoutManagerMock.Setup(x => x.LayoutCompleted).Returns(completed);

                var impl = new Mock<ITopLevelImpl>();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                var target = new TestTopLevel(impl.Object);
                completed.OnNext(Unit.Default);

                var renderManagerMock = Mock.Get(Locator.Current.GetService<IRenderManager>());
                renderManagerMock.Verify(x => x.InvalidateRender(target));
            }
        }

        [Fact]
        public void Width_And_Height_Should_Be_Set_After_Window_Resize_Notification()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupAllProperties();
                impl.Setup(x => x.ClientSize).Returns(new Size(123, 456));

                // The user has resized the window, so we can no longer auto-size.
                var target = new TestTopLevel(impl.Object);
                impl.Object.Resized(new Size(100, 200));

                Assert.Equal(100, target.Width);
                Assert.Equal(200, target.Height);
            }
        }

        [Fact]
        public void Activate_Should_Call_Impl_Activate()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                var target = new TestTopLevel(impl.Object);

                target.Activate();

                impl.Verify(x => x.Activate());
            }
        }

        [Fact]
        public void Impl_Activate_Should_Call_Raise_Activated_Event()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupAllProperties();

                bool raised = false;
                var target = new TestTopLevel(impl.Object);
                target.Activated += (s, e) => raised = true;

                impl.Object.Activated();

                Assert.True(raised);
            }
        }

        [Fact]
        public void Impl_Close_Should_Call_Raise_Closed_Event()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupAllProperties();

                bool raised = false;
                var target = new TestTopLevel(impl.Object);
                target.Closed += (s, e) => raised = true;

                impl.Object.Closed();

                Assert.True(raised);
            }
        }

        [Fact]
        public void Impl_Deactivate_Should_Call_Raise_Activated_Event()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupAllProperties();

                bool raised = false;
                var target = new TestTopLevel(impl.Object);
                target.Deactivated += (s, e) => raised = true;

                impl.Object.Deactivated();

                Assert.True(raised);
            }
        }

        [Fact]
        public void Impl_Input_Should_Pass_Input_To_InputManager()
        {
            using (Locator.CurrentMutable.WithResolver())
            {
                this.RegisterServices();

                var impl = new Mock<ITopLevelImpl>();
                impl.SetupAllProperties();
                var target = new TestTopLevel(impl.Object);

                var input = new RawKeyEventArgs(
                    new Mock<IKeyboardDevice>().Object,
                    0,
                    RawKeyEventType.KeyDown,
                    Key.A,
                    "A");
                impl.Object.Input(input);

                var inputManagerMock = Mock.Get(InputManager.Instance);
                inputManagerMock.Verify(x => x.Process(input));
            }
        }

        private void RegisterServices()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var l = Locator.CurrentMutable;

            var formattedText = fixture.Create<IFormattedTextImpl>();
            var globalStyles = new Mock<IGlobalStyles>();
            var layoutManager = fixture.Create<ILayoutManager>();
            var renderInterface = fixture.Create<IPlatformRenderInterface>();
            var renderManager = fixture.Create<IRenderManager>();
            var windowImpl = new Mock<IWindowImpl>();
            var theme = new Styles();

            globalStyles.Setup(x => x.Styles).Returns(theme);

            l.RegisterConstant(new Mock<IInputManager>().Object, typeof(IInputManager));
            l.RegisterConstant(new Mock<IFocusManager>().Object, typeof(IFocusManager));
            l.RegisterConstant(globalStyles.Object, typeof(IGlobalStyles));
            l.RegisterConstant(layoutManager, typeof(ILayoutManager));
            l.RegisterConstant(new Mock<IPlatformThreadingInterface>().Object, typeof(IPlatformThreadingInterface));
            l.RegisterConstant(renderInterface, typeof(IPlatformRenderInterface));
            l.RegisterConstant(renderManager, typeof(IRenderManager));
            l.RegisterConstant(new Styler(), typeof(IStyler));
        }

        private class TestTopLevel : TopLevel
        {
            public TestTopLevel(ITopLevelImpl impl)
                : base(impl)
            {
            }
        }
    }
}
