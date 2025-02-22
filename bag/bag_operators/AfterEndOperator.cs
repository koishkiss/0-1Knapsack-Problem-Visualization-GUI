using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class AfterEndOperator : BagOperator
    {
        public AfterEndOperator(Bag_Problem Bag) : base(Bag, 0) 
        {
            max_value = Bag.max_value;
            max_value_item_list = BagOperatorStack.precent_max_value_item_list;
            stepExplain = "所有步骤执行完毕，得到背包可装载的最高价值为" + max_value + "，共展示了" + BagOperatorStack.operatorStack.Count + "步操作，动画结束。";
        }

        public override void doOperator()
        {
            Bag.setLeftItemInfo(-1);
            Bag.resetMaxValueItemList(max_value_item_list);
            Bag.window.printOperatorExplain(stepExplain);
            Bag.window.setAnimationStatusHasEnd();
            Bag.boxOfItemWithMaxValue.setAllItemInUse();
        }

        public override void afterDoOperator()
        {
            throw new StepEndException();
        }

        public override void undoOperator() 
        {
            Bag.boxOfItemWithMaxValue.setAllItemUnuse();
        }

        public override void backToTheOperator()
        {
            if (Bag.max_value != max_value)
            {
                Bag.resetMaxValueItemList(max_value_item_list);
                Bag.max_value = max_value;
            }
            Bag.setLeftItemInfo(-1);
            Bag.boxOfItemWithMaxValue.setAllItemInUse();
            Bag.window.printOperatorExplain(stepExplain);
        }
    }

    internal class StepEndException : RowException 
    {
        public StepEndException() : base("已经是最后一步了!") {}
    }
}
