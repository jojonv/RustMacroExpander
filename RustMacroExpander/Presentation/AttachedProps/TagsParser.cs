using RustMacroExpander.Helpers;
using RustMacroExpander.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;

namespace RustMacroExpander.Presentation.AttachedProps
{
    public class TagsParser
    {
        public static XDocument GetContent(DependencyObject obj)
        {
            return (XDocument)obj.GetValue(ContentProperty);
        }

        public static void SetContent(DependencyObject obj, XDocument value)
        {
            obj.SetValue(ContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached(
                "Content"
                , typeof(XDocument)
                , typeof(TagsParser)
                , new PropertyMetadata(null, ContentChanged));

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBlock;
            // Clean inlines
            //
            var doc = e.NewValue as XDocument;
            if (tb == null || doc == null) return;
            tb.Inlines.Clear();


            foreach (var n in doc.Root.Descendants())
            {
                var r = new Run($"{n.Value}{Environment.NewLine}");
                if (n.Name.LocalName.Equals(Tags.Error.ToString()))
                    r.Background = Brushes.Red;
                tb.Inlines.Add(r);
            }
        }
    }
}
