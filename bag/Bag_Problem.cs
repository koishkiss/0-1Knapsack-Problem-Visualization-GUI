using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WpfApp1.bag.bag_operators;

namespace WpfApp1.bag
{
    internal class Bag_Problem
    {
        public int capacity;
        public int max_value;
        public int precent_value;
        public int left_capacity;
        public int left_item_num;
        public int left_item_value;
        public int left_item_weight;
        Item_List item_list;
        BagOperatorStack bagOperatorStack;

        public BoxOfItemBlock? boxOfItemBlock;
        public BoxOfItemBlock? boxOfItemWithMaxValue;
        public MainWindow window;

        public Bag_Problem(int capacity, MainWindow window)
        {
            this.capacity = capacity;

            max_value = 0;
            precent_value = 0;
            left_capacity = capacity;

            item_list = new Item_List();

            left_item_num = 0;
            left_item_value = 0;
            left_item_weight = 0;

            this.window = window;
            boxOfItemBlock = null;
            boxOfItemWithMaxValue = null;
        }

        public Bag_Problem(int capacity, int initial_items_num, MainWindow window)
        {
            this.capacity = capacity;

            max_value = 0;
            precent_value = 0;
            left_capacity = capacity;

            item_list = new Item_List(capacity, initial_items_num);

            left_item_num = initial_items_num;
            left_item_value = item_list.total_value;
            left_item_weight = item_list.total_weight;

            this.window = window;
            boxOfItemBlock = null;
            boxOfItemWithMaxValue = null;
        }

        public List<Item> getItemList()
        {
            List<Item> list = new List<Item>(getItemsNum());
            for (Item? item = item_list.getFirstItem(); item != null; item = item_list.getNextItem())
            {
                list.Add(item);
            }
            return list;
        }

        public void resetItemListSort(List<Item> list)
        {
            item_list.resetItemSort(list);
        }

        public int getItemsNum()
        {
            return item_list.Count;
        }

        public void addItem(int weight, int value)
        {
            if (item_list.Count >= 20)
            {
                throw new RowException("物品数量不宜大于二十！");
            }

            else if (weight < 0)
            {
                throw new RowException("物品重量不得为负数！");
            }

            else if (weight > capacity)
            {
                throw new RowException("该物品重量超出背包上限！");
            }

            else if (value < 0)
            {
                throw new RowException("物品价值不得为负数！");
            }
            left_item_num += 1;
            left_item_value += value;
            left_item_weight += weight;
            item_list.addItem(weight, value);
        }

        public void deleteItem(int ID)
        {
            Item item = item_list.deleteItem(ID);
            left_item_num -= 1;
            left_item_value -= item.value;
            left_item_weight -= item.weight;
        }

        public Item getItemByIndex(int index)
        {
            return item_list.getItemByIndex(index);
        }

        public void suffleItem()
        {
            item_list.shuffleItem();
        }

        public void clearItem()
        {
            item_list.clearItem();
            left_item_num = 0;
            left_item_value = 0;
            left_item_weight = 0;
        }

        public void renumberItem()
        {
            item_list = new Item_List(item_list);
            boxOfItemBlock = null;
            boxOfItemWithMaxValue = null;
        }

        public void setCapacity(int newCapacity)
        {
            if (newCapacity <= 0)
            {
                throw new RowException("背包容量应大于等于一！");
            } 
            else if (newCapacity > 25)
            {
                throw new RowException("背包容量不宜大于二十五！");
            }
            foreach (Item item in item_list.Values) 
            { 
                if (item.weight > newCapacity)
                {
                    throw new RowException("列表中存在重量比该容量大的物品！");
                }
            }
            this.capacity = newCapacity;
            left_capacity = newCapacity;
        }

        public void createBoxOfItemBlock(Canvas container)
        {
            container.Children.Clear();
            boxOfItemBlock = new(container, capacity, "block");
        }

        public void createBoxOfItemWithMaxValue(Canvas container)
        {
            container.Children.Clear();
            boxOfItemWithMaxValue = new(container, capacity, "blockRecorder");
        }

        public void fillItemInBox(Item item)
        {
            if (boxOfItemBlock != null)
            {
                boxOfItemBlock.fillItem(item, window);
            }
        }

        public List<Item> getItemListAfterIndex(int index)
        {
            List<Item> list = new List<Item>();
            for (; index < item_list.Count; index++)
            {
                list.Add(item_list.getItemByIndex(index));
            }
            return list;
        }

        public void fillLeftItemInBox(int index)
        {
            if (boxOfItemBlock == null) return;
            boxOfItemBlock.fillAllItem(getItemListAfterIndex(index), window);
        }

        public void fillAllItemInBox(List<Item> list)
        {
            if (boxOfItemBlock != null)
            {
                boxOfItemBlock.fillAllItem(list, window);
            }
        }

        public void takeOutItemInBox()
        {
            if (boxOfItemBlock != null)
            {
                boxOfItemBlock.unFillItem();
            }
        }

        public void takeOutLeftItemInBox(int index)
        {
            if (boxOfItemBlock == null) return;
            boxOfItemBlock.unFillAllItem(item_list.Count - index);
        }

        public void takeOutNumOfItemInBox(int num)
        {
            if (boxOfItemBlock != null)
            {
                boxOfItemBlock.unFillAllItem(num);
            }
        }

        public void clearItemStatusInBox()
        {
            max_value = 0;
            precent_value = 0;
            left_capacity = capacity;

            left_item_num = 0;
            left_item_value = 0;
            left_item_weight = 0;
            foreach (Item item in item_list.Values)
            {
                left_item_num++;
                left_item_value += item.value;
                left_item_weight += item.weight;
                item.setUnuseStatus();
            }
            if (boxOfItemBlock != null)
            {
                foreach (ItemFilledInBox item in boxOfItemBlock.items)
                {
                    item.ClearName();
                }
            }
            if (boxOfItemWithMaxValue != null)
            {
                foreach (ItemFilledInBox item in boxOfItemWithMaxValue.items)
                {
                    item.ClearName();
                }
            }
        }

        public void clearOperatorStack()
        {
            bagOperatorStack = new BagOperatorStack(this);
        }

        public void setLeftItemInfo(int index)
        {
            left_item_num = item_list.Count - index - 1;
            left_item_value = item_list.leftValueAfterIndex[index + 1];
            left_item_weight = item_list.leftWeightAfterIndex[index + 1];
        }

        public int getLeftItemNumAfterIndex(int index)
        {
            return item_list.Count - index - 1;
        }

        public int getLeftItemValueAfterIndex(int index)
        {
            return item_list.leftValueAfterIndex[index + 1];
        }

        public int getLeftItemWeightAfterIndex(int index)
        {
            return item_list.leftWeightAfterIndex[index + 1];
        }

        public void toNextStep()
        {
            bagOperatorStack.nextOperator();
        }

        public void toLastStep()
        {
            bagOperatorStack.lastOperator();
        }

        public void resetMaxValueItemList(List<Item> items)
        {
            if (boxOfItemWithMaxValue != null)
            {
                boxOfItemWithMaxValue.resetAllItem(items, window);
            }
        }
    }
}
