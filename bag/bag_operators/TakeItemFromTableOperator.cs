using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class TakeItemFromTableOperator : BagOperator
    {
        private MaxValue maxValueItemList;

        public TakeItemFromTableOperator(Bag_Problem Bag, MaxValue maxValue, int index) : base(Bag, index)
        {
            this.maxValueItemList = maxValue;
        }

        public override void doOperator()
        {
            stepExplain = "从之前记录的中发现 [剩余背包容量 , 剩余物品个数]=[" + Bag.left_capacity + " , " + (Bag.getItemsNum()-index-1) +  "] 时已经存在最优解，故直接使用最优解，将最优解情况下的物品放入背包。\n\n";

            Bag.fillAllItemInBox(maxValueItemList.itemList);
            Bag.precent_value += maxValueItemList.total_value;
            // 最大值更新
            if (Bag.precent_value > Bag.max_value)
            {
                stepExplain += "将这些物品放入背包后，背包中的物品总价值" + Bag.precent_value + "，已经超过原先记录的最高价值" + Bag.max_value + "，于是重新记录最高价值。";

                max_value = Bag.precent_value;
                Bag.max_value = max_value;

                // 更新最大价值时背包状态
                if (max_value_item_list.Count == 0)
                {
                    List<Item> max_value_item_list = new List<Item>();
                    foreach (ItemFilledInBox item in Bag.boxOfItemBlock.items)
                    {
                        max_value_item_list.Add(item.item);
                    }
                    max_value_item_list.Reverse();
                    this.max_value_item_list = max_value_item_list;
                    BagOperatorStack.precent_max_value_item_list = max_value_item_list;
                }

                // 将最大值时的背包状态显示在界面
                if (BagOperatorStack.showAnimation)
                {
                    Bag.resetMaxValueItemList(max_value_item_list);
                }
            }
            else
            {
                max_value_item_list = BagOperatorStack.precent_max_value_item_list;
                stepExplain += "将这些物品放入背包后，背包中的物品总价值" + Bag.precent_value + "，小于等于原先记录的最高价值" + Bag.max_value + "，故不作更改。";
            }

            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setInUseStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
                // 在下方设置canvas展示
                maxValueItemList.showBlock(Bag.window, Bag.left_capacity, Bag.getItemsNum() - index - 1);
            }

            Bag.left_capacity -= maxValueItemList.total_weight;
            Bag.left_item_num = 0;
            Bag.left_item_value = 0;
            Bag.left_item_weight = 0;
        }

        public override void afterDoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                maxValueItemList.hideBlock(Bag.window);
            }
            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 下一步骤：退回全部
                BagOperatorStack.operatorStack.Add(new LayBackAfterTakeItemFromTableOperator(Bag, maxValueItemList, index));
            }
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setUnuseStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
                maxValueItemList.hideBlock(Bag.window);
            }

            Bag.takeOutNumOfItemInBox(maxValueItemList.itemList.Count);
            Bag.setLeftItemInfo(index);
            Bag.precent_value -= maxValueItemList.total_value;
            Bag.left_capacity += maxValueItemList.total_weight;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setInUseStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
                // 在下方设置canvas展示
                maxValueItemList.showBlock(Bag.window, Bag.left_capacity + maxValueItemList.total_weight, Bag.getItemsNum() - index - 1);
            }
        }
    }
}
