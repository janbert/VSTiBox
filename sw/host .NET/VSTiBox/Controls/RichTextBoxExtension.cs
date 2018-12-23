using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox.Controls
{
    /// <summary>
    /// RichTextBoxExtension adds the ability to easily append and format text.
    /// </summary>
    public static class RichTextBoxExtension
    {
        /// <summary>
        /// Appends the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="text">The text.</param>
        public static void AppendSelection(this RichTextBox control, string text)
        {
            int len = control.TextLength;

            // Append the text.
            control.AppendText(text);

            // Prepare it for formatting.
            control.SelectionStart = len;
            control.SelectionLength = text.Length;

            // Scroll to it.
            control.ScrollToCaret();
        }

        /// <summary>
        /// Appends the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="text">The text.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="font">The font.</param>
        public static void AppendSelection(this RichTextBox control, string text, Color colour, Font font)
        {
            AppendSelection(control, text);
            control.SelectionColor = colour;
            control.SelectionFont = font;
        }

        /// <summary>
        /// Appends the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="text">The text.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="font">The font.</param>
        public static void AppendLog(this RichTextBox control, string text, Color colour, Font font)
        {
            Action append = () => AppendSelection(control, text, colour, font);
            if (control.InvokeRequired)
                control.Invoke(append);
            else
                append();
        }
    }
}
