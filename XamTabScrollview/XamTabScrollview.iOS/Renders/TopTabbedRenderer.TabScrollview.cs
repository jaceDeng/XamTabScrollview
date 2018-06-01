using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace XamTabScrollview.iOS.Renders
{
    class TabScrollview : UIScrollView
    {
        //
        Action<int> ClickBlock;
        private const int tagLineheight = 2;
        private StackOrientation direction = StackOrientation.Horizontal;
        //默认开启选中标题自动居中功能
        private bool openAutoCorrection = false;
        //tab 元素宽高
        private nfloat tabWidth, tabHeight;
        //高亮下划线
        private UIColor tagLineColor = UIColor.Red;
        //tab下方标记线
        private UIView tagLine;
        private int tagIndex;
        //label 的元素
        List<UIView> Views { get; set; }
        public TabScrollview(CGRect frame) : base(frame)
        {
            this.tagLine = new UIView();
            this.tagLine.BackgroundColor = tagLineColor;  //默认标记线为红色           
            this.ShowsVerticalScrollIndicator = false;
            this.ShowsHorizontalScrollIndicator = false;
        }


        public void ConfigParameter(nfloat tabWidth, nfloat tabHeight, UIColor tagLineColor, List<UIView> viewArr, int index, Action<int> tabSwitch)
        {

            Views = viewArr;
            this.tagLine.BackgroundColor = this.tagLineColor = tagLineColor;
            this.tabHeight = tabHeight;
            this.tabWidth = tabWidth;
            tagIndex = index;
            UpdateTag(index);
            ClickBlock = tabSwitch;
            for (int i = 0; i < viewArr.Count; i++)
            {
                var item = viewArr[i];
                item.Frame = new CGRect(i * tabWidth, 0, tabWidth, tabHeight - UIApplication.SharedApplication.StatusBarFrame.Height);
                this.AddSubview(item);
                item.Tag = i;
                item.UserInteractionEnabled = true;
                //标签点击事件
                item.AddGestureRecognizer(new UITapGestureRecognizer((tap) =>
                {
                    //处理点击
                    this.UpdateTagLine((int)tap.View.Tag);

                    ClickBlock?.Invoke((int)tap.View.Tag);

                }));
            }
            this.AddSubview(tagLine);
            this.ContentSize = new CGSize(tabWidth * viewArr.Count, 0);
            this.ContentOffset = new CGPoint(0, 0);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        void UpdateTag(int index)
        {
            this.TabOffset(index);
            var label = (Views[tagIndex] as UILabel);
            if (label != null)
            {
                label.TextColor = (Views[index] as UILabel).TextColor;
            }

            if (Views[index] is UILabel)
            {
                (Views[index] as UILabel).TextColor = UIColor.White;
            }
            else
                Views[index].TintColor = UIColor.White;

            tagLine.Frame = new CGRect(index * tabWidth, tabHeight - UIApplication.SharedApplication.StatusBarFrame.Height - tagLineheight, tabWidth, tagLineheight);

        }

        void TabOffset(int index)
        {
            //水平滚动的时候,计算偏移量
            //if (direction == StackOrientation.Horizontal)
            //{
            //    //获取scrollview宽度
            //    nfloat maxWidth = this.Frame.Size.Width;
            //    //获取当前点击的tab所处的位置大小
            //    nfloat maxW = tabWidth * index;
            //    //判断tab是否处于大于屏幕一半的位置,并计算出偏移量
            //    nfloat offsethalfmaxWidth = maxW - maxWidth / 2;
            //    //当tab偏移量不足tab宽度时,计算出最小的偏移量
            //    nfloat itemOffset = offsethalfmaxWidth + tabWidth / 2;

            //    //当偏移量>0的时候,
            //    if (offsethalfmaxWidth > 0)
            //    {
            //        //假如偏移量小于一个tab的宽度,说明还没有到最初始位置,可以执行偏移
            //        if (offsethalfmaxWidth < tabWidth)
            //        {
            //            this.ContentOffset = new CGPoint(itemOffset, 0);
            //            return;
            //        }
            //        nfloat maxCont = tabWidth * Views.Count;
            //        //获取偏移的页数,减1的作用是我们的偏移是从0开始的,所以需要减去一个屏幕长度
            //        nfloat remainderx = maxCont / maxWidth - 1;
            //        //获取最后一页的偏移量
            //        nfloat remainder = maxCont % maxWidth;

            //        //获取到最大偏移量
            //        nfloat maxOffset = remainderx * maxWidth + remainder;


            //        //假如我们的计算的偏移量小于最大偏移,说明是可以偏移的
            //        if (itemOffset <= maxOffset)
            //        {
            //            //假如偏移量大于一个tab的宽度,判断
            //            if (itemOffset <= tabWidth)
            //            {  //当点击的偏移量小于tab的宽度的时候,归零偏移量
            //                this.ContentOffset = new CGPoint(0, 0);
            //                return;
            //            }
            //            else
            //            {
            //                this.ContentOffset = new CGPoint(itemOffset, 0);
            //            }

            //        }
            //        else
            //        {
            //            this.ContentOffset = new CGPoint(maxOffset, 0);
            //        }
            //    }
            //    else if (offsethalfmaxWidth < 0)
            //    {
            //        //判断往后滚的偏移量小于0但是却和半个tab宽度之和要大于0的时候,说明还可以进行微调滚动,
            //        if (itemOffset > 0)
            //        {
            //            this.ContentOffset = new CGPoint(itemOffset, 0);
            //            return;
            //        }
            //        //最小偏移小于0,说明往前滚,将偏移重置为初始位置
            //        this.ContentOffset = new CGPoint(0, 0);
            //    }
            //    else
            //    {
            //        this.ContentOffset = new CGPoint(0, 0);
            //    }
            //}

        }
        void AutoTabOffset(int index)
        {
            //nfloat maxWidth = this.Frame.Size.Width;//.frame.size.width;
            //nfloat currOffset = tabWidth * index;

            ////获取scrollview移动了的距离
            //nfloat pointx = this.ContentOffset.X;
            //if (tagIndex < index)
            //{ //往后滚

            //    nfloat equalvalue = maxWidth % tabWidth;
            //    if (equalvalue == 0)
            //    { //假如tab宽度等分屏幕,说明屏幕右边一定能完全显示一个tab
            //      //直接计算一个tab宽度偏移
            //        if (currOffset >= maxWidth)
            //        {
            //            //偏移一个tab长度
            //            this.ContentOffset = new CGPoint(pointx + tabWidth, 0);

            //        }
            //    }
            //    else
            //    { //tab宽度不等分屏幕,说明屏幕右边肯定有一个tab显示不全
            //      //显示不全的时候,我们需要将不全的部分偏移也计算进去

            //        if ((currOffset + tabWidth) >= maxWidth)
            //        {
            //            //偏移一个tab长度
            //            this.ContentOffset = new CGPoint(pointx + tabWidth, 0);
            //        }

            //    }

            //}
            //else
            //{ //往前滚
            //  //     NSLog(@"移动了的距离---%f---当前tag--%f", pointx, currOffset);

            //    if (currOffset == 0)
            //    {//假如回滚到第一格,初始化偏移量
            //        this.ContentOffset = new CGPoint(0, 0);
            //        return;
            //    }
            //    if (currOffset < pointx)
            //    {
            //        //往后回滚一格
            //        this.ContentOffset = new CGPoint((pointx - tabWidth), 0);
            //    }

            //}

        }
        /// <summary>
        /// 切换标记线位置
        /// </summary>
        /// <param name="index"></param>
        public void UpdateTagLine(int index)
        {
            if (tagIndex == index)
            { //如果标记重复,为了节省消耗,直接中断
                return;
            }
            //点击了
            if (openAutoCorrection)
            {
                this.TabOffset(index);

            }
            else
            {
                this.AutoTabOffset(index);// autoTabOffset:index;

            }
            if (direction == StackOrientation.Horizontal)
            {
                //标记线切换动画
                Animate(0.1, () =>
                {
                    tagLine.Frame = new CGRect(index * tabWidth, tabHeight - tagLineheight - 0.5, tabWidth, tagLineheight);
                },
                () =>
                {
                    UpdateTag(index);
                    tagIndex = index;
                });



            }
            else
            {
                //标记线切换动画
                Animate(0.1, () =>
                {

                    tagLine.Frame = new CGRect(tabWidth - tagLineheight - 0.5, tabHeight * index, tagLineheight, tabHeight);
                }, () =>
                {
                    tagIndex = index;

                });
            }
        }


    }
}