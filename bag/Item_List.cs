using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.bag
{
    internal class Item_List : Dictionary<int, Item>
    {
        private ID_Generator ID_gen = new ID_Generator();
        private Item_List_Iterator iterator;
        public int total_weight = 0;
        public int total_value = 0;

        public List<int> leftValueAfterIndex;
        public List<int> leftWeightAfterIndex;

        public Item_List() 
        {
            iterator = new Item_List_Iterator();
            leftValueAfterIndex = new List<int>();
            leftWeightAfterIndex = new List<int>();
            resetLeftItemInfoList();
        }

        public Item_List(int capacity, int initial_items_num)
        {
            for (int i = 1; i <= initial_items_num; i++)
            {
                int ID = ID_gen.next();
                Item item = new Item(ID, capacity);
                this.Add(ID, item);
                total_weight += item.weight;
                total_value += item.value;
            }
            iterator = new Item_List_Iterator(this);
            leftValueAfterIndex = new List<int>();
            leftWeightAfterIndex = new List<int>();
            resetLeftItemInfoList();
        }

        public Item_List(Item_List oldItemList)
        {
            for (Item? item = oldItemList.getFirstItem(); item != null; item = oldItemList.getNextItem())
            {
                int ID = ID_gen.next();
                this.Add(ID, new Item(ID, item));
                total_weight += item.weight;
                total_value += item.value;
            }
            iterator = new Item_List_Iterator(this);
            leftValueAfterIndex = new List<int>();
            leftWeightAfterIndex = new List<int>();
            resetLeftItemInfoList();
        }

        /*        public void addRandomItem(int total_weight)
                {
                    int ID = ID_gen.next();
                    this.Add(ID, new Item(ID, total_weight));
                    iterator.append(ID);
                }*/

        public void addItem(int weight, int value)
        {
            int ID = ID_gen.next();
            Item item = new Item(ID, weight, value);
            this.Add(ID, item);
            total_weight += item.weight;
            total_value += item.value;
            iterator.append(ID);
            resetLeftItemInfoList();
        }

        public Item deleteItem(int ID)
        {
            if (this.Count == 0)
            {
                throw new RowException("物品列表已为空！");
            }
            if (this.ContainsKey(ID))
            {
                Item item = this[ID];
                this.Remove(ID);
                total_weight -= item.weight;
                total_value -= item.value;
                iterator.delete(ID);
                resetLeftItemInfoList();
                return item;
            }
            else
            {
                throw new RowException("无法找到该物品ID！");
            }
        }

        public void shuffleItem()
        {
            iterator = new Item_List_Iterator(this, true);
            resetLeftItemInfoList();
        }

        public void clearItem()
        {
            this.Clear();
            total_weight = 0;
            total_value = 0;
            ID_gen.reset();
            iterator = new Item_List_Iterator();
            resetLeftItemInfoList();
        }

        public void resetIterator()
        {
            iterator.reset();
        }

        public Item? getFirstItem()
        {
            resetIterator();
            int ID = iterator.next();
            if (ID == -1)
            {
                return null;
            }
            return this[ID];
        }

        public Item? getNextItem()
        {
            int ID = iterator.next();
            if (ID == -1)
            {
                return null;
            }
            return this[ID];
        }

        public Item getItemByIndex(int index)
        {
            return this[iterator.at(index)];
        }

        public void resetSort(List<Item> items)
        {

        }

        public void resetItemSort(List<Item> items)
        {
            iterator = new Item_List_Iterator(items);
            leftValueAfterIndex = new List<int>();
            leftWeightAfterIndex = new List<int>();
            resetLeftItemInfoList();
        }

        public void resetLeftItemInfoList()
        {
            leftValueAfterIndex.Clear();
            leftWeightAfterIndex.Clear();
            leftValueAfterIndex.Add(0);
            leftWeightAfterIndex.Add(0);
            for (int i = Count - 1; i >= 0; i--)
            {
                Item item = getItemByIndex(i);
                leftValueAfterIndex.Insert(0, leftValueAfterIndex[0] + item.value);
                leftWeightAfterIndex.Insert(0, leftWeightAfterIndex[0] + item.weight);
            }
        }
    }




    internal class Item_List_Iterator
    {
        private List<int> item_ID_list;
        private int cur;

        public Item_List_Iterator()
        {
            item_ID_list = new List<int>();
            cur = -1;
        }

        public Item_List_Iterator(Item_List item_list, bool suffle = false)
        {
            int[] ID_list = new int[item_list.Count];

            if (suffle)
            {
                foreach (int ID in item_list.Keys)
                {
                    int randomIndex = Random_Generator.random.Next(item_list.Count);
                    while (ID_list[randomIndex] != 0)
                    {
                        randomIndex = (randomIndex + 1) % item_list.Count;
                    }
                    ID_list[randomIndex] = ID;
                }
            }
            else 
            {
                int i = 0;
                foreach (int ID in item_list.Keys)
                {
                    ID_list[i] = ID;
                    i++;
                }
            }

            item_ID_list = new List<int>(ID_list);
            cur = -1;
        }

        public Item_List_Iterator(List<Item> items)
        {
            item_ID_list = new List<int>();
            foreach (Item item in items)
            {
                item_ID_list.Add(item.ID);
            }
            cur = -1;
        }

        public void append(int ID)
        {
            item_ID_list.Add(ID);
        }

        public void delete(int ID)
        {
            item_ID_list.Remove(ID);
        }

        public int next()
        {
            if (cur >= item_ID_list.Count - 1)
            {
                return -1;
            }
            cur++;
            return item_ID_list[cur];
        }

        public void reset()
        {
            cur = -1;
        }

        public int at(int index)
        {
            return item_ID_list[index];
        }
    }
}
