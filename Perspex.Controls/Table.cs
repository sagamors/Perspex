// -----------------------------------------------------------------------
// <copyright file="Table.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using Perspex.Controls.Generators;

    public class Table : ItemsControl
    {
        private TableColumns columns;

        public TableColumns Columns
        {
            get
            {
                if (this.columns == null)
                {
                    this.columns = new TableColumns();
                }

                return this.columns;
            }

            set
            {
                if (this.columns != value)
                {
                    this.columns = value;
                }
            }
        }

        protected override ItemContainerGenerator CreateItemContainerGenerator()
        {
            return new TableItemContainerGenerator(this);
        }
    }
}
