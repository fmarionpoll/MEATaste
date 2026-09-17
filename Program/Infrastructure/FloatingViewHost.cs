using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using OxyPlot.Wpf;
using ScottPlot.WPF;

namespace MEATaste.Infrastructure
{
    public class FloatingViewHost
    {
        private readonly Panel host;
        private readonly UIElement content;
        private readonly string title;
        private readonly Action afterReparent;
        private readonly Button restorePlaceholder;
        private Window window;
        private bool shuttingDown;

        public FloatingViewHost(Panel host, UIElement content, string title, Action afterReparent)
        {
            this.host = host;
            this.content = content;
            this.title = title;
            this.afterReparent = afterReparent;
            restorePlaceholder = new Button
            {
                Content = "Open in extra window — click to restore",
                Margin = new Thickness(8),
                Padding = new Thickness(8)
            };
            restorePlaceholder.Click += (_, _) => Redock();
        }

        public bool IsFloating => window != null;

        public void Toggle()
        {
            if (IsFloating)
                Redock();
            else
                Undock();
        }

        public void Undock()
        {
            if (IsFloating)
            {
                window.Activate();
                return;
            }

            host.Children.Remove(content);
            if (!host.Children.Contains(restorePlaceholder))
                host.Children.Add(restorePlaceholder);

            window = new Window
            {
                Title = title,
                Content = content,
                Width = Math.Max(480, host.ActualWidth > 0 ? host.ActualWidth : 640),
                Height = Math.Max(360, host.ActualHeight > 0 ? host.ActualHeight : 480)
            };
            window.Closing += OnWindowClosing;
            window.ContentRendered += OnContentRendered;
            window.Show();
        }

        public void Redock()
        {
            if (window == null) return;

            var floating = window;
            window = null;
            floating.Closing -= OnWindowClosing;
            floating.ContentRendered -= OnContentRendered;
            floating.Content = null;

            RestoreContent();

            if (floating.IsLoaded)
                floating.Close();

            RequestRefresh();
        }

        public void Shutdown()
        {
            shuttingDown = true;
            if (window == null) return;
            var floating = window;
            window = null;
            floating.Closing -= OnWindowClosing;
            floating.ContentRendered -= OnContentRendered;
            floating.Close();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (shuttingDown || window == null) return;
            window = null;
            if (sender is Window floating)
            {
                floating.ContentRendered -= OnContentRendered;
                floating.Content = null;
            }
            RestoreContent();
            RequestRefresh();
        }

        private void RestoreContent()
        {
            host.Children.Remove(restorePlaceholder);
            if (!host.Children.Contains(content))
                host.Children.Add(content);
        }

        private void OnContentRendered(object sender, EventArgs e) => RequestRefresh();

        private void RequestRefresh()
        {
            var target = content;
            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
            {
                afterReparent?.Invoke();
                RefreshPlots(target);
            });
        }

        public static void RefreshPlots(DependencyObject root)
        {
            if (root == null) return;
            switch (root)
            {
                case WpfPlot wpfPlot:
                    wpfPlot.Refresh();
                    break;
                case PlotView plotView:
                    plotView.InvalidatePlot(true);
                    break;
            }

            var n = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < n; i++)
                RefreshPlots(VisualTreeHelper.GetChild(root, i));
        }
    }
}
