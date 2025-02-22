using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class LayBackAsOverCapacityOperator : BagOperator
    {
        public LayBackAsOverCapacityOperator(Bag_Problem Bag, int index) : base(Bag, index)
        {
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            stepExplain = "将超出背包剩余容量的" + item.getName() + "取出，随后执行后面的操作。";
        }

        public override void doOperator()
        {

            Bag.takeOutItemInBox();
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
                    BagOperatorStack.maxValueTable[Bag.left_capacity, index] = new(0, 0, new());
                }

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
        }

        public override void undoOperator()
        {
            if (BagOperatorStack.showAnimation)
            {
                item.setOverCapacityStatus();
            }
            Bag.fillItemInBox(item);

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
                }
                Bag.window.printOperatorExplain(stepExplain);
            }
            Bag.max_value = max_value;
            Bag.setLeftItemInfo(index);
        }
    }
}
