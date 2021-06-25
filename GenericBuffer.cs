using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace YFPos.Utils
{
    /// <summary>
    /// 通用缓冲类
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class GenericBuffer<T>
    {
        #region --字段/属性--
        /// <summary>
        /// 缓冲队列
        /// </summary>
        private Queue<T> _queue = null;

        /// <summary>
        /// 缓冲队列
        /// </summary>
        private Queue<T> Queue
        {
            get
            {
                return _queue;
            }
        }

        /// <summary>
        /// 获取队列元素个数
        /// </summary>
        public int QueueItemCount
        {
            get
            {
                return this._queue.Count;
            }
        }

        /// <summary>
        /// 缓冲队列头是否正在处理
        /// </summary>
        private bool _queueHasItems = false;
        /// <summary>
        /// 缓冲队列头是否正在处理
        /// </summary>
        private bool QueueHasItems
        {
            get
            {
                return _queueHasItems;
            }
            set
            {
                _queueHasItems = value;
            }
        }
        #endregion

        #region --构造器--
        /// <summary>
        /// 构造器
        /// </summary>
        public GenericBuffer()
        {
            _queue = new Queue<T>();
        }
        #endregion

        #region --方法--
        /// <summary>
        /// 将元素送入缓冲区
        /// </summary>
        /// <param name="item">缓冲元素</param>
        public void EnQueue(T item)
        {
            LogHelper.WriteLog("将元素送入缓冲区,队列元素数{0}", this.Queue.Count);
            lock (_queue)
            {
                if (this._queueHasItems)
                {
                    this._queue.Enqueue(item);
                }
                else
                {
                    Action<T> handler = new Action<T>(ProcQueueItem);
                    handler.BeginInvoke(item, new AsyncCallback(ProcQueueItemCallback), this);
                    this._queueHasItems = true;
                }
            }
        }

        /// <summary>
        /// 处理单个缓冲元素
        /// </summary>
        /// <param name="item">缓冲元素</param>
        void ProcQueueItem(T item)
        {
            if (this.ItemProcEvent != null)
            {
                this.ItemProcEvent(item);
            }
        }

        /// <summary>
        /// 处理单个缓冲元素结束回调
        /// </summary>
        /// <param name="ar">回调参数</param>
        void ProcQueueItemCallback(IAsyncResult ar)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(BeginProcQueueItem));
        }

        /// <summary>
        /// 处理单个缓冲元素
        /// </summary>
        /// <param name="state">状态参数</param>
        private void BeginProcQueueItem(object state)
        {
            GenericBuffer<T> buffer = this;
            lock (buffer.Queue)
            {
                if (buffer.Queue.Count > 0)
                {
                    Action<T> handler = new Action<T>(ProcQueueItem);
                    T pItem = buffer.Queue.Dequeue();
                    LogHelper.WriteLog("处理单个缓冲元素,队列元素数{0}", this.Queue.Count);
                    handler.BeginInvoke(pItem, new AsyncCallback(ProcQueueItemCallback), this);
                }
                else
                {
                    buffer.QueueHasItems = false;
                }
            }
        }
        #endregion

        #region --事件--
        /// <summary>
        /// 缓冲元素处理事件
        /// </summary>
        public event Action<T> ItemProcEvent = null;
        #endregion
    }
}
