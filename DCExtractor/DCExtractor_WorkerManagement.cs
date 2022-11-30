using DC.IO;
using System;
using System.Windows.Forms;

namespace DCExtractor
{
    /// <summary>
    /// Class   :   "DCExtractorForm_WorkerManagement"
    /// 
    /// Purpose :   in order to make the threading more manageable, I've split it off into its own
    /// file. It's still part of the form, it's just compartmentalized in this file and we're 
    /// "pretending" it's a separate system. I did this so others poking around in the form will
    /// have an easier time finding where to add new buttons.
    /// </summary>
    public partial class DCExtractorForm : Form
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Our list of workers.
        DCBackgroundWorker[] workers;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets whether or not the form is doing work.
        /// </summary>
        private bool isWorking
        {
            get
            {
                for (int i = 0; i < workers.Length; i++)
                {
                    if (workers[i].isWorking)
                        return true;
                }
                return false;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Finishes the initialization of workers.
        /// </summary>
        void FinishInitializingWorkers()
        {
            //Loop through and assign the event handlers.
            for (int i = 0; i < workers.Length; i++)
            {
                workers[i].WorkerStarted += OnWorkerStarted;
                workers[i].WorkerCanceled += OnWorkerCanceled;
                workers[i].ProgressNameChanged += OnWorkerProgressNameChanged;
                workers[i].ProgressChanged += OnWorkerProgressChanged;
                workers[i].MaximumChanged += OnWorkerMaximumChanged;
                workers[i].WorkerFinished += OnWorkerFinished;
            }
        }

        /// <summary>
        /// Starts work for the form.
        /// </summary>
        /// <param name="tWorker">The worker to use.</param>
        /// <param name="szParameters">The parameters to pass the worker.</param>
        void StartWork(DCBackgroundWorker tWorker, string[] szParameters)
        {
            bool bFound = false;
            for (int i = 0; i < workers.Length; i++)
            {
                if (tWorker == workers[i])
                {
                    bFound = true;
                    break;
                }
            }
            if (bFound == false)
                throw new InvalidOperationException("The worker you are attempting to start has not been added to the workers list! See DCExtractor_Operations.InitializeWorkers");

            if (isWorking == false)
            {
                ToggleButtons(false);
                tWorker.Run(szParameters);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Event Handlers.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when a worker starts working.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWorkerStarted(object sender, EventArgs e)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.ToggleButtons(false)));
            else
                ToggleButtons(false);
        }

        /// <summary>
        /// Called when a worker has its work canceled.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWorkerCanceled(object sender, EventArgs e)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.ToggleButtons(true)));
            else
                ToggleButtons(true);
        }

        /// <summary>
        /// Called when a worker thread updates its progress name.
        /// </summary>
        /// <param name="nValue">The current progress value.</param>
        private void OnWorkerProgressNameChanged(string szValue)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => SetProgressName(szValue)));
            else
                ToggleButtons(true);
        }

        /// <summary>
        /// Sets the progress name.
        /// </summary>
        /// <param name="szName">The name to set the progress name to.</param>
        private void SetProgressName(string szName)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            LB_progressText.Text = szName;
        }

        /// <summary>
        /// Called when a worker thread updates its progress.
        /// </summary>
        /// <param name="nValue">The current progress value.</param>
        private void OnWorkerProgressChanged(int nValue)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.OnWorkerProgressChanged(nValue)));
            else
                PB_progress.Value = nValue;
        }

        /// <summary>
        /// Called when a worker thread updates the progress bar's maximum value.
        /// </summary>
        /// <param name="nValue">The maximum value.</param>
        private void OnWorkerMaximumChanged(int nValue)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.OnWorkerMaximumChanged(nValue)));
            else
                PB_progress.Maximum = nValue;
        }

        /// <summary>
        /// Called when a worker starts finishes.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWorkerFinished(object sender, EventArgs e)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            if (this.InvokeRequired)
                this.Invoke(new Action(() => this.ToggleButtons(true)));
            else
                ToggleButtons(true);
        }

        /// <summary>
        /// Fires when the cancel work button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void BT_cancelWork_Click(object sender, EventArgs e)
        {
            if (m_bIsClosing || this.IsDisposed)
                return;
            for (int i = 0; i < workers.Length; i++)
            {
                if (workers[i].isWorking)
                    workers[i].Cancel();
            }
        }
    }
}
