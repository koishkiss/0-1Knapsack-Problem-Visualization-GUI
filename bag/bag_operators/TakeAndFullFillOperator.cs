using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class TakeAndFullFillOperator : BagOperator
    {
        public TakeAndFullFillOperator(Bag_Problem Bag, int index) : base(Bag, index) { }
        public TakeAndFullFillOperator(Bag_Problem Bag, Item item, int index) : base(Bag, item, index) { }

        public override void doOperator()
        {
            stepExplain = "尝试将" + item.getName() + "放入背包。背包剩余容量" + Bag.left_capacity + "，物品重量为" + item.weight + "，该物品已经将背包填满。\n\n";

            Bag.fillItemInBox(item);
            Bag.precent_value += item.value;
            if (Bag.precent_value > Bag.max_value)
            {
                stepExplain += item.getName() + "放入背包后，背包中的物品总价值" + Bag.precent_value + "，已经超过原先记录的最高价值" + Bag.max_value + "，于是重新记录最高价值。";

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
                stepExplain += item.getName() + "放入背包后，背包中的物品总价值" + Bag.precent_value + "，小于等于原先记录的最高价值" + Bag.max_value + "，故不作更改。";
            }

            Bag.left_capacity -= item.weight;
            Bag.setLeftItemInfo(index);

            if (BagOperatorStack.showAnimation)
            {
                item.setFullFillStatus();
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator()
        {
/*            if (BagOperatorStack.showAnimation)
            {
                item.setInUseStatus();
            }*/

            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 直接退回
                BagOperatorStack.operatorStack.Add(new LayBackAsTryOtherOperator(Bag, index));
            }
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setUnuseStatus();
            }
            Bag.takeOutItemInBox();

            Bag.precent_value -= item.value;
            Bag.left_capacity += item.weight;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setInUseStatus();
                if (Bag.max_value != max_value)
                {
                    Bag.resetMaxValueItemList(max_value_item_list);
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
            Bag.max_value = max_value;
            Bag.setLeftItemInfo(index);
        }
    }
}
