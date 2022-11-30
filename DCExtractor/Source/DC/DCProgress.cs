namespace DC.IO
{
    /// <summary>
    /// Class   :   "DCProgress"
    /// 
    /// Purpose :   a bit overkill, but this static class allows the DC.IO functions to report their progress remotely
    /// without making the loading functions incredibly bloated. With this, we can report anywhere in any load function
    /// and not have to pass the progress up the chain, through a reference object, or with a obtuse wrapper.
    /// </summary>
    public static class DCProgress
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static DCBackgroundWorker m_tWorker;
        static string m_szName;
        static int m_nValue = 0;
        static int m_nMaximum = 1;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets/Sets the current worker for this progress.
        /// </summary>
        public static DCBackgroundWorker worker
        {
            get { return m_tWorker; }
            set { m_tWorker = value; }
        }

        /// <summary>
        /// Gets/Sets the value of the progress bar.
        /// </summary>
        public static int value
        {
            get { return m_nValue; }
            set
            {
                m_nValue = value;
                m_tWorker.progressValue = value;
            }
        }

        /// <summary>
        /// Gets/Sets the maximum of the progress bar.
        /// </summary>
        public static int maximum
        {
            get { return m_nMaximum; }
            set
            {
                m_nMaximum = value;
                m_tWorker.progressMaximum = value;
            }
        }

        /// <summary>
        /// Gets/Sets the name of the current phase of the operation.
        /// </summary>
        public static string name
        {
            get { return m_szName; }
            set
            {
                m_szName = value;
                m_tWorker.progressName = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether or not the progress has been canceled.
        /// </summary>
        public static bool canceled
        {
            get { return m_tWorker.canceled; }
        }
    }
}
