// -----------------------------------------------------------------------
// <copyright file="TableRowContainerGenerator.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls.Generators
{
    using System.Collections;
    using System.Collections.Generic;

    public class TableItemContainerGenerator : ItemContainerGenerator
    {
        public TableItemContainerGenerator(Table owner)
            : base(owner)
        {
        }

        protected override Control CreateContainerOverride(object item)
        {
            TableRow result = item as TableRow;

            if (result == null)
            {
                result = new TableRow();
                result.Items = this.ApplyTemplate(item);
            }

            return result;
        }

        private IEnumerable<Control> ApplyTemplate(object item)
        {
            var table = (Table)this.Owner;

            foreach (var column in table.Columns)
            {
                yield return column.CellTemplate(item);
            }
        }
    }
}
