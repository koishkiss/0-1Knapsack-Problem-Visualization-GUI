using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class TakeButFillUnsuccessOperator : BagOperator
    {
        public TakeButFillUnsuccessOperator(Bag_Problem Bag, int index) : base(Bag, index)
        {
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            stepExplain = "尝试将" + item.getName() + "放入背包。背包剩余容量" + Bag.left_capacity + "，物品重量为" + item.weight + "，物品无法放入背包。";
        }

        public TakeButFillUnsuccessOperator(Bag_Problem Bag, Item item, int index) : base(Bag, item, index)
        {
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            stepExplain = "尝试将" + item.getName() + "放入背包。背包剩余容量" + Bag.left_capacity + "，物品重量为" + item.weight + "，物品无法放入背包。";
        }

        public override void doOperator()
        {
            Bag.fillItemInBox(item);
            Bag.left_capacity -= item.weight;

            if (BagOperatorStack.showAnimation)
            {
                item.setOverCapacityStatus();
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator() 
        {
            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                BagOperatorStack.operatorStack.Add(new LayBackAsOverCapacityOperator(Bag, index));
            }
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setUnuseStatus();
            }
            Bag.takeOutItemInBox();

            Bag.left_capacity += item.weight;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                if (Bag.max_value != max_value)
                {
                    Bag.resetMaxValueItemList(max_value_item_list);
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
            Bag.max_value = max_value;
            Bag.setLeftItemInfo(index - 1);
        }
    }
}
