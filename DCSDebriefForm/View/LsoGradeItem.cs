using DCSDebriefFile;

namespace DCSDebriefForm
{
    public partial class LsoGradeItem : UserControl
    {
        public event EventHandler<ListViewItem>? FirstColumnClicked;

        public LsoGradeItem()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true); // Ensure the control is redrawn when resized
            DoubleBuffered = true; // Reduce flicker

            AutoScroll = false;

            listView.MouseDoubleClick += GradeListView_MouseClick;

        }
        private void GradeListView_MouseClick(object? sender, MouseEventArgs e)
        {
            ListView? lv = sender as ListView;
            if( lv == null ) return;

            // Perform a hit test to get information about what was clicked
            ListViewHitTestInfo hitTestInfo = lv.HitTest(e.Location);

            // Check if an item was clicked AND the clicked location is the item's label (first column)
            if( hitTestInfo.Item != null && hitTestInfo.Location == ListViewHitTestLocations.Label )
            {
                // Raise the custom event
                OnFirstColumnClicked(hitTestInfo.Item);
            }
        }


        // Protected method to safely raise the FirstColumnClicked event
        protected virtual void OnFirstColumnClicked(ListViewItem item)
        {
            FirstColumnClicked?.Invoke(this, item);
        }

        // Override the OnResize method to make sure the UserControl
        // takes up the full width of its parent (the ListBox).  This
        // is crucial for the items to fill the listbox.
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if( Parent != null )
            {
                Width = Parent.ClientSize.Width;
            }
        }


        // Public properties to access and modify the control's values.  These
        // properties provide a way to set the status, grade, and comment
        // from outside the class.

        public DateTime DateTime
        {
            get
            {
                if( labelDateTime.Tag != null ) return (DateTime)labelDateTime.Tag;
                return DateTime.MinValue;
            }
            set
            {
                labelDateTime.Text = value.ToString("G");
                labelDateTime.Tag = value;
            }
        }

        public string Grade
        {
            //get { return statusLabel.Text; }
            set
            {
                if( value == null ) return;

                labelGrade.Text = value;

                switch( value )
                {
                    case "OK":
                        panelGradeIndicator.BackColor = Color.Green;
                        break;

                    case "WO":
                    case "C":
                    case "CP":
                    case "UNK":
                    case "---":
                        panelGradeIndicator.BackColor = Color.Red;
                        break;

                    case "B":
                        panelGradeIndicator.BackColor = Color.LightYellow;
                        break;

                    default:
                        panelGradeIndicator.BackColor = Color.Yellow;
                        break;
                }

                //if( value.Equals("OK") )
                //    panelGradeIndicator.BackColor = Color.Green;
                //else if( value.Equals("No Grade") )
                //    panelGradeIndicator.BackColor = Color.Black;

                //else if( value.Equals("Fail") )
                //    panelGradeIndicator.BackColor = Color.Red;

                //panelGradeIndicator.BackColor = Color.Yellow;

            }
        }

        public string Pilot
        {
            set { labelPilot.Text = value; }
        }

        public string Carrier
        {
            set { labelCarrier.Text = value; }
        }
        public string LsoGrade
        {
            //get { return lsoGradeLabel.Text; }
            set { labelLsoGrade.Text = value; }
        }

        public string Translation
        {
            //get { return commentLabel.Text; }
            set { labelGradeTranslation.Text = value; }
        }

        public IList<LSOGrade.LSOGradeError>? Errors
        {
            set
            {
                if( value == null || value.Count == 0 ) return;

                foreach( LSOGrade.LSOGradeError lsoGradeItem in value )
                {
                    ListViewItem item = new(lsoGradeItem.Error);
                    item.SubItems.Add(lsoGradeItem.Translation);
                    listView.Items.Add(item);
                }

            }
        }
    }
}
