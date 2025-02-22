using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.bag
{

    internal class Item
    {
        public int ID;
        public int weight;
        public int value;

        public Color color;
        public Border border;

        public Item(int ID, int capacity)
        {
            this.ID = ID;
            this.color = Random_Generator.NextDarkColor();
            weight = Random_Generator.NextSmallerRandom(1, capacity);
            value = Random_Generator.NextSmallerRandom(1, capacity);
            border = createItemBorder();
        }

        public Item(int ID, int weight, int value)
        {
            this.ID = ID;
            this.color = Random_Generator.NextDarkColor();
            this.weight = weight;
            this.value = value;
            border = createItemBorder();
        }

        public Item(int ID, Item item)
        {
            this.ID = ID;
            this.color = item.color;
            this.weight = item.weight;
            this.value = item.value;
            border = createItemBorder();
        }

        public Border createItemBorder()
        {
            Border border = new Border();
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            StackPanel outsideStackPanel = new StackPanel();
            outsideStackPanel.Height = 80;
            outsideStackPanel.Width = 80;
            border.Child = outsideStackPanel;

            TextBlock itemNameText = new TextBlock();
            itemNameText.Text = getName();
            itemNameText.FontSize = 13;
            itemNameText.Foreground = new SolidColorBrush(color);
            itemNameText.Margin = new Thickness(5);
            itemNameText.HorizontalAlignment = HorizontalAlignment.Left;
            itemNameText.FontWeight = FontWeights.Bold;
            outsideStackPanel.Children.Add(itemNameText);

            StackPanel itemValueStackPanel = new StackPanel();
            itemValueStackPanel.Orientation = Orientation.Horizontal;
            itemValueStackPanel.Margin = new Thickness(3);
            itemValueStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            outsideStackPanel.Children.Add(itemValueStackPanel);

            TextBlock itemValueDescribeText = new TextBlock();
            itemValueDescribeText.Text = "价值";
            itemValueDescribeText.FontSize = 12;
            itemValueStackPanel.Children.Add(itemValueDescribeText);

            TextBlock itemValueText = new TextBlock();
            itemValueText.Text = value.ToString();
            itemValueText.FontSize = 12;
            itemValueText.Width = 35;
            itemValueText.TextAlignment = TextAlignment.Center;
            itemValueStackPanel.Children.Add(itemValueText);

            StackPanel itemWeightStackPanel = new StackPanel();
            itemWeightStackPanel.Orientation = Orientation.Horizontal;
            itemWeightStackPanel.Margin = new Thickness(3);
            itemWeightStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            outsideStackPanel.Children.Add(itemWeightStackPanel);

            TextBlock itemWeightDescribeText = new TextBlock();
            itemWeightDescribeText.Text = "重量";
            itemWeightDescribeText.FontSize = 12;
            itemWeightStackPanel.Children.Add(itemWeightDescribeText);

            TextBlock itemWeightText = new TextBlock();
            itemWeightText.Text = weight.ToString();
            itemWeightText.FontSize = 12;
            itemWeightText.Width = 35;
            itemWeightText.TextAlignment = TextAlignment.Center;
            itemWeightStackPanel.Children.Add(itemWeightText);

            return border;
        }

        public void setTakeAwayStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 128, 0));
        }

        public void setPushBackStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255));
        }

        public void setUnuseStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        public void setInUseStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(102, 204, 0));
        }

        public void setFullFillStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(128, 255, 0));
        }

        public void setOverCapacityStatus()
        {
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }

        public string getName()
        {
            return "物品" + ID;
        }
    }
}
