using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Windows.Forms;

namespace WristVizualizer
{
    public delegate void VersionUpdatedEventHandler(object sender, VersionUpdatedEventArgs e);

    public class VersionUpdatedEventArgs : EventArgs
    {
        public VersionUpdatedEventArgs()
        {
        }

        /// <summary>
        /// Set to false, to supress the pop-up that normally follows finding an available update.
        /// </summary>
        public bool ShowMessage = true;

    }

    class VersionManager
    {
        private string _url = @"http://www.networkingdoc.com/VersionCheck/WristVizualizer.xml";
        private string _availableVersion;
        private string _downloadLocation;
        private string _currentVersion;
        private bool _newerVersionAvailable = false;

        public event VersionUpdatedEventHandler VersionUpdated;

        public VersionManager()
        {
            _currentVersion = System.Windows.Forms.Application.ProductVersion;
        }

        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        public bool NewerVersionAvailable
        {
            get { return _newerVersionAvailable; }
        }

        public void checkForUpdates()
        {
            XmlDocument xml = new XmlDocument();
            //WebClient client = new WebClient();
            try
            {
                xml.Load(_url);
                _downloadLocation = xml.GetElementsByTagName("ApplicationUrl")[0].InnerText;
                _availableVersion = xml.GetElementsByTagName("AvailableVersion")[0].InnerText;

                Version current = new Version(_currentVersion);
                Version available = new Version(_availableVersion);
                if (availableVersionIsNewer(current, available))
                {
                    newUpdateAvailable();
                }
                else 
                {
                    string msgBoxMesssage = String.Format("You have the most recent version of {0}", 
                        System.Windows.Forms.Application.ProductName);
                    MessageBox.Show(msgBoxMesssage);
                }

            }
            catch (Exception ex)
            {
                string msgBoxMesssage = String.Format("Error checking for latest version of {0}\n{1}", 
                    System.Windows.Forms.Application.ProductName,
                    ex.Message);
                MessageBox.Show(msgBoxMesssage);
            }
        }

        private static bool availableVersionIsNewer(Version current, Version available)
        {
            if (available.Major > current.Major) return true;
            if (available.Major == current.Major && available.Minor > current.Minor) return true;
            if (available.Major == current.Major && available.Minor == current.Minor
                && available.Build > current.Build) return true;

            return false; //don't want to test the revsion
        }

        public void checkForUpdatesAsynch()
        {
            
            WebClient client = new WebClient();
            try
            {
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                client.OpenReadAsync(new Uri(_url));
            }
            catch { }
        }

        private void newUpdateAvailable()
        {
            _newerVersionAvailable = true;

            VersionUpdatedEventArgs eventArgs = new VersionUpdatedEventArgs();
            if (VersionUpdated != null)
                VersionUpdated(this, eventArgs);

            if (!eventArgs.ShowMessage) //check if the popup as been canceled.
                return;

            string msgBoxMesssage = String.Format("Version {0} of {1} is available. Would you like to download this update?",
                _availableVersion, System.Windows.Forms.Application.ProductName);
            string msgBoxCaption = System.Windows.Forms.Application.ProductName;
            DialogResult result = MessageBox.Show(
                msgBoxMesssage,
                msgBoxCaption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(_downloadLocation);
            }

        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(e.Result);
                _downloadLocation = xml.GetElementsByTagName("ApplicationUrl")[0].InnerText;
                _availableVersion = xml.GetElementsByTagName("AvailableVersion")[0].InnerText;

                Version current = new Version(_currentVersion);
                Version available = new Version(_availableVersion);
                if (availableVersionIsNewer(current, available))
                {
                    newUpdateAvailable();
                }
            }
            catch { }
        }

    }

}
