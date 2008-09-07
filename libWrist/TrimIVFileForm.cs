using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace libWrist
{
    [System.Diagnostics.DebuggerDisplay("TrimIVFileForm")]
    public partial class TrimIVFileForm : Form
    {
        private BackgroundWorker _bgWorker;
        private StringBuilder _error;
        private int _converted;
        private int _total;

        public TrimIVFileForm()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.FileName = textBoxFilename.Text;
            open.Filter = "Inventor Files (*.iv)|*.iv|All Files (*.*)|*.*";
            open.Multiselect = false;
            if (DialogResult.OK != open.ShowDialog())
                return;

            textBoxFilename.Text = open.FileName;
        }

        private void buttonTrim_Click(object sender, EventArgs e)
        {
            if (radioButtonTrim.Checked)
                TrimIVFile.tryStripFile(textBoxFilename.Text.Trim(), checkBoxCamera.Checked, checkBoxMaterial.Checked);
            else
                VRMLParser.ConvertVRMLToIV(textBoxFilename.Text.Trim(), checkBoxMimics10.Checked);
        }

        private bool validateTrim(string[] filenames)
        {
            //quick check that this is valid
            foreach (string file in filenames)
            {
                if (!Path.GetExtension(file).ToLower().Equals(".iv"))
                {
                    string msg = String.Format("Invalid file ({0})\nCan only trim IV files.", file);
                    MessageBox.Show(msg, "IV Trim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!File.Exists(file))
                {
                    string msg = String.Format("Invalid file ({0})\nFile does not exist!", file);
                    MessageBox.Show(msg, "IV Trim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            string msg2 = String.Format("This will trim {0} file(s).\n\nAre you sure you wish to continue?", filenames.Length);
            if (MessageBox.Show(msg2, "IV Trim", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            //got here, then we are okay
            return true;
        }

        private bool validateConvert(string[] filenames)
        {
            //quick check that this is valid
            foreach (string file in filenames)
            {
                if (!Path.GetExtension(file).ToLower().Equals(".wrl"))
                {
                    string msg = String.Format("Invalid file ({0})\nCan only convert WRL files.", file);
                    MessageBox.Show(msg, "IV Trim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!File.Exists(file))
                {
                    string msg = String.Format("Invalid file ({0})\nFile does not exist!", file);
                    MessageBox.Show(msg, "IV Trim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            string msg2 = String.Format("This will convert {0} file(s).\n\nAre you sure you wish to continue?", filenames.Length);
            if (MessageBox.Show(msg2, "IV Trim", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            //got here, then we are okay
            return true;
        }

        private bool validateModification(string[] filenames)
        {
            if (radioButtonTrim.Checked)
                return validateTrim(filenames);
            else
                return validateConvert(filenames);
        }

        private void ParseMultipleFiles(string[] filenames)
        {
            if (!validateModification(filenames))
                return;

            //okay, lets start the background worker
            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = false;
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            _bgWorker.DoWork += new DoWorkEventHandler(worker_DoWork);
            //set form to be hidden
            _converted = 0;
            _error = new StringBuilder();
            _total = filenames.Length;
            progressBar1.Value = 0;
            progressBar1.Visible = true;
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;
            _bgWorker.RunWorkerAsync(filenames);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //setup worker queue
            string[] filenames = (string[])e.Argument;
            System.Collections.Queue workQueue = new System.Collections.Queue(filenames.Length);
            System.Collections.Queue syncQueue = System.Collections.Queue.Synchronized(workQueue);
            foreach (string file in filenames)
                syncQueue.Enqueue(file);

            //create and start the threads
            int numThreads = Math.Min(System.Environment.ProcessorCount, syncQueue.Count);
            Thread[] threads = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(workThreadWork);
                threads[i].Start(workQueue);
            }
            
            //wait for all the threads to end
            foreach (Thread curThread in threads)
                curThread.Join();
        }


        private void workThreadWork(object queue)
        {
            System.Collections.Queue workQueue = (System.Collections.Queue)queue;
            bool done = false;
            while (!done)
            {
                string nextFile = "";
                lock (workQueue)
                {
                    if (workQueue.Count == 0)
                        return; //all done :)

                    nextFile = (string)workQueue.Dequeue();
                }
                try
                {
                    if (radioButtonTrim.Checked)
                        TrimIVFile.stripFile(nextFile, checkBoxCamera.Checked, checkBoxMaterial.Checked);
                    else
                        VRMLParser.ConvertVRMLToIV(nextFile, checkBoxMimics10.Checked);

                    lock (workQueue)
                        _converted++;
                }
                catch (Exception ex)
                {
                    _error.AppendFormat("Error converting file ({0}), {1}\n", nextFile, ex.Message);
                }
                finally
                {
                    _bgWorker.ReportProgress((int)((_total - workQueue.Count) * 100 / _total));
                }
            }            
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            Console.WriteLine("Progress Changed {0}...",e.ProgressPercentage);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.Enabled = true;
            progressBar1.Value = 100;
            String msg;
            if (radioButtonTrim.Checked)
                msg = String.Format("{0} out of {1} files trimmed.\n\n{2}", _converted, _total, _error.ToString());
            else
                msg = String.Format("{0} out of {1} files converted.\n\n{2}", _converted, _total, _error.ToString());
            MessageBox.Show(msg);
            progressBar1.Visible = false;
        }


        private void panelDropFiles_DragDrop(object sender, DragEventArgs e)
        {
            new System.Security.Permissions.FileIOPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            string[] filenames = e.Data.GetData("FileDrop") as string[];
            try
            {
                ParseMultipleFiles(filenames);                
            }
            catch (Exception ex)
            {
                string msg = "Error loading file(s):\n" + ex.Message;
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                System.Security.CodeAccessPermission.RevertAssert();
            }
        }

        private bool validateExtension(string ext)
        {
            if (radioButtonTrim.Checked)
                return ext.ToLower().Equals(".iv");
            else
                return ext.ToLower().Equals(".wrl");
        }

        private void panelDropFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("FileDrop"))
            {
                string file1 = ((string[])e.Data.GetData("FileDrop"))[0];
                string ext = Path.GetExtension(file1).ToLower();
                if (validateExtension(Path.GetExtension(file1)))
                {
                    e.Effect = DragDropEffects.Move;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;        
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxCamera.Enabled = radioButtonTrim.Checked;
            checkBoxMaterial.Enabled = radioButtonTrim.Checked;
            checkBoxMimics10.Enabled = !radioButtonTrim.Checked;

            if (radioButtonConvert.Checked)
            {
                //in convert mode
                buttonTrim.Text = "Convert";
            }
            else
            {
                buttonTrim.Text = "Trim";
            }
        }

    }
}