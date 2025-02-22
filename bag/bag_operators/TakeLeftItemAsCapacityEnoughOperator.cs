using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class TakeLeftItemAsCapacityEnoughOperator : BagOperator
    {
        public TakeLeftItemAsCapacityEnoughOperator(Bag_Problem Bag, int index) : base(Bag, index) { }
        public TakeLeftItemAsCapacityEnoughOperator(Bag_Problem Bag, Item item, int index) : base(Bag, item, index) { }

        public override void doOperator()
        {
            stepExplain = "背包剩余容量为" + Bag.left_capacity + "，已经能够将所有剩余物品装下，故直接将剩余物品都放入。\n\n";

            Bag.fillLeftItemInBox(index);
            Bag.precent_value += Bag.left_item_value;
            if (Bag.precent_value > Bag.max_value)
            {
                stepExplain += "将剩余物品放入背包后，背包中的物品总价值" + Bag.precent_value + "，已经超过原先记录的最高价值" + Bag.max_value + "，于是重新记录最高价值。";

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
                stepExplain += "将剩余物品放入背包后，背包中的物品总价值" + Bag.precent_value + "，小于等于原先记录的最高价值" + Bag.max_value + "，故不作更改。";
            }

            Bag.left_capacity -= Bag.left_item_weight;
            Bag.left_item_num = 0;
            Bag.left_item_value = 0;
            Bag.left_item_weight = 0;


            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setInUseStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator()
        {
            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 下一步骤：退回全部
                BagOperatorStack.operatorStack.Add(new LayBackAfterFillLeftItemOperator(Bag, index));
            }
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                for (int i = index; i < Bag.getItemsNum(); i++)
                {
                    Bag.getItemByIndex(i).setUnuseStatus();
                }
            }

            Bag.takeOutLeftItemInBox(index);
            Bag.setLeftItemInfo(index - 1);
            Bag.precent_value -= Bag.left_item_value;
            Bag.left_capacity += Bag.left_item_weight;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                Bag.window.printOperatorExplain(stepExplain);
            }
        }
    }
}
