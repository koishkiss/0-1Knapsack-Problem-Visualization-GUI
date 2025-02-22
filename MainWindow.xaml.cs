using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.bag;
using WpfApp1.bag.bag_operators;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        bag.Bag_Problem Bag;
        private bool hasStart = false;
        private bool hasStop = true;
        private bool hasEnd = false;

        public MainWindow()
        {
            Bag = new bag.Bag_Problem(10, 5, this);
            InitializeComponent();
/*            Bag.clearItem();
            Bag.addItem(7, 8);
            Bag.addItem(4, 1);
            Bag.addItem(7, 1);
            Bag.addItem(4, 7);
            Bag.addItem(3, 2);
            Bag.addItem(4, 2);
            Bag.addItem(4, 5);
            Bag.addItem(6, 1);
            Bag.addItem(1, 5);
            Bag.addItem(6, 2);*/
            item_num_text.Text = Bag.getItemsNum().ToString();
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
            this.SizeChanged += OnWindowSizeChanged;
            tryUpdateAllStepGeneratorOption();
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!hasStop)
            {
                // 暂停播放
                dispatcherTimer.Stop();
            }
            reloadItems();
            reloadBagBox();
            if (!hasStop)
            {
                // 暂停播放
                dispatcherTimer.Start();
            }
        }

        private void exclute(Func<int> func)
        {
            //func();
            try
            {
                func();
            }
            catch (bag.RowException ex)
            {
                MessageBox.Show(ex.msg);
            }
            catch
            {
                MessageBox.Show("请正确填写参数!");
            }
        }

        // 刷新上边的物品栏
        private void reloadItems()
        {
            List<Item> items = Bag.getItemList();
            item_canvas.Children.Clear();

            double totalWidth = item_canvas.ActualWidth;
            double totalHeight = item_canvas.ActualHeight;

            double nowLeft = 0;
            double nowTop = 0;

            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (nowTop + 88 >= totalHeight )
                {
                    MessageBox.Show("警告：物品数量已经超出屏幕！");
                    clearAnimaionOperatorButtonStatus();
                    for (; i < items.Count; i++)
                    {
                        Bag.deleteItem(items[i].ID);
                    }
                    reloadValue();
                    break;
                }

                Canvas.SetLeft(item.border, nowLeft);
                Canvas.SetTop(item.border, nowTop);
                item_canvas.Children.Add(item.border);

                nowLeft += 92;
                if (nowLeft + 88 >=  totalWidth)
                {
                    nowLeft = 0;
                    nowTop += 92;
                }
            }
        }

        // 刷新动画界面的Canvas
        private void reloadBagBox()
        {
            if (Bag.boxOfItemBlock != null)
            {
                Stack<ItemFilledInBox> items = Bag.boxOfItemBlock.items;
                Bag.createBoxOfItemBlock(bag_canvas);

                foreach (var item in items)
                {
                    item.ClearName();
                    Bag.boxOfItemBlock.setItem(item.item, this);
                }
            }
            else
            {
                Bag.createBoxOfItemBlock(bag_canvas);
            }

            if (Bag.boxOfItemWithMaxValue != null)
            {
                Stack<ItemFilledInBox> items = Bag.boxOfItemWithMaxValue.items;
                Bag.createBoxOfItemWithMaxValue(bag_max_value_canvas);

                foreach (var item in items)
                {
                    item.ClearName();
                    Bag.boxOfItemWithMaxValue.setItem(item.item, this);
                }
            } 
            else
            {
                Bag.createBoxOfItemWithMaxValue(bag_max_value_canvas);
            }
        }

        // 刷新值到页面
        private void reloadValue()
        {
            max_value_text.Text = Bag.max_value.ToString();
            precent_value_text.Text = Bag.precent_value.ToString();
            left_capacity_text.Text = Bag.left_capacity.ToString();
            left_item_num_text.Text = Bag.left_item_num.ToString();
            left_item_value_text.Text = Bag.left_item_value.ToString();
            left_item_weight_text.Text = Bag.left_item_weight.ToString();
        }

        // 还原动画状态
        private void clearAnimaionOperatorButtonStatus()
        {
            if (!hasStop)
            {
                // 暂停播放
                dispatcherTimer.Stop();
                hasStop = true;
            }
            printOperatorExplain("");
            Bag.clearOperatorStack();
            Bag.clearItemStatusInBox();
            Bag.createBoxOfItemBlock(bag_canvas);
            Bag.createBoxOfItemWithMaxValue(bag_max_value_canvas);
            tryUpdateAllStepGeneratorOption();
            left_capacity_search_text.Text = "";
            left_item_num_search_text.Text = "";
            max_value_record_canvas.Children.Clear();
            dispatcherTimer = new DispatcherTimer();
            hasStart = false;
            hasEnd = false;
            start_button.Content = "开始";
        }

        // 设置结束态
        public void setAnimationStatusHasEnd()
        {
            hasEnd = true;
            hasStop = true;
            start_button.Content = "重播";
            dispatcherTimer.Stop();
        }

        private bool askWhenChangeBagAfterStart()
        {
            if (hasStart) 
            { 
                if (hasEnd)
                {
                    //清空动画和堆栈
                    clearAnimaionOperatorButtonStatus();
                    reloadValue();
                    return true;
                }
                MessageBoxResult doChange = MessageBox.Show("动画已经开始播放，该操作将退出当前动画，是否继续？", "", MessageBoxButton.OKCancel);
                if (doChange == MessageBoxResult.OK)
                {
                    //清空动画和堆栈
                    clearAnimaionOperatorButtonStatus();
                    reloadValue();
                    return true;
                }
                return false;
            }
            else return true;
        }

        private void add_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                int weight = int.Parse(weight_text.Text);
                int value = int.Parse(value_text.Text);
                Bag.addItem(weight, value);
                reloadValue();
                capacity_text.Text = Bag.capacity.ToString();
                reloadItems();
                return 0;
            });
            weight_text.Text = string.Empty;
            value_text.Text = string.Empty;
        }

        private void random_set_item(object sender, RoutedEventArgs e)
        {
            weight_text.Text =  Random_Generator.NextSmallerRandom(1, Bag.capacity).ToString();
            value_text.Text = Random_Generator.NextSmallerRandom(1, Bag.capacity).ToString();
        }

        private void delete_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                int ID = int.Parse(ID_text.Text);
                Bag.deleteItem(ID);
                reloadItems();
                return 0;
            });
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
            ID_text.Text = string.Empty;
        }

        private void random_gen(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                int num = int.Parse(item_num_text.Text);
                if (num < 1)
                {
                    throw new bag.RowException("物品数量应大于等于一！");
                } 
                else if (num > 20)
                {
                    throw new bag.RowException("物品数量不宜大于二十！");
                }
                Bag = new bag.Bag_Problem(Bag.capacity, num, this);
                if (sortMethed.Count != 0)
                {
                    sort_item(sender, e);
                    renumber_item(sender, e);
                }
                reloadItems();
                return 0;
            });
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
            reloadBagBox();
        }

        private void renumber_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                Bag.renumberItem();
                reloadItems();
                reloadValue();
                capacity_text.Text = Bag.capacity.ToString();
                reloadBagBox();
                return 0;
            });
        }

        private void suffle_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                Bag.suffleItem();
                return 0;
            });
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
            reloadItems();
        }

        private void clear_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                Bag = new Bag_Problem(Bag.capacity, 0, this);
                reloadItems();
                reloadBagBox();
                return 0;
            });
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
        }

        private void change_capacity(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                int capacity = int.Parse(capacity_text.Text);
                Bag.setCapacity(capacity);
                reloadBagBox();
                return 0;
            });
            reloadValue();
            capacity_text.Text = Bag.capacity.ToString();
        }


        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private void reset_animation(object sender, RoutedEventArgs e)
        {
            // 重置动画
            if (!hasStart)
            {
                return;
            }
            if (!hasStop)
            {
                // 暂停播放
                dispatcherTimer.Stop();
                hasStop = true;
            }
            printOperatorExplain("");
            tryUpdateAllStepGeneratorOption();
            BagOperatorStack.cur = 0;
            Bag.clearItemStatusInBox();
            Bag.createBoxOfItemBlock(bag_canvas);
            Bag.createBoxOfItemWithMaxValue(bag_max_value_canvas);
            left_capacity_search_text.Text = "";
            left_item_num_search_text.Text = "";
            max_value_record_canvas.Children.Clear();
            reloadValue();

            dispatcherTimer = new DispatcherTimer();
            hasStart = false;
            hasEnd = false;
            start_button.Content = "开始";

        }

        private void to_last_step(object sender, RoutedEventArgs e)
        {
            // 跳到上一步
            exclute(() =>
            {
                if (hasStart && !hasStop)
                {
                    // 暂停播放
                    dispatcherTimer.Stop();
                    start_button.Content = "继续";
                    hasStop = true;
                }
                else if (hasEnd)
                {
                    hasEnd = false;
                    start_button.Content = "继续";
                }
                Bag.toLastStep();
                reloadValue();
                return 0;
            });

        }

        private void start(object sender, RoutedEventArgs e)
        {
            exclute(() =>
            {
                if (hasEnd)
                {
                    printOperatorExplain("");
                    tryUpdateAllStepGeneratorOption();
                    BagOperatorStack.cur = 0;
                    Bag.clearItemStatusInBox();
                    Bag.createBoxOfItemBlock(bag_canvas);
                    Bag.createBoxOfItemWithMaxValue(bag_max_value_canvas);
                    left_capacity_search_text.Text = "";
                    left_item_num_search_text.Text = "";
                    max_value_record_canvas.Children.Clear();
                    reloadValue();

                    start_button.Content = "暂停";
                    hasEnd = false;
                    hasStop = false;
                    dispatcherTimer.Start();
                }
                else if (hasStop)
                {
                    start_button.Content = "暂停";
                    hasStop = false;
                    tryUpdateAllStepGeneratorOption();
                    // 连续播放
                    if (!hasStart)
                    {
                        if (Bag.getItemsNum() == 0)
                        {
                            throw new RowException("物品为空！");
                        }
                        Bag.clearOperatorStack();
                        hasStart = true;

                        dispatcherTimer.Tick += new EventHandler((s, args) =>
                        {
                            try
                            {
                                Bag.toNextStep();
                                reloadValue();
                            }
                            catch (bag.bag_operators.StepEndException)
                            {
                                hasEnd = true;
                                hasStop = true;
                                start_button.Content = "重播";
                                dispatcherTimer.Stop();
                            }
                            catch (bag.RowException ex)
                            {
                                MessageBox.Show(ex.msg);
                                dispatcherTimer.Stop();
                            }
                            catch
                            {
                                MessageBox.Show("发生异常!");
                                dispatcherTimer.Stop();
                            }
                        });
                        dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / animationSpeed);
                    }
                    dispatcherTimer.Start();
                }
                else 
                {
                    // 暂停播放
                    dispatcherTimer.Stop();
                    start_button.Content = "继续";
                    hasStop = true;
                }
                return 0;
            });
        }

        private void to_next_step(object sender, RoutedEventArgs e)
        {
            // 跳到下一步
            exclute(() =>
            {
                if (!hasStart)
                {
                    if (Bag.getItemsNum() == 0)
                    {
                        throw new RowException("物品为空！");
                    }
                    start_button.Content = "继续";
                    Bag.clearOperatorStack();
                    hasStart = true;

                    dispatcherTimer.Tick += new EventHandler((s, args) =>
                    {
                        try
                        {
                            Bag.toNextStep();
                            reloadValue();
                        } 
                        catch (bag.bag_operators.StepEndException)
                        {
                            hasEnd = true;
                            hasStop = true;
                            start_button.Content = "重播";
                            dispatcherTimer.Stop();
                        }
                        catch (bag.RowException ex)
                        {
                            MessageBox.Show(ex.msg);
                            dispatcherTimer.Stop();
                        }
                        catch
                        {
                            MessageBox.Show("发生异常!");
                            dispatcherTimer.Stop();
                        }
                    });
                    dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / animationSpeed);
                }
                if (!hasStop)
                {
                    // 暂停播放
                    dispatcherTimer.Stop();
                    start_button.Content = "继续";
                    hasStop = true;
                }
                Bag.toNextStep();
                reloadValue();
                return 0;
            });
        }

        private void to_end(object sender, RoutedEventArgs e)
        {
            // 跳到最后
            if (hasEnd)
            {
                return;
            }

            exclute(() =>
            {
                if (!hasStart)
                {
                    if (Bag.getItemsNum() == 0)
                    {
                        throw new RowException("物品为空！");
                    }
                    Bag.clearOperatorStack();
                    hasStart = true;

                    dispatcherTimer.Tick += new EventHandler((s, args) =>
                    {
                        try
                        {
                            Bag.toNextStep();
                            reloadValue();
                        }
                        catch (bag.bag_operators.StepEndException)
                        {
                            hasEnd = true;
                            hasStop = true;
                            start_button.Content = "重播";
                            dispatcherTimer.Stop();
                        }
                        catch (bag.RowException ex)
                        {
                            MessageBox.Show(ex.msg);
                            dispatcherTimer.Stop();
                        }
                        catch
                        {
                            MessageBox.Show("发生异常!");
                            dispatcherTimer.Stop();
                        }
                    });
                    dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / animationSpeed);
                }
                else if (!hasStop)
                {
                    dispatcherTimer.Stop();
                }
                return 0;
            });

            // 清除当前物品状态
            foreach (Item item in Bag.getItemList())
            {
                item.setUnuseStatus();
            }
            left_capacity_search_text.Text = "";
            left_item_num_search_text.Text = "";
            max_value_record_canvas.Children.Clear();

            BagOperatorStack.showAnimation = false;
            while (!hasEnd)
            {
                //Bag.toNextStep();
                try
                {
                    Bag.toNextStep();
                }
                catch (bag.bag_operators.StepEndException)
                {
                    hasEnd = true;
                    hasStop = true;
                    start_button.Content = "重播";
                    break;
                }
                catch (bag.RowException ex)
                {
                    MessageBox.Show(ex.msg);
                    break;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发生异常!" + ex.Message);
                    break;
                }
            }
            reloadValue();
            BagOperatorStack.showAnimation = true;
        }

        public void printOperatorExplain(String explain)
        {
            explain_text.Text = explain;
        }

        // 有关步骤生成算法的选项在每一次刚开始时、重置时、重播时生效
        public static bool skipItemOverCapacityStatusBuffer = false;
        private void skipItemOverCapacityChecked(object sender, RoutedEventArgs e)
        {
            if (keepTryAfterFullFillStatusBuffer)
            {
                keep_try_after_full_fill_check_box.IsChecked = false;
            }
            if (hasStart)
            {
                skipItemOverCapacityStatusBuffer = true;
            }
            else
            {
                skipItemOverCapacityStatusBuffer = true;
                tryUpdateAllStepGeneratorOption();
            }

        }

        private void skipItemOverCapacityUnchecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                skipItemOverCapacityStatusBuffer = false;
            }
            else
            {
                skipItemOverCapacityStatusBuffer = false;
                tryUpdateAllStepGeneratorOption();
            }
        }

        bool keepTryAfterFullFillStatusBuffer = false;
        private void keepTryAfterFullFillChecked(object sender, RoutedEventArgs e)
        {
            if (skipItemOverCapacityStatusBuffer)
            {
                skip_item_over_capacity_check_box.IsChecked = false;
            }
            if (hasStart)
            {
                keepTryAfterFullFillStatusBuffer = true;
            } 
            else
            {
                keepTryAfterFullFillStatusBuffer = true;
                tryUpdateAllStepGeneratorOption();
            }
        }

        private void keepTryAfterFullFillUnchecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                keepTryAfterFullFillStatusBuffer = false;
            }
            else
            {
                keepTryAfterFullFillStatusBuffer = false;
                tryUpdateAllStepGeneratorOption();
            }
        }

        bool useTableToReduceStepBuffer = true;
        private void useTableToReduceStepChecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                useTableToReduceStepBuffer = true;
            }
            else
            {
                useTableToReduceStepBuffer = true;
                tryUpdateAllStepGeneratorOption();
            }
        }

        private void useTableToReduceStepUnchecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                useTableToReduceStepBuffer = false;
            }
            else
            {
                useTableToReduceStepBuffer = false;
                tryUpdateAllStepGeneratorOption();
            }
        }

        bool disposableFillWhileCapacityEnoughBuffer = true;
        private void disposableFillWhileCapacityEnoughChecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                disposableFillWhileCapacityEnoughBuffer = true;
            }
            else
            {
                disposableFillWhileCapacityEnoughBuffer = true;
                tryUpdateAllStepGeneratorOption();
            }
        }

        private void disposableFillWhileCapacityEnoughUnchecked(object sender, RoutedEventArgs e)
        {
            if (hasStart)
            {
                disposableFillWhileCapacityEnoughBuffer = false;
            }
            else
            {
                disposableFillWhileCapacityEnoughBuffer = false;
                tryUpdateAllStepGeneratorOption();
            }
        }

        private void tryUpdateAllStepGeneratorOption()
        {
            if (skipItemOverCapacityStatusBuffer != BagOperatorStack.skipItemOverCapacityStatus
                || keepTryAfterFullFillStatusBuffer != BagOperatorStack.keepTryAfterFullFillStatus
                || useTableToReduceStepBuffer != BagOperatorStack.useTableToReduceStepStatus
                || disposableFillWhileCapacityEnoughBuffer != BagOperatorStack.disposableFillWhileCapacityEnoughStatus)
            {
                BagOperatorStack.skipItemOverCapacityStatus = skipItemOverCapacityStatusBuffer;
                BagOperatorStack.keepTryAfterFullFillStatus = keepTryAfterFullFillStatusBuffer;
                BagOperatorStack.useTableToReduceStepStatus = useTableToReduceStepBuffer;
                BagOperatorStack.disposableFillWhileCapacityEnoughStatus = disposableFillWhileCapacityEnoughBuffer;
                Bag.clearOperatorStack();
            }
        }

        private void debug(object sender, RoutedEventArgs e)
        {
            item_num_text.Text = "10";
            capacity_text.Text = "20";
            keep_try_after_full_fill_check_box.IsChecked = false;
            skip_item_over_capacity_check_box.IsChecked = false;
            /*            for (int i = 0; i < 1000; i++)
                        {
                            random_gen_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result1 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result2 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result3 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result4 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            if (result1 != result2 || result2 != result3 || result3 != result4)
                            {
                                MessageBox.Show("完啦1");
                                break;
                            }
                        }
                        skip_item_over_capacity_check_box.IsChecked = true;
                        for (int i = 0; i < 1000; i++)
                        {
                            random_gen_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result1 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result2 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result3 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result4 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            if (result1 != result2 || result2 != result3 || result3 != result4)
                            {
                                MessageBox.Show("完啦2 " + i + " result1 " + result1 + " result2 " + result2 + " result3 " + result3 + " result4 " + result4);
                                break;
                            }
                            if (result1 != result3)
                            {
                                MessageBox.Show("完啦2 " + i + " result1 " + result1 + " result3 " + result3);
                                break;
                            }
                        }
                        keep_try_after_full_fill_check_box.IsChecked = true;
                        for (int i = 0; i < 1000; i++)
                        {
                            random_gen_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result1 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            use_table_to_reduce_step_check_box.IsChecked = true;
                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result2 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result3 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            disposable_fill_while_capacity_enough_check_box.IsChecked = false;
                            use_table_to_reduce_step_check_box.IsChecked = false;
                            end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            int result4 = Bag.max_value;
                            reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                            if (result1 != result2 || result2 != result3 || result3 != result4)
                            {
                                MessageBox.Show("完啦3");
                                break;
                            }
                        }*/
            disposable_fill_while_capacity_enough_check_box.IsChecked = true;
            for (int i = 0; i < 1000; i++)
            {
                random_gen_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                use_table_to_reduce_step_check_box.IsChecked = true;
                skip_item_over_capacity_check_box.IsChecked = false;
                end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                int result1 = Bag.max_value;
                reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                use_table_to_reduce_step_check_box.IsChecked = true;
                skip_item_over_capacity_check_box.IsChecked = true;
                end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                int result2 = Bag.max_value;
                reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                use_table_to_reduce_step_check_box.IsChecked = false;
                skip_item_over_capacity_check_box.IsChecked = false;
                end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                int result3 = Bag.max_value;
                reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                use_table_to_reduce_step_check_box.IsChecked = false;
                skip_item_over_capacity_check_box.IsChecked = true;
                end_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                int result4 = Bag.max_value;
                reset_button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                if (result1 != result2 || result2 != result3 || result3 != result4)
                {
                    MessageBox.Show("完啦4");
                    break;
                }
            }
        }

        public double animationSpeed = 1;

        private void change_speed(object sender, RoutedEventArgs e)
        {
            exclute(() =>
            {
                double newSpeed = double.Parse(change_speed_text_box.Text);
                if (newSpeed < 0.1 || newSpeed > 10)
                {
                    throw new RowException("请将参数控制在合理范围内！(0.1 ~ 10)");
                }
                animationSpeed = newSpeed;
                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / animationSpeed);
                    dispatcherTimer.Start();
                }
                return 0;
            });
        }

        private void change_speed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                exclute(() =>
                {
                    double newSpeed = double.Parse(change_speed_text_box.Text);
                    if (newSpeed < 0.1 || newSpeed > 10)
                    {
                        throw new RowException("请将参数控制在合理范围内！(0.1 ~ 10)");
                    }
                    animationSpeed = newSpeed;
                    if (dispatcherTimer.IsEnabled)
                    {
                        dispatcherTimer.Stop();
                        dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / animationSpeed);
                        dispatcherTimer.Start();
                    }
                    return 0;
                });
            }
        }


        private void max_value_saerch(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                exclute(() =>
                {
                    if (left_capacity_search_text.Text != "" && left_item_num_search_text.Text != "")
                    {
                        int leftCapacitySearch = int.Parse(left_capacity_search_text.Text);
                        int leftItemNumSearch = int.Parse(left_item_num_search_text.Text);

                        MaxValue maxValue = BagOperatorStack.maxValueTable[leftCapacitySearch, Bag.getItemsNum() - leftItemNumSearch];
                        if (maxValue != null)
                        {
                            maxValue.searchBlock(this, leftCapacitySearch, leftItemNumSearch);
                        } 
                        else
                        {
                            MaxValue.showNotFound(this);
                        }
                    }
                    return 0;
                });
            }
        }

        private void max_value_saerch(object sender, RoutedEventArgs e)
        {
            exclute(() =>
            {
                if (left_capacity_search_text.Text != "" && left_item_num_search_text.Text != "")
                {
                    int leftCapacitySearch = int.Parse(left_capacity_search_text.Text);
                    int leftItemNumSearch = int.Parse(left_item_num_search_text.Text);
                    if (leftCapacitySearch < 0 || leftCapacitySearch > Bag.capacity)
                    {
                        throw new RowException("剩余背包空间不应该小于零或大于背包容量!");
                    }
                    if (leftItemNumSearch < 0 || leftItemNumSearch > Bag.getItemsNum())
                    {
                        throw new RowException("剩余物品个数不应该小于零或大于物品总数!");
                    }

                    MaxValue maxValue = BagOperatorStack.maxValueTable[leftCapacitySearch, Bag.getItemsNum() - leftItemNumSearch];
                    if (maxValue != null)
                    {
                        maxValue.searchBlock(this, leftCapacitySearch, leftItemNumSearch);
                    }
                    else
                    {
                        MaxValue.showNotFound(this);
                    }
                }
                return 0;
            });
        }
     
        List<int> sortMethed = new List<int>();
        private void sort_item(object sender, RoutedEventArgs e)
        {
            if (!askWhenChangeBagAfterStart()) return;
            exclute(() =>
            {
                if (sortMethed.Count == 0)
                {
                    throw new RowException("请选择排序方式!");
                }
                List<Item> items = Bag.getItemList();
                for (int i = sortMethed.Count - 1; i >= 0; i--)
                {
                    if (sortMethed[i] == 1)
                    {
                        sortItemByWeight(items);
                    }
                    else if (sortMethed[i] == 2)
                    {
                        sortItemByValue(items);
                    }
                    else
                    {
                        sortItemByEffective(items);
                    }
                }
                Bag.resetItemListSort(items);
                reloadItems();
                reloadValue();
                capacity_text.Text = Bag.capacity.ToString();
                reloadBagBox();
                return 0;
            });
        }

        private void sortItemByWeight(List<Item> items)
        {
            items.Sort(delegate (Item A, Item B)
            {
                if (sort_by_positive_sequence_check_box.IsChecked.GetValueOrDefault())
                {
                    return A.weight.CompareTo(B.weight);
                } 
                else
                {
                    return B.weight.CompareTo(A.weight);
                }
            });
        }

        private void sortItemByValue(List<Item> items)
        {
            items.Sort(delegate (Item A, Item B)
            {
                if (sort_by_positive_sequence_check_box.IsChecked.GetValueOrDefault())
                {
                    return A.value.CompareTo(B.value);
                }
                else
                {
                    return B.value.CompareTo(A.value);
                }
            });
        }

        private void sortItemByEffective(List<Item> items)
        {
            items.Sort(delegate (Item A, Item B)
            {
                if (sort_by_positive_sequence_check_box.IsChecked.GetValueOrDefault())
                {
                    return ((double)A.value / (double)A.weight).CompareTo((double)B.value / (double)B.weight);
                }
                else
                {
                    return ((double)B.value / (double)B.weight).CompareTo((double)A.value / (double)A.weight);
                }
            });
        }

        Brush First = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        Brush Second = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        Brush NoUse = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        private void sortByWeightChecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[1] == 2)
                {
                    sort_by_value_check_box.IsChecked = false;
                } 
                else
                {
                    sort_by_effective_check_box.IsChecked = false;
                }
                sortMethed.Add(1);
                sort_by_weight_text.Foreground = Second;
            }
            else if (sortMethed.Count == 1)
            {
                sortMethed.Add(1);
                sort_by_weight_text.Foreground = Second;
            } 
            else
            {
                sortMethed.Add(1);
                sort_by_weight_text.Foreground = First;
            }
        }

        private void sortByValueChecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[1] == 1)
                {
                    sort_by_weight_check_box.IsChecked = false;
                }
                else
                {
                    sort_by_effective_check_box.IsChecked = false;
                }
                sortMethed.Add(2);
                sort_by_value_text.Foreground = Second;
            }
            else if (sortMethed.Count == 1)
            {
                sortMethed.Add(2);
                sort_by_value_text.Foreground = Second;
            }
            else
            {
                sortMethed.Add(2);
                sort_by_value_text.Foreground = First;
            }
        }

        private void sortByEffectiveChecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[1] == 2)
                {
                    sort_by_value_check_box.IsChecked = false;
                }
                else
                {
                    sort_by_weight_check_box.IsChecked = false;
                }
                sortMethed.Add(3);
                sort_by_effective_text.Foreground = Second;
            }
            else if (sortMethed.Count == 1)
            {
                sortMethed.Add(3);
                sort_by_effective_text.Foreground = Second;
            }
            else
            {
                sortMethed.Add(3);
                sort_by_effective_text.Foreground = First;
            }
        }

        private void sortByWeightUnchecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[0] == 1)
                {
                    if (sortMethed[1] == 2)
                    {
                        sort_by_value_text.Foreground = First;
                    }
                    else
                    {
                        sort_by_effective_text.Foreground = First;
                    }
                }
            }
            sortMethed.Remove(1);
            sort_by_weight_text.Foreground = NoUse;
        }

        private void sortByValueUnchecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[0] == 2)
                {
                    if (sortMethed[1] == 1)
                    {
                        sort_by_weight_text.Foreground = First;
                    }
                    else
                    {
                        sort_by_effective_text.Foreground = First;
                    }
                }
            }
            sortMethed.Remove(2);
            sort_by_value_text.Foreground = NoUse;
        }

        private void sortByEffectiveUnchecked(object sender, RoutedEventArgs e)
        {
            if (sortMethed.Count == 2)
            {
                if (sortMethed[0] == 3)
                {
                    if (sortMethed[1] == 1)
                    {
                        sort_by_weight_text.Foreground = First;
                    }
                    else
                    {
                        sort_by_value_text.Foreground = First;
                    }
                }
            }
            sortMethed.Remove(3);
            sort_by_effective_text.Foreground = NoUse;
        }

        private void random_gen_tendency(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Random_Generator.randomGeneratorTendencyRate = e.NewValue / 10;
        }
    }
}