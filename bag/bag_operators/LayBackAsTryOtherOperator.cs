using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class LayBackAsTryOtherOperator : BagOperator
    {
        public LayBackAsTryOtherOperator(Bag_Problem Bag, int index) : base(Bag, index)
        {
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            stepExplain = "目前已经得到当剩余背包容量为" + (Bag.left_capacity + item.weight) + "的情况下选取" + item.getName() + "时背包能够装载的最高价值" + max_value + "，现在尝试在这一情况下不选取该物品是否能达到更高的价值。";
        }

        public override void doOperator()
        {
            Bag.takeOutItemInBox();
            Bag.precent_value -= item.value;
            Bag.left_capacity += item.weight;
            Bag.setLeftItemInfo(index);

            if (BagOperatorStack.showAnimation)
            {
                item.setPushBackStatus();
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setUnuseStatus();
            }

            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 更新最大标记
                if (BagOperatorStack.useTableToReduceStepStatus)
                {
                    MaxValue maxValueNow = BagOperatorStack.maxValueTable[Bag.left_capacity - item.weight, index + 1];
                    if (maxValueNow == null)
                    {
                        List<Item> items = new();
                        items.Add(item);
                        BagOperatorStack.maxValueTable[Bag.left_capacity, index] = new(item.value, item.weight, items);
                    }
                    else
                    {
                        List<Item> items = new(maxValueNow.itemList);
                        items.Insert(0, item);
                        BagOperatorStack.maxValueTable[Bag.left_capacity, index] = new(maxValueNow.total_value + item.value, maxValueNow.total_weight + item.weight, items);
                    }
                }

                if (index + 1 < Bag.getItemsNum())
                {
                    // 当存在下一个物品
                    BagOperatorStack.tryFillItemInBox(Bag, index + 1);
                }
                else
                {
                    BagOperatorStack.jumpToOtherBranch(Bag, index);

/*                    // 已经是最后一个物品，尝试从堆栈中选择不同的路线
                    if (BagOperatorStack.itemIndexStack.Count == 0)
                    {
                        // 路线已经选择完毕，放入结尾步骤
                        BagOperatorStack.operatorStack.Add(new AfterEndOperator(Bag));
                    }
                    else if (BagOperatorStack.useTableToReduceStepStatus)
                    {
                        //在选择不同路径前需要更改MaxValueTable
                        int indexTurnTo = BagOperatorStack.itemIndexStack.Pop();
                        MaxValue maxValueNow = BagOperatorStack.maxValueTable[Bag.left_capacity, index];
                        for (int nextIndex = index - 1; nextIndex > indexTurnTo; nextIndex--)
                        {
                            MaxValue maxValueNext = BagOperatorStack.maxValueTable[Bag.left_capacity, nextIndex];
                            if (maxValueNext == null || maxValueNext.total_value < maxValueNow.total_value)
                            {
                                BagOperatorStack.maxValueTable[Bag.left_capacity, nextIndex] = maxValueNow;
                            } 
                            else
                            {
                                maxValueNow = maxValueNext;
                            }
                        }
                        BagOperatorStack.operatorStack.Add(new LayBackAsTryOtherOperator(Bag, indexTurnTo));
                    }
                    else
                    {
                        BagOperatorStack.operatorStack.Add(new LayBackAsTryOtherOperator(Bag, BagOperatorStack.itemIndexStack.Pop()));
                    }*/
                }
            }
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setInUseStatus();
            }
            Bag.fillItemInBox(item);

            Bag.precent_value += item.value;
            Bag.left_capacity -= item.weight;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setPushBackStatus();
                if (Bag.max_value != max_value)
                {
                    Bag.resetMaxValueItemList(max_value_item_list);
                    Bag.max_value = max_value;
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
            Bag.setLeftItemInfo(index);
        }
    }
}
