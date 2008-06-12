using System;
using System.Collections;
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

        public AnimationCreatorForm()
        {

            InitializeComponent();
            _animationOrder = new List<int>();
            testSetupTestData();
            updateButtonsEnabledState();
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

        private bool validateForm()
        {
            //TODO!
            return true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            validateForm();

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

    }
}