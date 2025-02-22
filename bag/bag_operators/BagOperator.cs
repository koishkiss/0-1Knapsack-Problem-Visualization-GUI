using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag.bag_operators
{
    internal abstract class BagOperator
    {
        protected Bag_Problem Bag;
        protected Item item;
        protected int index;
        protected int max_value;
        protected List<Item> max_value_item_list = new();
        protected string stepExplain = "";

        protected BagOperator(Bag_Problem Bag, int index)
        {
            this.Bag = Bag;
            this.item = Bag.getItemByIndex(index);
            this.index = index;
            max_value = Bag.max_value;
        }

        protected BagOperator(Bag_Problem Bag, Item item, int index)
        {
            this.Bag = Bag;
            this.item = item;
            this.index = index;
            max_value = Bag.max_value;
        }

        public abstract void doOperator();

        public abstract void afterDoOperator();

        public abstract void undoOperator();

        public abstract void backToTheOperator();

    }
}
