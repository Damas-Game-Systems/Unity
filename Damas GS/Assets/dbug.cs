using UnityEngine;
using System.Collections.Generic;

namespace Damas.Utils
{
    [System.Serializable]
    public class dbug
    {
        private const string tooLongMsg =
            "Message was over char limit. Truncted: ";

        [SerializeField] private bool show = true;

        private int tooLong = 500;
        private int maxReportSize = 10;

        private Queue<string> messages;
        private string report = "";

        public void print()
        {
            print();
        }

        public void print(string msg)
        {
            if (!show) { return; }
            Debug.Log(trim(msg));
        }

        public void warn()
        {
            warn(report);
        }

        public void warn(string msg)
        {
            if (!show) { return; }
            Debug.LogWarning(msg);
        }

        public void error()
        {
            error(report);
        }

        public void error(string msg)
        {
            if (!show) { return; }
            Debug.LogError(trim(msg));
        }

        public void add(string msg)
        {
            if (messages.Count == maxReportSize)
            {
                warn($"The recent message just added " +
                    $" to {this} report resulted in a " +
                    $"report of over {maxReportSize}" +
                    $"lines. This and further msgs wont be added");
                return;
            }

            messages.Enqueue(trim(msg));
        }

        private string generateReport()
        {
            return string.Join('\n', messages);
        }

        private string trim(string msg)
        {
            return isTooLong(msg)
                ? msg
                : (tooLong + msg)[..tooLong];
        }

        private bool isTooLong(string msg)
        {
            return msg.Length > tooLong;
        }
    }
}
