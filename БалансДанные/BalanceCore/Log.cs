using System;
using System.Drawing;
using System.Windows.Forms;

namespace BalanceCore
{
    /// <summary>
    /// Log class with attached RichTextBox
    /// </summary>
    public class Log
    {
        private RichTextBox log;

        /// <summary>
        /// Log ctor
        /// </summary>
        /// <param name="log">External RichTextBox element to be attached</param>
        public Log(RichTextBox log)
        {
            this.log = log;
            this.log.ReadOnly = true; //Log editing restriction
        }

        /// <summary>
        /// Add message to Log
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="type">Message type</param>
        public void AddMessage(string text, MessageType type = MessageType.Info)
        {
            Color color = type == MessageType.Info
                            ? Color.Black
                            : type == MessageType.Success
                                ? Color.DarkGreen
                                : Color.Red;
            log.SuspendLayout();
            log.SelectionColor = color;
            log.AppendText($"{text}{Environment.NewLine}");
            log.ScrollToCaret();
            log.ResumeLayout();
        }

        /// <summary>
        /// Log message type enumeration
        /// </summary>
        public enum MessageType
        {
            Info = 0,       //Information message - black color text
            Success = 1,    //Success message - dark-green text
            Error = 2       //Error message - red color text
        }
    }
}