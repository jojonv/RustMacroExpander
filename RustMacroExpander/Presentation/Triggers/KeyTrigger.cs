using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace RustMacroExpander.Presentation.Triggers
{
    public class KeyTrigger : TriggerBase<UIElement>
    {
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyTrigger));

        public event EventHandler CanExecuteChanged;

        protected override void OnAttached()
        {

            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            base.OnAttached();
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
            {
                InvokeActions(e);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            base.OnDetaching();
        }
    }

}
