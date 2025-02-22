using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.bag.bag_operators
{
    internal class BagOperatorStack
    {
        public static Stack<int> itemIndexStack = new();
        public static List<BagOperator> operatorStack = new();
        public static int cur = 0;
        public static List<Item> precent_max_value_item_list = new();
        public static bool showAnimation = true;
        public static MaxValue[,] maxValueTable = new MaxValue[0,0];  // [剩余背包容量 0 - capacity , 递归深度 0 - item_num]

        public static bool skipItemOverCapacityStatus = false;
        public static bool keepTryAfterFullFillStatus = false;
        public static bool useTableToReduceStepStatus = true;
        public static bool disposableFillWhileCapacityEnoughStatus = false;

        // 这个构造器的意义在于重置static变量，也就是没有什么意义
        public BagOperatorStack(Bag_Problem Bag)
        {
            operatorStack.Clear();
            operatorStack.Add(new BeforeStartOperator(Bag));
            cur = 0;
            itemIndexStack.Clear();
            precent_max_value_item_list = new();
            maxValueTable = new MaxValue[Bag.capacity + 1, Bag.left_item_num + 1];
        }

        public static bool judgeIfNextStepNotGenerate()
        {
            return cur + 1 >= operatorStack.Count;
        }

        public static void jumpToOtherBranch(Bag_Problem Bag, int index)
        {
            // 尝试从堆栈中选择不同的路线
            if (itemIndexStack.Count == 0)
            {
                MaxValue maxValueNow = maxValueTable[Bag.left_capacity, index];
                if (maxValueNow == null)
                {
                    maxValueNow = new MaxValue(0, 0, new());
                }
                for (int nextIndex = index - 1; nextIndex > -1; nextIndex--)
                {
                    MaxValue maxValueNext = maxValueTable[Bag.left_capacity, nextIndex];
                    if (maxValueNext == null || maxValueNext.total_value < maxValueNow.total_value)
                    {
                        maxValueTable[Bag.left_capacity, nextIndex] = maxValueNow;
                    }
                    else
                    {
                        maxValueNow = maxValueNext;
                    }
                }
                // 路线已经选择完毕，放入结尾步骤
                operatorStack.Add(new AfterEndOperator(Bag));
            }
            else if (useTableToReduceStepStatus)
            {
                //在选择不同路径前需要更改MaxValueTable
                int indexTurnTo = itemIndexStack.Pop();
                MaxValue maxValueNow = maxValueTable[Bag.left_capacity, index];
                if (maxValueNow == null)
                {
                    maxValueNow = new MaxValue(0, 0, new());
                }
                for (int nextIndex = index - 1; nextIndex > indexTurnTo; nextIndex--)
                {
                    MaxValue maxValueNext = maxValueTable[Bag.left_capacity, nextIndex];
                    if (maxValueNext == null || maxValueNext.total_value < maxValueNow.total_value)
                    {
                        maxValueTable[Bag.left_capacity, nextIndex] = maxValueNow;
                    }
                    else
                    {
                        maxValueNow = maxValueNext;
                    }
                }
                operatorStack.Add(new LayBackAsTryOtherOperator(Bag, indexTurnTo));
            }
            else
            {
                operatorStack.Add(new LayBackAsTryOtherOperator(Bag, itemIndexStack.Pop()));
            }
/*            if (itemIndexStack.Count == 0)
            {
                // 放入结尾步骤
                operatorStack.Add(new AfterEndOperator(Bag));
            }
            else
            {
                int index = itemIndexStack.Pop();
                operatorStack.Add(new LayBackAsTryOtherOperator(Bag, index));
            }*/
        }

        public static void tryFillItemInBox(Bag_Problem Bag, int index, bool searchFromMaxValueTable = false)
        {
            // 跳过超额物品的情况下
            if (skipItemOverCapacityStatus)
            {
                // 先检查是否能将后续物品一次性放入
                if (disposableFillWhileCapacityEnoughStatus && Bag.left_item_weight <= Bag.left_capacity && Bag.left_item_num > 1)
                {
                    operatorStack.Add(new TakeLeftItemAsCapacityEnoughOperator(Bag, index));
                }

                // 随后从二维表中尝试寻找先前得到的结果
                else if (useTableToReduceStepStatus && searchFromMaxValueTable && maxValueTable[Bag.left_capacity, index] != null)
                {
                    operatorStack.Add(new TakeItemFromTableOperator(Bag, maxValueTable[Bag.left_capacity, index], index - 1));
                }

                // 最后直接放入物品，根据放入后背包容量情况分类
                else
                {
                    Item nextItem = Bag.getItemByIndex(index);
                    if (!keepTryAfterFullFillStatus && nextItem.weight == Bag.left_capacity)
                    {
                        operatorStack.Add(new TakeAndFullFillOperator(Bag, nextItem, index));
                    }
                    else if (nextItem.weight > Bag.left_capacity)
                    {
                        // 超出背包容量，直接跳过
                        if (index + 1 < Bag.getItemsNum())
                        {
                            // 当存在下一个物品
                            BagOperatorStack.tryFillItemInBox(Bag, index + 1);
                        }
                        else
                        {
                            // 已经是最后一个物品，尝试从堆栈中选择不同的路线
                            BagOperatorStack.jumpToOtherBranch(Bag, index);
                        }
                    }
                    else
                    {
                        operatorStack.Add(new TakeAndFillSuccessOperator(Bag, nextItem, index));
                    }
                }
            }

            // 先检查是否能将后续物品一次性放入
            else if (disposableFillWhileCapacityEnoughStatus && Bag.left_item_weight <= Bag.left_capacity && Bag.left_item_num > 1)
            {
                operatorStack.Add(new TakeLeftItemAsCapacityEnoughOperator(Bag, index));
            }

            // 随后从二维表中尝试寻找先前得到的结果
            else if (useTableToReduceStepStatus && searchFromMaxValueTable && maxValueTable[Bag.left_capacity, index] != null)
            {
                operatorStack.Add(new TakeItemFromTableOperator(Bag, maxValueTable[Bag.left_capacity, index], index - 1));
            }

            // 最后直接放入物品，根据放入后背包容量情况分类
            else
            {
                Item nextItem = Bag.getItemByIndex(index);
                if (!keepTryAfterFullFillStatus && nextItem.weight == Bag.left_capacity)
                {
                    operatorStack.Add(new TakeAndFullFillOperator(Bag, nextItem, index));
                }
                else if (nextItem.weight > Bag.left_capacity)
                {
                    operatorStack.Add(new TakeButFillUnsuccessOperator(Bag, nextItem, index));
                }
                else
                {
                    operatorStack.Add(new TakeAndFillSuccessOperator(Bag, nextItem, index));
                }
            }
        }

        public void nextOperator()
        {
            operatorStack[cur].afterDoOperator();
            cur++;
            operatorStack[cur].doOperator();
            
        }

        public void lastOperator()
        {
            operatorStack[cur].undoOperator();
            cur--;
            operatorStack[cur].backToTheOperator();
        }
    }

    internal class MaxValue
    {
        public int total_value;
        public int total_weight;
        public List<Item> itemList;
        BoxOfItemBlock? boxOfRecordMaxValueItemBlock;

        public MaxValue(int total_value, int total_weight, List<Item> items)
        {
            this.total_value = total_value;
            this.total_weight = total_weight;
            itemList = items;
        }

        public void showBlock(MainWindow window, int left_capacity, int left_item_num)
        {
            window.left_capacity_search_text.Text = left_capacity.ToString();
            window.left_item_num_search_text.Text = left_item_num.ToString();
            boxOfRecordMaxValueItemBlock = new(window.max_value_record_canvas, left_capacity, "BlockOfRecord");
            boxOfRecordMaxValueItemBlock.setAllItem(itemList, window);
            foreach (ItemFilledInBox item in boxOfRecordMaxValueItemBlock.items)
            {
                item.ClearName();
            }
        }

        public void searchBlock(MainWindow window, int left_capacity, int left_item_num)
        {
            boxOfRecordMaxValueItemBlock = new(window.max_value_record_canvas, left_capacity, "BlockOfRecord");
            boxOfRecordMaxValueItemBlock.setAllItem(itemList, window);
            foreach (ItemFilledInBox item in boxOfRecordMaxValueItemBlock.items)
            {
                item.ClearName();
            }
        }

        public void hideBlock(MainWindow window)
        {
            window.left_capacity_search_text.Text = "";
            window.left_item_num_search_text.Text = "";
            boxOfRecordMaxValueItemBlock = null;
            window.max_value_record_canvas.Children.Clear();
        }

        public static void showNotFound(MainWindow window)
        {
            Canvas container = window.max_value_record_canvas;

            double totalWidth = container.ActualWidth;
            double totalHeight = container.ActualHeight;

            double Left = 20;
            double Top = totalHeight / 2 - 20;

            container.Children.Clear();

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "目前没有结果!";
            textBlock.FontSize = 13;
            textBlock.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(textBlock, Left);
            Canvas.SetTop(textBlock, Top);

            container.Children.Add(textBlock);
        }
    }
    
}
