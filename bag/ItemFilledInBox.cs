using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp1.bag
{
    internal class ItemFilledInBox
    {
        public StackPanel itemBlockStackPanel;
        DoubleAnimation animation;
        Storyboard storyboard;
        MainWindow window;
        public Item item;
        public double canvasLeft;
        public double width;
        public TimeSpan animationDuration;

        public ItemFilledInBox(Item item, BoxOfItemBlock boxOfItemBlock, MainWindow window, string blockName, bool appearAnimation)
        {
            this.window = window;
            this.item = item;

            width = boxOfItemBlock.scaleLength * item.weight;
            if (width > boxOfItemBlock.getUnFilledLength() + 25)
            {
                width = boxOfItemBlock.getUnFilledLength() + 25;
            }
            canvasLeft = boxOfItemBlock.filledLength;

            itemBlockStackPanel = new StackPanel();
            itemBlockStackPanel.Name = item.getName() + blockName;
            itemBlockStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            itemBlockStackPanel.Width = 0;
            Canvas.SetLeft(itemBlockStackPanel, canvasLeft);
            Canvas.SetTop(itemBlockStackPanel, -15);

            string name = itemBlockStackPanel.Name;
            window.RegisterName(itemBlockStackPanel.Name, itemBlockStackPanel);
            boxOfItemBlock.box.Children.Add(itemBlockStackPanel);

            TextBlock itemNameText = new TextBlock();
            itemNameText.Text = item.getName();
            itemNameText.FontSize = 9;
            itemNameText.Margin = new System.Windows.Thickness(0, 0, 0, 3);
            itemBlockStackPanel.Children.Add(itemNameText);

            TextBlock itemBlock = new TextBlock();
            itemBlock.Height = 21;
            itemBlock.Width = width;
            itemBlock.Background = new SolidColorBrush(item.color);
            itemBlockStackPanel.Children.Add(itemBlock);

            animation = new DoubleAnimation();
            animationDuration = TimeSpan.FromSeconds(width / boxOfItemBlock.boxLength / window.animationSpeed);
            animation.Duration = new System.Windows.Duration(animationDuration);
            Storyboard.SetTargetName(animation, itemBlockStackPanel.Name);
            Storyboard.SetTargetProperty(animation, new System.Windows.PropertyPath(StackPanel.WidthProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            appear(appearAnimation);
        }

        public ItemFilledInBox(Item item, BoxOfItemBlock boxOfItemBlock, MainWindow window, string blockName)
        {
            this.window = window;
            this.item = item;

            width = boxOfItemBlock.scaleLength * item.weight;
            if (width > boxOfItemBlock.getUnFilledLength() + 25)
            {
                width = boxOfItemBlock.getUnFilledLength() + 25;
            }
            canvasLeft = boxOfItemBlock.filledLength;

            itemBlockStackPanel = new StackPanel();
            itemBlockStackPanel.Name = item.getName() + blockName;
            itemBlockStackPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            itemBlockStackPanel.Width = 0;
            Canvas.SetLeft(itemBlockStackPanel, canvasLeft);
            Canvas.SetTop(itemBlockStackPanel, -15);

            string name = itemBlockStackPanel.Name;
            window.RegisterName(itemBlockStackPanel.Name, itemBlockStackPanel);
            boxOfItemBlock.box.Children.Add(itemBlockStackPanel);

            TextBlock itemNameText = new TextBlock();
            itemNameText.Text = item.getName();
            itemNameText.FontSize = 9;
            itemNameText.Margin = new System.Windows.Thickness(0, 0, 0, 3);
            itemBlockStackPanel.Children.Add(itemNameText);

            TextBlock itemBlock = new TextBlock();
            itemBlock.Height = 21;
            itemBlock.Width = width;
            itemBlock.Background = new SolidColorBrush(item.color);
            itemBlockStackPanel.Children.Add(itemBlock);

            animation = new DoubleAnimation();
            animationDuration = TimeSpan.FromSeconds(width / boxOfItemBlock.boxLength / window.animationSpeed);
            animation.Duration = new System.Windows.Duration(animationDuration);
            Storyboard.SetTargetName(animation, itemBlockStackPanel.Name);
            Storyboard.SetTargetProperty(animation, new System.Windows.PropertyPath(StackPanel.WidthProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(animation);
        }


        public void appear(bool withAnimation)
        {
            if (withAnimation)
            {
                animation.From = 0;
                animation.To = width;
                storyboard.Begin(window);
            }
            else
            {
                itemBlockStackPanel.Width = width;
            }
        }

        public void appearAfterDelay(TimeSpan delay)
        {
            animation.From = 0;
            animation.To = width;
            storyboard.BeginTime = delay;
            storyboard.Begin(window);
        }
        public void disappearAfterDelay(TimeSpan delay)
        {
            animation.From = width;
            animation.To = 0;
            storyboard.BeginTime = delay;
            storyboard.Begin(window);
            ClearName();
        }

        public void disappear(bool withAnimation)
        {
            if (withAnimation)
            {
                animation.From = width;
                animation.To = 0;
                storyboard.Begin(window);
                ClearName();
            }
            else
            {
                itemBlockStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                ClearName();
            }
        }

        public void ClearName()
        {
            window.UnregisterName(itemBlockStackPanel.Name);
        }
    }
}
