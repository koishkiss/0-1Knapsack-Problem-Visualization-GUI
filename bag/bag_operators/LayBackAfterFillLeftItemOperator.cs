using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class LayBackAfterFillLeftItemOperator : BagOperator
    {
        public LayBackAfterFillLeftItemOperator(Bag_Problem Bag, int index) : base(Bag, index) 
        {
            stepExplain = "将刚刚放入的所有剩余物品取出，随后执行后面的操作。";
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
        }
        public LayBackAfterFillLeftItemOperator(Bag_Problem Bag, Item item, int index) : base(Bag, item, index)
        {
            stepExplain = "将刚刚放入的所有剩余物品取出，随后执行后面的操作。";
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
        }

        public override void doOperator()
        {

            Bag.takeOutLeftItemInBox(index);
            Bag.left_capacity += Bag.getLeftItemWeightAfterIndex(index - 1);
            Bag.precent_value -= Bag.getLeftItemValueAfterIndex(index - 1);


            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setPushBackStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setUnuseStatus();
                }
            }

            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 更新最大标记
                if (BagOperatorStack.useTableToReduceStepStatus)
                {
                    BagOperatorStack.maxValueTable[Bag.left_capacity, index] = new(Bag.getLeftItemValueAfterIndex(index - 1), Bag.getLeftItemWeightAfterIndex(index - 1), Bag.getItemListAfterIndex(index));
                }

                BagOperatorStack.jumpToOtherBranch(Bag, index);
/*                // 尝试从堆栈中选择不同的路线
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

        public override void undoOperator()
        {
            Bag.fillLeftItemInBox(index);
            Bag.setLeftItemInfo(index - 1);
            Bag.left_capacity -= Bag.left_item_weight;
            Bag.precent_value += Bag.left_item_value;
            Bag.left_item_num = 0;
            Bag.left_item_value = 0;
            Bag.left_item_weight = 0;

            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setInUseStatus();
                }
            }
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setPushBackStatus();
                }
                if (Bag.max_value != max_value)
                {
                    Bag.resetMaxValueItemList(max_value_item_list);
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
            Bag.max_value = max_value;
            Bag.left_item_num = 0;
            Bag.left_item_value = 0;
            Bag.left_item_weight = 0;
        }

    }
}
