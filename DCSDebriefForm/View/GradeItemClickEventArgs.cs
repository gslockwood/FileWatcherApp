namespace DCSDebriefForm.View
{
    public class GradeItemClickEventArgs : EventArgs
    {
        public LsoGradeItem GradeItemControl { get; } // The LsoGradeItem that was clicked
        public ListViewItem ClickedListViewItem { get; } // The specific ListViewItem that was clicked

        public GradeItemClickEventArgs(LsoGradeItem gradeItemControl, ListViewItem clickedListViewItem)
        {
            GradeItemControl = gradeItemControl;
            ClickedListViewItem = clickedListViewItem;
        }
    }
}
