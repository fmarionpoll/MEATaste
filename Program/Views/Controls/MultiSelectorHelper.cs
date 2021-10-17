using System;
using System.Collections;
using System.Reflection;
using System.Windows.Controls.Primitives;

// adapted from Illya Reznykov https://github.com/IReznykov/Blog

namespace MEATaste.Views.Controls
{
    public static class MultiSelectorHelper
    {
        private static readonly PropertyInfo _piIsUpdatingSelectedItems;
        private static readonly MethodInfo _miBeginUpdateSelectedItems;
        private static readonly MethodInfo _miEndUpdateSelectedItems;

        static MultiSelectorHelper()
        {
            _piIsUpdatingSelectedItems = typeof(MultiSelector).GetProperty("IsUpdatingSelectedItems", BindingFlags.NonPublic | BindingFlags.Instance);
            _miBeginUpdateSelectedItems = typeof(MultiSelector).GetMethod("BeginUpdateSelectedItems", BindingFlags.NonPublic | BindingFlags.Instance);
            _miEndUpdateSelectedItems = typeof(MultiSelector).GetMethod("EndUpdateSelectedItems", BindingFlags.NonPublic | BindingFlags.Instance);
        }


        public static void SelectManyItems(this MultiSelector control, IEnumerable itemsToBeSelected)
        {
            control.Dispatcher.Invoke(
                (Action)(() =>
                {
                    if (!(bool)_piIsUpdatingSelectedItems.GetValue(control, null))
                    {
                        _miBeginUpdateSelectedItems.Invoke(control, null);
                        try
                        {
                            foreach (object item in itemsToBeSelected)
                                control.SelectedItems.Add(item);
                        }
                        finally
                        {
                            _miEndUpdateSelectedItems.Invoke(control, null);
                        }
                    }
                })
            );
        }

        public static void UnSelectManyItems(this MultiSelector control, IEnumerable itemsToBeUnSelected)
        {
            control.Dispatcher.Invoke(
                (Action)(() =>
                {
                    if (!(bool)_piIsUpdatingSelectedItems.GetValue(control, null))
                    {
                        _miBeginUpdateSelectedItems.Invoke(control, null);
                        try
                        {
                            foreach (object item in itemsToBeUnSelected)
                                control.SelectedItems.Remove(item);
                        }
                        finally
                        {
                            _miEndUpdateSelectedItems.Invoke(control, null);
                        }
                    }
                })
            );
        }


    }
}
