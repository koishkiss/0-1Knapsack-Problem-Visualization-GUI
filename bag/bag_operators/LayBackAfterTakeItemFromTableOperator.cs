using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class LayBackAfterTakeItemFromTableOperator : BagOperator
    {
        private MaxValue maxValueItemList;

        public LayBackAfterTakeItemFromTableOperator(Bag_Problem Bag, MaxValue maxValue, int index) : base(Bag, index)
        {
            stepExplain = "将刚刚放入的物品取出，随后执行后面的操作。";
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            this.maxValueItemList = maxValue;
        }

        public override void doOperator()
        {
            Bag.takeOutNumOfItemInBox(maxValueItemList.itemList.Count);
            Bag.left_capacity += maxValueItemList.total_weight;
            Bag.precent_value -= maxValueItemList.total_value;
            Bag.left_item_num = 0;
            Bag.left_item_value = 0;
            Bag.left_item_weight = 0;

            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setPushBackStatus();
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
        }

        public override void afterDoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setUnuseStatus();
                }
            }

            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                // 下一步：选择其它分支
                BagOperatorStack.jumpToOtherBranch(Bag, index);
            }
        }

        public override void undoOperator()
        {
            Bag.fillAllItemInBox(maxValueItemList.itemList);
            Bag.left_capacity -= maxValueItemList.total_weight;
            Bag.precent_value += maxValueItemList.total_value;
        }

        public override void backToTheOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                foreach (Item item in maxValueItemList.itemList)
                {
                    item.setPushBackStatus();
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
