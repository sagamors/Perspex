// -----------------------------------------------------------------------
// <copyright file="TableRowStyle.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Themes.Default
{
    using System.Linq;
    using Perspex.Controls;
    using Perspex.Controls.Presenters;
    using Perspex.Styling;

    public class TableRowStyle : Styles
    {
        private ItemsPanelTemplate panel = new ItemsPanelTemplate(() => new TableRowPanel());

        public TableRowStyle()
        {
            this.AddRange(new[]
            {
                new Style(x => x.OfType<TableRow>())
                {
                    Setters = new[]
                    {
                        new Setter(TableRow.TemplateProperty, ControlTemplate.Create<TableRow>(this.Template)),
                    },
                },
            });
        }

        private Control Template(TableRow control)
        {
            return new ItemsPresenter
            {
                [~ItemsPresenter.ItemsProperty] = control[~ItemsControl.ItemsProperty],
                [ItemsPresenter.ItemsPanelProperty] = this.panel,
            };
        }
    }
}
