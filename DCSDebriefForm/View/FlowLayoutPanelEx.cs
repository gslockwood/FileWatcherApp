namespace DCSDebriefForm.View
{
    public class FlowLayoutPanelEx : FlowLayoutPanel
    {
        // Define the event that the containing form will subscribe to
        public event EventHandler<GradeItemClickEventArgs>? GradeItemFirstColumnClicked;

        public FlowLayoutPanelEx()
        {
            // Optional: Set default flow direction
            FlowDirection = FlowDirection.TopDown;
            // Optional: Prevent wrapping if you want a single column
            WrapContents = false;
            AutoScroll = true;

        }

        public void Insert(LsoGradeItem item)
        {
            Controls.Add(item);
            int index = 0;
            foreach( Control control in Controls )
            {
                LsoGradeItem? lsoGradeItem = control as LsoGradeItem;
                if( lsoGradeItem != null && lsoGradeItem.DateTime > item.DateTime ) { index++; continue; }

                Controls.SetChildIndex(item, index);
                break;
            }
        }

        // Override OnControlAdded to handle sizing and event subscription
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            // Check if the added control is an LsoGradeItem
            if( e.Control is LsoGradeItem lsoGradeItem )
            {
                // Set its width to fill the panel horizontally
                ResizeLsoGradeItem(lsoGradeItem);

                // Subscribe to the custom event exposed by the LsoGradeItem
                lsoGradeItem.FirstColumnClicked += LsoGradeItem_FirstColumnClicked;
            }
        }

        // Override OnControlRemoved to unsubscribe from the event
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            // Check if the removed control was an LsoGradeItem
            if( e.Control is LsoGradeItem lsoGradeItem )
            {
                // Unsubscribe to prevent memory leaks
                lsoGradeItem.FirstColumnClicked -= LsoGradeItem_FirstColumnClicked;
            }
        }

        // Override OnResize to adjust the width of LsoGradeItems when the panel resizes
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLsoGradeItemSizes();
        }

        // Helper method to resize all LsoGradeItem controls
        private void AdjustLsoGradeItemSizes()
        {
            foreach( Control control in Controls )
                if( control is LsoGradeItem lsoGradeItem )
                    ResizeLsoGradeItem(lsoGradeItem);
        }

        // Helper method to set the width of a single LsoGradeItem
        private void ResizeLsoGradeItem(LsoGradeItem lsoGradeItem)
        {
            // Set the width to the client width minus padding/margins
            // Account for the vertical scrollbar if it's visible
            int availableWidth = ClientSize.Width - Padding.Horizontal;

            // Estimate scrollbar width if needed, or rely on AutoScroll
            // A simple way is to just use ClientSize.Width
            lsoGradeItem.Width = ClientSize.Width; // FlowLayoutPanel handles padding internally

            // Note: FlowLayoutPanel's default behavior often handles the horizontal
            // size correctly when FlowDirection is TopDown and WrapContents is false,
            // especially if the child's Anchor is Left | Right or Dock is Fill.
            // Setting the Width explicitly like this ensures it fills the available space.
            // If you encounter issues with scrollbars or padding, adjust the calculation.
        }


        // Event handler for the FirstColumnClicked event from the LsoGradeItem controls
        private void LsoGradeItem_FirstColumnClicked(object? sender, ListViewItem clickedListViewItem)
        {
            // The sender is the LsoGradeItem that raised the event
            if( sender is LsoGradeItem lsoGradeItem )
            {
                // Raise the event for the form to handle
                OnGradeItemFirstColumnClicked(lsoGradeItem, clickedListViewItem);
            }
        }

        // Protected method to safely raise the GradeItemFirstColumnClicked event
        protected virtual void OnGradeItemFirstColumnClicked(LsoGradeItem gradeItemControl, ListViewItem clickedListViewItem)
        {
            // Create the custom event arguments
            GradeItemClickEventArgs args = new GradeItemClickEventArgs(gradeItemControl, clickedListViewItem);

            // Raise the event
            GradeItemFirstColumnClicked?.Invoke(this, args);
        }

    }

    //----------------------------------------------------------------------------------

}
