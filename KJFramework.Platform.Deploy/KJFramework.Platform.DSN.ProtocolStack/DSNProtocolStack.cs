using System;
using System.Collections.Generic;
using KJFramework.Basic.Enum;
using KJFramework.EventArgs;
using KJFramework.IO.Helper;
using KJFramework.Logger;
using KJFramework.Messages.Engine;
using KJFramework.Platform.Deploy.DSN.ProtocolStack.Statistics;
using KJFramework.Statistics;

namespace KJFramework.Platform.Deploy.DSN.ProtocolStack
{
    /// <summary>
    ///     动态服务协议栈，提供了相关的基本操作。
    /// </summary>
    public class DSNProtocolStack : Net.ProtocolStacks.ProtocolStack<DSNMessage>, IStatisticable<IStatistic>
    {
        #region Constructor

        /// <summary>
        ///     动态服务协议栈，提供了相关的基本操作。
        /// </summary>
        public DSNProtocolStack()
        {
            DSNProtocolStackStatistic statistic = new DSNProtocolStackStatistic();
            statistic.Initialize(this);
            _statistics.Add(StatisticTypes.Other, statistic);
        }

        ~DSNProtocolStack()
        {
            if (_statistics != null)
            {
                foreach (KeyValuePair<StatisticTypes, IStatistic> keyValuePair in _statistics)
                {
                    keyValuePair.Value.Close();
                }
                _statistics.Clear();
                _statistics = null;
            }
        }

        #endregion

        #region Members

        private Dictionary<StatisticTypes, IStatistic> _statistics = new Dictionary<StatisticTypes, IStatistic>();
        private Dictionary<int, Type> _messageTypes = new Dictionary<int, Type>();

        #endregion

        #region Overrides of ProtocolStack<DynamicServiceMessage>

        /// <summary>
        ///     初始化
        /// </summary>
        /// <returns>
        ///     返回初始化的结果
        /// </returns>
        /// <exception cref="T:KJFramework.Net.Exception.InitializeFailedException">初始化失败</exception>
        public override bool Initialize()
        {
            _messageTypes.Add(0, typeof(DSNBeginTransferFileRequestMessage));
            _messageTypes.Add(1, typeof(DSNBeginTransferFileResponseMessage));
            _messageTypes.Add(2, typeof(DSNTransferDataMessage));
            _messageTypes.Add(3, typeof(DSNEndTransferFileRequestMessage));
            _messageTypes.Add(4, typeof(DSNEndTransferFileResponseMessage));
            _messageTypes.Add(5, typeof(DSNLossCompensationMessage));
            _messageTypes.Add(6, typeof(DSNBeginDeployMessage));
            _messageTypes.Add(7, typeof(DSNEndDeployMessage));
            _messageTypes.Add(8, typeof(DSNDeployStatusReportMessage));
            _messageTypes.Add(9, typeof(DSNUnDeployServiceRequestMessage));
            _messageTypes.Add(10, typeof(DSNUnDeployServiceResponseMessage));
            _messageTypes.Add(11, typeof(DSNStartServiceRequestMessage));
            _messageTypes.Add(12, typeof(DSNStartServiceResponseMessage));
            _messageTypes.Add(13, typeof(DSNStopServiceRequestMessage));
            _messageTypes.Add(14, typeof(DSNStopServiceResponseMessage));
            _messageTypes.Add(15, typeof(DSNGetLocalServiceInfomationRequestMessage));
            _messageTypes.Add(16, typeof(DSNGetLocalServiceInfomationResponseMessage));
            return true;
        }

