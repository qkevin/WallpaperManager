using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace WallpaperManager
{
    public class WidthCaculatorConverter : IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double boxWidth = (double)value;
            return boxWidth - SystemParameters.VerticalScrollBarWidth;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //why i use this complicated methods to get the scrollbar actual width is 
            //because just incase someone is using custom scrollbar style
            //If just use system default scrollbar then SystemParameters.VerticalScrollBarWidth is enough
            double boxWidth = (double)values[0];
            var control = values[1] as DependencyObject;
            var sviewer = GetVisualParent<ScrollViewer>(control);
            var sbar= GetVisualChild<ScrollBar>(sviewer);
            return boxWidth - sbar.ActualWidth;
        }

        private T GetVisualParent<T>(DependencyObject current) where T: DependencyObject
        {
          var parent =  VisualTreeHelper.GetParent(current);
            if (parent != null)
            {
                if (parent is T)
                {
                    return parent as T;
                }
                else
                {
                    return GetVisualParent<T>(parent);
                }
            }
            else
            {
                return null;
            }
        }

        private T GetVisualChild<T>(DependencyObject current) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                if (child is T)
                {
                    return child as T;
                }
                else
                {
                    var grandChild = GetVisualChild<T>(child);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
