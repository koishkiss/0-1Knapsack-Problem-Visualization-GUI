using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.bag.bag_operators;

namespace WpfApp1.bag
{
    internal class BoxOfItemBlock
    {
        public Canvas box;
        public double boxLength;
        public double filledLength;
        public double scaleLength;
        public Stack<ItemFilledInBox> items;
        public string blockName;

        public BoxOfItemBlock(Canvas container, int capacity, string blockName)
        {
            double totalWidth = container.ActualWidth;
            double totalHeight = container.ActualHeight;

            double thickness = 2;
            double canvasLeft = 20;
            double canvasTop = totalHeight / 2 - 20;

            boxLength = capacity * 31;
            boxLength = totalWidth * 0.9 > boxLength ? boxLength : totalWidth * 0.9;
            scaleLength = boxLength / capacity;

            // 背包现状
            container.Children.Clear();

            Canvas itemFilled = new Canvas();
            Canvas.SetLeft(itemFilled, canvasLeft + thickness);
            Canvas.SetTop(itemFilled, canvasTop + thickness);
            container.Children.Add(itemFilled);
            box = itemFilled;

            Border bagBorder = new Border();
            bagBorder.BorderThickness = new Thickness(thickness);
            bagBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(bagBorder, canvasLeft);
            Canvas.SetTop(bagBorder, canvasTop);
            container.Children.Add(bagBorder);

            StackPanel bagStackPanel = new StackPanel();
            bagStackPanel.Height = 20;
            bagStackPanel.Width = boxLength;
            bagBorder.Child = bagStackPanel;

            filledLength = 0;
            items = new();
            this.blockName = blockName;
        }

        public double getUnFilledLength()
        {
            return boxLength - filledLength;
        }

        public void setItem(Item item, MainWindow window)
        {
            ItemFilledInBox newItemBlock = new ItemFilledInBox(item, this, window, blockName, false);
            filledLength += newItemBlock.width;
            items.Push(newItemBlock);
        }

        public void setAllItem(List<Item> items,  MainWindow window)
        {
            foreach (Item item in items)
            {
                setItem(item, window);
            }
        }

        public void fillItem(Item item, MainWindow window)
        {
            lock (this)
            {
                ItemFilledInBox newItemBlock = new ItemFilledInBox(item, this, window, blockName, BagOperatorStack.showAnimation);
                filledLength += newItemBlock.width;
                items.Push(newItemBlock);
            }
        }

        public void fillAllItem(List<Item> itemList, MainWindow window)
        {
            lock (this)
            {
                TimeSpan delay = TimeSpan.Zero;
                foreach (Item item in itemList)
                {
                    ItemFilledInBox newItemBlock = new ItemFilledInBox(item, this, window, blockName);
                    filledLength += newItemBlock.width;
                    items.Push(newItemBlock);
                    if (BagOperatorStack.showAnimation)
                    {
                        newItemBlock.appearAfterDelay(delay);
                        delay = delay.Add(newItemBlock.animationDuration);
                    } 
                }
            }
        }

        public void unFillItem()
        {
            lock (this)
            {
                ItemFilledInBox item = items.Pop();
                filledLength -= item.width;
                item.disappear(BagOperatorStack.showAnimation);
            }
        }

        public void unFillAllItem(int num)
        {
            lock(this)
            {
                TimeSpan delay = TimeSpan.Zero;
                while (num > 0)
                {
                    ItemFilledInBox item = items.Pop();
                    filledLength -= item.width;
                    if (BagOperatorStack.showAnimation)
                    {
                        item.disappearAfterDelay(delay);
                        delay = delay.Add(item.animationDuration);
                    } 
                    else
                    {
                        item.disappear(false);
                    }
                    num--;

                }
            }
        }

        public void resetAllItem(List<Item> newItems, MainWindow window)
        {
            foreach (ItemFilledInBox item in items)
            {
                item.disappear(false);
            }
            items.Clear();
            filledLength = 0;
            foreach (Item item in newItems)
            {
                setItem(item, window);
            }
        }

        public void setAllItemInUse()
        {
            foreach (ItemFilledInBox item in items)
            {
                item.item.setInUseStatus();
            }
        }

        public void setAllItemUnuse()
        {
            foreach (ItemFilledInBox item in items)
            {
                item.item.setUnuseStatus();
            }
        }
    }
}
