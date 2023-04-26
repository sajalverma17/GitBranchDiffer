using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using static Microsoft.VisualStudio.Shell.RegistrationAttribute;
using System.Windows.Forms;
using System.Windows.Input;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using Key = System.Windows.Input.Key;

namespace BranchDiffer.VS.Shared.FileDiff.Navigation
{
    [Export(typeof(IKeyProcessorProvider))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.EmbeddedPeekTextView)]
    [ContentType("code")]
    [Name("GitBranchDifferNavigation")]
    [Order(Before = "VisualStudioKeyboardProcessor")]
    public class F12KeyProcessorProvider : IKeyProcessorProvider
    {
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new F12KeyProcessor(wpfTextView);
        }
    }

    internal class F12KeyProcessor : KeyProcessor
    {
        private readonly ITextView _textView;

        public F12KeyProcessor(ITextView textView)
        {
            _textView = textView;
        }

        public override void KeyDown(System.Windows.Input.KeyEventArgs args)
        {
            if (IsF12)
            {
                if (_textView.Caret != null)
                {                    
                }
            }
        }

        public bool IsF12
        {
            get { return Keyboard.IsKeyDown(Key.F12) || Keyboard.IsKeyDown(Key.F12); }
        }
    }
}
