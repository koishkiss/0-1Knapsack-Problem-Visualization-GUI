using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal class BeforeStartOperator : BagOperator
    {
        public BeforeStartOperator(Bag_Problem Bag) : base(Bag, 0) 
        {
            max_value = 0;
        }

        public override void doOperator() {}

        public override void afterDoOperator() 
        {
            if (BagOperatorStack.judgeIfNextStepNotGenerate())
            {
                BagOperatorStack.tryFillItemInBox(Bag, 0);
            }
        }

        public override void undoOperator()
        {
            throw new StepHeadException();
        }

        public override void backToTheOperator()
        {
            if (Bag.max_value != max_value)
            {
                Bag.resetMaxValueItemList(max_value_item_list);
                Bag.max_value = max_value;
            }
            Bag.setLeftItemInfo(-1);
            Bag.window.printOperatorExplain(stepExplain);
        }
    }

    internal class StepHeadException : RowException
    {
        public StepHeadException() : base("这是第一步，不可退回!")
        {
        }
    }
}
