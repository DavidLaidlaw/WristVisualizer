using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class AnimationCreatorForm : Form
    {
        List<int> _animationOrder; //used make sure we keep track of indexes and are not reliant on the string comparison

        private static double[] DEFAULT_DISTANCES = { 1.0, 1.5, 2.0 };
        private const int DEFAULT_MAX_NUM_CONTOURS = 5;

        private CheckBox[] _contourCheckBoxes;
        private NumericUpDown[] _contourNumericUpDowns;
        private Button[] _contourColorButtons;
        private double[] _defaultContourDistances;

        public AnimationCreatorForm(string[] positionNames)
        {

            InitializeComponent();
            _animationOrder = new List<int>();
            _defaultContourDistances = DEFAULT_DISTANCES;

            setupContourSelectionGUI();
            //testSetupTestData();
            setupListBoxAllPositions(positionNames); //Do I want to save these?
            updateButtonsEnabledState();
        }

        private void setupListBoxAllPositions(string[] positions)
        {
            listBoxAllPositions.Items.Clear();
            listBoxAllPositions.Items.AddRange(positions);
        }

        private void testSetupTestData()
        {
            listBoxAllPositions.Items.Add("Neutral");
            listBoxAllPositions.Items.Add("Junk1");
            listBoxAllPositions.Items.Add("Junk2");
            listBoxAllPositions.Items.Add("Junk3");
            listBoxAllPositions.Items.Add("Junk4");
            listBoxAllPositions.Items.Add("Junk5");
        }

        public int[] getAnimationOrder()
        {
            return _animationOrder.ToArray();
        }

        public int NumberStepsPerPositionChange
        {
            get { return (int)numericUpDownSteps.Value; }
        }

        public bool ShowDistanceMap
        {
            get { return checkBoxDistanceMap.Checked; }
            set { checkBoxDistanceMap.Checked = value; }
        }

        public decimal DistanceMapMaximumValue
        {
            get { return numericUpDownDistanceMapDist.Value; }
            set { numericUpDownDistanceMapDist.Value = value; }
        }

        #region Selecting Positions and Order
        private void updateButtonsEnabledState()
        {
            buttonAdd.Enabled = (listBoxAllPositions.SelectedIndices.Count > 0);
            //listBoxAnimationSequence should only be one selected at a time, but lets check
            if (listBoxAnimationSequence.SelectedIndex == -1)
            {
                //none selected, yes?
                buttonRemove.Enabled = false;
                buttonMoveUp.Enabled = false;
                buttonMoveDown.Enabled = false;
            }
            else
            {
                //okay, so we have something, enable remove
                buttonRemove.Enabled = true;
                buttonMoveUp.Enabled = (listBoxAnimationSequence.SelectedIndex != 0);
                buttonMoveDown.Enabled = (listBoxAnimationSequence.SelectedIndex != listBoxAnimationSequence.Items.Count - 1);
            }

            buttonOK.Enabled = (listBoxAnimationSequence.Items.Count >= 2); //need at least two positions to animate anything :)
        }

        private void listBoxAllPositions_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtonsEnabledState();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            /* We need to get the currently selected position, because we want
             * to add the new positions below the selected one. This also works well, 
             * because if no position is selected (when the form first loads). Then 
             * selectedIndex==-1, so when we add 1 to the index, it will get added 
             * to index==0. Clever, hu?
             */
            int selectedAnimationSequenceIndex = listBoxAnimationSequence.SelectedIndex;

            //do it for each selected item
            for (int i = 0; i < listBoxAllPositions.SelectedIndices.Count; i++)
            {
                int newIndex = selectedAnimationSequenceIndex + 1 + i;
                listBoxAnimationSequence.Items.Insert(newIndex, listBoxAllPositions.SelectedItems[i]);
                _animationOrder.Insert(newIndex, listBoxAllPositions.SelectedIndices[i]);
                //now select the newest item...
                listBoxAnimationSequence.SelectedIndex = newIndex;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            //if none selected, force update and get out
            int oldIndex = listBoxAnimationSequence.SelectedIndex;
            if (oldIndex == -1) //none selected or already at the top
            {
                //how did we get here? lets force check of enabled states and leave...
                listBoxAnimationSequence_SelectedIndexChanged(this, null);
                return;
            }

            listBoxAnimationSequence.Items.RemoveAt(oldIndex);
            _animationOrder.RemoveAt(oldIndex);

            //check if there are any items left
            if (listBoxAnimationSequence.Items.Count > 0)
            {
                //try and make the selected item, the one before the current...yes?
                if (oldIndex == 0 || listBoxAnimationSequence.Items.Count == 1)
                    listBoxAnimationSequence.SelectedIndex = 0;
                else
                    listBoxAnimationSequence.SelectedIndex = oldIndex - 1;
            }
        }

        private void listBoxAnimationSequence_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateButtonsEnabledState();
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            int oldIndex = listBoxAnimationSequence.SelectedIndex;
            if (oldIndex == -1 || oldIndex == 0) //none selected or already at the top
            {
                //how did we get here? lets force check of enabled states and leave...
                listBoxAnimationSequence_SelectedIndexChanged(this, null);
                return;
            }

            //actualy move us up: 1) Save 2) remove at old positoin, 3) insert into new
            object selectedObject = listBoxAnimationSequence.Items[oldIndex];
            int selectedObjectKey = _animationOrder[oldIndex];
            listBoxAnimationSequence.Items.RemoveAt(oldIndex);
            _animationOrder.RemoveAt(oldIndex);
            listBoxAnimationSequence.Items.Insert(oldIndex - 1, selectedObject);
            _animationOrder.Insert(oldIndex - 1, selectedObjectKey);

            //need to keep this one selected, yes...?
            listBoxAnimationSequence.SelectedIndex = oldIndex - 1; //this should then call the selected index change to update the buttons
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            int oldIndex = listBoxAnimationSequence.SelectedIndex;
            if (oldIndex == -1 || oldIndex == listBoxAnimationSequence.Items.Count - 1) //none selected or already at the bottom
            {
                //how did we get here? lets force check of enabled states and leave...
                listBoxAnimationSequence_SelectedIndexChanged(this, null);
                return;
            }

            //actualy move us up: 1) Save 2) remove at old positoin, 3) insert into new
            object selectedObject = listBoxAnimationSequence.Items[oldIndex];
            int selectedObjectKey = _animationOrder[oldIndex];
            listBoxAnimationSequence.Items.RemoveAt(oldIndex);
            _animationOrder.RemoveAt(oldIndex);
            listBoxAnimationSequence.Items.Insert(oldIndex + 1, selectedObject);
            _animationOrder.Insert(oldIndex + 1, selectedObjectKey);

            //need to keep this one selected, yes...?
            listBoxAnimationSequence.SelectedIndex = oldIndex + 1; //this should then call the selected index change to update the buttons
        }
        #endregion

        #region GUI Setup
        private void setupContourSelectionGUI()
        {
            int maxNumberContours = Math.Max(_defaultContourDistances.Length, DEFAULT_MAX_NUM_CONTOURS);
            _contourCheckBoxes = new CheckBox[maxNumberContours];
            _contourNumericUpDowns = new NumericUpDown[maxNumberContours];
            _contourColorButtons = new Button[maxNumberContours];

            this.SuspendLayout();
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();

            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {
                setupContourRow(i);
                if (_defaultContourDistances.Length > i) //check if we have a default value...
                {
                    _contourCheckBoxes[i].Checked = true;
                    _contourNumericUpDowns[i].Enabled = true;
                    _contourNumericUpDowns[i].Value = (decimal)_defaultContourDistances[i];
                    _contourColorButtons[i].Enabled = true;
                }
            }
            this.ResumeLayout();
        }

        private void setupContourRow(int row)
        {
            CheckBox box = new CheckBox();
            NumericUpDown upDown = new NumericUpDown();
            Label label = new Label();
            Button button = new Button();

            box.AutoSize = true;
            box.CheckedChanged += new EventHandler(contourCheckBox_CheckedChanged);

            upDown.Minimum = 0;
            upDown.Maximum = 5;
            upDown.DecimalPlaces = 2;
            upDown.Increment = 0.1M;
            upDown.Size = new System.Drawing.Size(59, 20);
            upDown.Enabled = false;

            button.AutoSize = true;
            button.Size = new System.Drawing.Size(22, 22);
            button.BackColor = Color.White;
            button.Click += new EventHandler(colorButton_Click);
            button.Enabled = false;

            label.AutoSize = true;
            label.Text = "mm";
            label.Margin = new Padding(0);
            label.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label.TextAlign = ContentAlignment.TopLeft;

            tableLayoutPanel.Controls.Add(box, 0, row);
            tableLayoutPanel.Controls.Add(upDown, 1, row);
            tableLayoutPanel.Controls.Add(label, 2, row);
            tableLayoutPanel.Controls.Add(button, 3, row);
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());

            _contourCheckBoxes[row] = box;
            _contourNumericUpDowns[row] = upDown;
            _contourColorButtons[row] = button;

        }

        #endregion

        #region Distance Map Stuff
        void contourCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //lets find out which one we are, and change the enabled state of our little friend
            for (int i = 0; i < _contourCheckBoxes.Length; i++)
            {
                if (_contourCheckBoxes[i] == sender) //found us!
                {
                    _contourNumericUpDowns[i].Enabled = ((CheckBox)sender).Checked;
                    _contourColorButtons[i].Enabled = ((CheckBox)sender).Checked;
                    return;
                }
            }
        }

        void colorButton_Click(object sender, EventArgs e)
        {
            //lets find out which one we are, and change the enabled state of our little friend
            for (int i = 0; i < _contourColorButtons.Length; i++)
            {
                if (_contourColorButtons[i] == sender) //found us!
                {
                    ColorDialog cg = new ColorDialog();
                    cg.Color = ((Button)sender).BackColor;
                    DialogResult r = cg.ShowDialog();
                    if (r == DialogResult.Cancel)
                        return;

                    ((Button)sender).BackColor = cg.Color;
                }
            }
        }
        #endregion

        private bool validateForm()
        {
            if (_animationOrder.Count < 2)
                throw new WristVizualizerException("Need at least two positions in order to animate");

            return true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                validateForm();
            }
            catch (WristVizualizerException ex)
            {
                string msg = "Error: " + ex.Message;
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
                return;
            }

            //TODO: Check validation output
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //Do we want to check before we cancel?
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBoxDistanceMap_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownDistanceMapDist.Enabled = checkBoxDistanceMap.Checked;
        }

    }
}