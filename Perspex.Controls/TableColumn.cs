// -----------------------------------------------------------------------
// <copyright file="TableColumn.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;

    public class TableColumn : PerspexObject
    {
        public static readonly PerspexProperty<GridLength> WidthProperty =
            ColumnDefinition.WidthProperty.AddOwner<TableColumn>();

        public TableColumn(object header, Func<object, Control> cellTemplate)
        {
            this.CellTemplate = cellTemplate;
        }

        public Func<object, Control> CellTemplate
        {
            get;
            private set;
        }

        public double ActualWidth
        {
            get;
            internal set;
        }

        public GridLength Width
        {
            get { return this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }
    }

    public class TableColumn<T> : TableColumn
    {
        public TableColumn(object header, Func<T, Control> cellTemplate)
            : base(header, x => cellTemplate((T)x))
        {
        }
    }
}
