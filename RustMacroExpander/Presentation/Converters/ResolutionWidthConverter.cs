using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;

namespace RustMacroExpander.Presentation.Converters
{
    public class ResolutionWidthConverter : IValueConverter
    {
        double GetCurrentScreenWidth()
        {
            Point pos = Cursor.Position;
            var screen = Screen.FromPoint(pos);

            return screen.Bounds.Width;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currWidth = (Double)value;
            var realWidth = GetCurrentScreenWidth();
            return currWidth <= realWidth ? value : realWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