        /// <summary>
        /// 解析元数据
        /// </summary>
        /// <param name="data">元数据</param>
        /// <returns>
        /// 返回能否解析的一个标示
        /// </returns>
        public override List<DSNMessage> Parse(byte[] data)
        {
            int offset = 0;
            int totalLength;
            List<DSNMessage> messages = new List<DSNMessage>();
            try
            {
                while (offset < data.Length)
                {
                    totalLength = BitConverter.ToInt32(ByteArrayHelper.GetReallyData(data, offset, 4), 0);
                    if (totalLength > data.Length)
                    {
                        ParseMessageFailedHandler(new LightSingleArgEventArgs<byte[]>(data));
                        return messages;
                    }
                    byte[] messageData = ByteArrayHelper.GetReallyData(data, offset, totalLength);
                    int protocolId = BitConverter.ToInt32(ByteArrayHelper.GetNextData(messageData, 18, 4), 0);
                    Type messageType = GetMessageType(protocolId);
                    if (messageType == null)
                    {
                        ParseMessageFailedHandler(new LightSingleArgEventArgs<byte[]>(messageData));
                        return messages;
                    }
                    offset += messageData.Length;
                    DSNMessage message;
                    try
                    {
                        //使用智能对象引擎进行解析
                        message = IntellectObjectEngine.GetObject<DSNMessage>(messageType, messageData);
                        if (message == null)
                        {
                            ParseMessageFailedHandler(new LightSingleArgEventArgs<byte[]>(messageData));
                            return messages;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ParseMessageFailedHandler(new LightSingleArgEventArgs<byte[]>(messageData));
                        Logs.Logger.Log(ex);
                        continue;
                    }
                    ParseMessageSuccessedHandler(new LightSingleArgEventArgs<DSNMessage>(message));
                    messages.Add(message);
                    if (data.Length - offset < 4)
                    {
                        break;
                    }
                }
                return messages;
            }
            catch (System.Exception ex)
            {
                Logs.Logger.Log(ex);
                return messages;
            }
        }

        /// <summary>
        /// 将一个消息转换为2进制形式
        /// </summary>
        /// <param name="message">需要转换的消息</param>
        /// <returns>
        /// 返回转换后的2进制
        /// </returns>
        public override byte[] ConvertToBytes(DSNMessage message)
        {
            if (message == null)
            {
                return null;
            }
            message.Bind();
            return message.IsBind ? message.Body : null;
        }

        #endregion

        #region Implementation of IStatisticable<IStatistic>

        /// <summary>
        /// 获取或设置统计器
        /// </summary>
        public Dictionary<StatisticTypes, IStatistic> Statistics
        {
            get { return _statistics; }
            set { _statistics = value; }
        }

        #endregion

        #region Events

        /// <summary>
        ///     解析消息失败
        /// </summary>
        internal event EventHandler<LightSingleArgEventArgs<byte[]>> ParseMessageFailed;
        protected void ParseMessageFailedHandler(LightSingleArgEventArgs<byte[]> e)
        {
            EventHandler<LightSingleArgEventArgs<byte[]>> handler = ParseMessageFailed;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     解析消息成功
        /// </summary>
        internal event EventHandler<LightSingleArgEventArgs<DSNMessage>> ParseMessageSuccessed;
        protected void ParseMessageSuccessedHandler(LightSingleArgEventArgs<DSNMessage> e)
        {
            EventHandler<LightSingleArgEventArgs<DSNMessage>> handler = ParseMessageSuccessed;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     转换元数据失败
        /// </summary>
        internal event EventHandler<LightSingleArgEventArgs<DSNMessage>> MessageToByteFailed;
        protected void MessageToByteFailedHandler(LightSingleArgEventArgs<DSNMessage> e)
        {
            EventHandler<LightSingleArgEventArgs<DSNMessage>> handler = MessageToByteFailed;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     转换元数据成功
        /// </summary>
        internal event EventHandler<LightSingleArgEventArgs<DSNMessage>> MessageToByteSuccessed;
        protected void MessageToByteSuccessedHandler(LightSingleArgEventArgs<DSNMessage> e)
        {
            EventHandler<LightSingleArgEventArgs<DSNMessage>> handler = MessageToByteSuccessed;
            if (handler != null) handler(this, e);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     尝试用过一个协议编号获取指定的消息类型
        /// </summary>
        /// <param name="protocolId">协议编号</param>
        /// <returns>返回消息类型</returns>
        private Type GetMessageType(int protocolId)
        {
            Type value;
            if (_messageTypes.TryGetValue(protocolId, out value))
            {
                return value;
            }
            return null;
        }

        #endregion
    }
}