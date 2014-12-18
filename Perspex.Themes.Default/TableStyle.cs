// -----------------------------------------------------------------------
// <copyright file="TableStyle.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Themes.Default
{
    using System.Linq;
    using Perspex.Controls;
    using Perspex.Controls.Presenters;
    using Perspex.Media;
    using Perspex.Styling;

    public class TableStyle : Styles
    {
        public TableStyle()
        {
            this.AddRange(new[]
            {
                new Style(x => x.OfType<Table>())
                {
                    Setters = new[]
                    {
                        new Setter(Table.TemplateProperty, ControlTemplate.Create<Table>(this.Template)),
                        new Setter(Table.BorderBrushProperty, Brushes.Black),
                        new Setter(Table.BorderThicknessProperty, 1.0),
                    },
                },
            });
        }

        private Control Template(Table control)
        {
            return new Border
            {
                Padding = new Thickness(4),
                [~Border.BackgroundProperty] = control[~Table.BackgroundProperty],
                [~Border.BorderBrushProperty] = control[~Table.BorderBrushProperty],
                [~Border.BorderThicknessProperty] = control[~Table.BorderThicknessProperty],
                Content = new Grid
                {
                    RowDefinitions = new RowDefinitions
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Star),
                    },
                    Children = new Controls
                    {
                        new TableRow
                        {
                            Id = "header",
                        },
                        new ScrollViewer
                        {
                            Content = new ItemsPresenter
                            {
                                [~ItemsPresenter.ItemsProperty] = control[~Table.ItemsProperty],
                                [~ItemsPresenter.ItemsPanelProperty] = control[~Table.ItemsPanelProperty],
                            },
                            [Grid.RowProperty] = 1,
                        }
                    }
                }
            };
        }
    }
}
