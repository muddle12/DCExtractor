using System.ComponentModel;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "DCBackgroundWorker"
    /// 
    /// Purpose :   a wrapper for the System.ComponentModel.BackgroundWorker, designed to interface with the DC.IO namespace
    /// to deliver progress reports on the data extraction without flooding those namespaces with tons of winforms noise.
    /// 
    /// It works like this: DC.IO functions report their progress to DCProgress, who then reports this to DCBackgroundWorker,
    /// who turns around and reports this to the DCExtractorForm, who then invokes its own reporting functions to update
    /// its progress bar.
    /// 
    /// I tried to make this as simple and reusable as possible. However, it's threading, so of course it'll be confusing.
    /// </summary>
    public class DCBackgroundWorker
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Delegates.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public delegate void WorkerReportingHandler(string szValue);
        public delegate void WorkerValueChangedHandler(int nValue);


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Events.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public event System.EventHandler WorkerStarted;
        public event System.EventHandler WorkerCanceled;
        public event WorkerReportingHandler ProgressNameChanged;
        public event WorkerValueChangedHandler ProgressChanged;
        public event WorkerValueChangedHandler MaximumChanged;
        public event System.EventHandler WorkerFinished;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        BackgroundWorker m_tWorker = null;
        bool m_bWorking = false;
        bool m_bCanWork = true;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets whether or not this background worker is currently working.
        /// </summary>
        public bool isWorking
        {
            get { return m_bWorking; }
        }

        /// <summary>
        /// Gets whether or not this worker is able to work.
        /// </summary>
        public bool canWork
        {
            get { return m_bCanWork && m_bWorking == false; }
            set { m_bCanWork = value; }
        }

        /// <summary>
        /// Sets the current progress name for this worker.
        /// </summary>
        public string progressName
        {
            set
            {
                if (ProgressNameChanged != null)
                    ProgressNameChanged.Invoke(value);
            }
        }

        /// <summary>
        /// Sets the progress value for this worker.
        /// </summary>
        public int progressValue
        {
            set
            {
                if (ProgressChanged != null && isWorking)
                    ProgressChanged.Invoke(value);
            }
        }

        /// <summary>
        /// Sets the progress maximum for this worker.
        /// </summary>
        public int progressMaximum
        {
            set
            {
                if (MaximumChanged != null && isWorking)
                    MaximumChanged.Invoke(value);
            }
        }

        /// <summary>
        /// Gets whether or not the work for this worker has been canceled.
        /// </summary>
        public bool canceled
        {
            get { return m_tWorker.CancellationPending; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workFunc">The work function this worker will use.</param>
        /// <param name="szFunctionName">The name of the function to display on the progress bar.</param>
        /// <param name="progressChangedFunc">The function that this worker will report to when the progress changes.</param>
        /// <param name="maximumChangedFunc">The function that this worker will report to when the maximum progress value changes.</param>
        public DCBackgroundWorker(DoWorkEventHandler workFunc)
        {
            //Create our worker and setup its reporting events.
            m_tWorker = new BackgroundWorker();
            m_tWorker.DoWork += workFunc;
            m_tWorker.RunWorkerCompleted += OnWorkComplete;
            m_tWorker.WorkerReportsProgress = true;
            m_tWorker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Runs the worker, if it canWork.
        /// </summary>
        /// <param name="szParameters">The parameters to pass the worker function.</param>
        public void Run(string[] szParameters)
        {
            if (canWork)
            {
                DCProgress.worker = this;
                if (WorkerStarted != null)
                    WorkerStarted.Invoke(this, System.EventArgs.Empty);
                m_tWorker.RunWorkerAsync(szParameters);
                m_bWorking = true;
            }
        }

        /// <summary>
        /// Cancels the current task, if there is one.
        /// </summary>
        public void Cancel()
        {
            if (isWorking)
            {
                m_tWorker.CancelAsync();
                if (WorkerCanceled != null)
                    WorkerCanceled.Invoke(this, System.EventArgs.Empty);
                canWork = true;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Event Handlers.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Fires when the worker completes its work.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The evnet arguments.</param>
        void OnWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            m_bWorking = false;
            if (WorkerFinished != null)
                WorkerFinished.Invoke(sender, System.EventArgs.Empty);
        }
    }
}
