namespace DCSDebriefForm.View
{
    public class ResizableListView : ListView
    {
        // Override the OnResize method to handle resizing logic
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Ensure the control is in Details view and has at least one column
            if( this.View == System.Windows.Forms.View.Details && Columns.Count > 0 )
            {
                // Calculate the total width of all columns except the last one
                int totalColumnWidth = 0;
                for( int i = 0; i < Columns.Count - 1; i++ )
                {
                    totalColumnWidth += Columns[i].Width;
                }

                // Get the available width in the ListView's client area
                // We need to account for the vertical scrollbar if it's visible
                int clientWidth = ClientSize.Width;

                // Check if a vertical scrollbar is visible.
                // This is a bit tricky to determine precisely without P/Invoke,
                // but we can make a reasonable estimate or check if the total width
                // of all columns exceeds the client width before adjustment.
                // A simpler approach is to subtract the system scrollbar width
                // if the total column width (before adjustment) is less than the client width,
                // implying the last column will expand and potentially push content
                // to require a scrollbar, or if items already require one.

                // A more robust way involves checking the actual scrollbar state,
                // but for a basic implementation, we can assume the scrollbar width
                // if the total width of fixed columns is less than the client width,
                // as the last column will expand.
                // Let's use a simple check based on the total width vs client width.
                // If the total width of columns *before* adjusting the last one
                // is less than the client width, the last column will expand,
                // and we might need to reserve space for a scrollbar.
                // This is not perfect, as scrollbar visibility depends on item count too.

                // A more reliable way is to check the actual scrollbar state if possible
                // or subtract a standard scrollbar width if the view is Details.
                // For simplicity here, let's subtract a standard scrollbar width
                // if the total width of fixed columns is less than the client width.
                // This might leave a small gap if no scrollbar appears, or be slightly off.

                // Get the system scrollbar width
                int scrollbarWidth = SystemInformation.VerticalScrollBarWidth;

                // Calculate the width for the last column
                // Subtract the total width of fixed columns and the scrollbar width
                int lastColumnWidth = clientWidth - totalColumnWidth - scrollbarWidth;

                // Ensure the calculated width is not negative
                if( lastColumnWidth > 0 )
                {
                    // Set the width of the last column
                    Columns[Columns.Count - 1].Width = lastColumnWidth;
                }
                else
                {
                    // If the calculated width is not positive,
                    // set the last column width to a minimum or 0,
                    // and let the horizontal scrollbar handle it.
                    Columns[Columns.Count - 1].Width = 0; // Or a small minimum width
                }
            }
        }

        // You might also want to call OnResize initially when the control is created
        // or when columns are added/removed, to ensure the initial layout is correct.
        // This can be done in the constructor or after adding columns.

        // Example of calling OnResize after columns are added:
        // In your form or user control code, after adding columns to ResizableListView:
        // yourResizableListView.OnResize(EventArgs.Empty);
    }

}
