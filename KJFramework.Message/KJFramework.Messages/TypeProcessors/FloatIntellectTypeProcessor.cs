using System;
using KJFramework.Messages.Analysers;
using KJFramework.Messages.Attributes;
using KJFramework.Messages.Exceptions;
using KJFramework.Messages.Helpers;
using KJFramework.Messages.Proxies;

namespace KJFramework.Messages.TypeProcessors
{
    /// <summary>
    ///     Float类型智能处理器，提供了相关的基本操作。
    /// </summary>
    public class FloatIntellectTypeProcessor : IntellectTypeProcessor
    {
        #region 构造函数

        /// <summary>
        ///     Float类型智能处理器，提供了相关的基本操作。
        /// </summary>
        public FloatIntellectTypeProcessor()
        {
            _supportedType = typeof (float);
        }

        #endregion

        #region Overrides of IntellectTypeProcessor

        /// <summary>
        ///     从第三方客户数据转换为元数据
        /// </summary>
        /// <param name="memory">需要填充的字节数组</param>
        /// <param name="offset">需要填充数组的起始偏移量</param>
        /// <param name="attribute">当前字段标注的属性</param>
        /// <param name="value">第三方客户数据</param>
        public override void Process(byte[] memory, int offset, IntellectPropertyAttribute attribute, object value)
        {
            unsafe
            {
                float temp = (float)value;
                BitConvertHelper.GetBytes(*(int*)&temp, memory, offset);
            }
        }

        /// <summary>
        ///     从第三方客户数据转换为元数据
        /// </summary>
        /// <param name="proxy">内存片段代理器</param>
        /// <param name="attribute">字段属性</param>
        /// <param name="analyseResult">分析结果</param>
        /// <param name="target">目标对象实例</param>
        /// <param name="isArrayElement">当前写入的值是否为数组元素标示</param>
        public override void Process(IMemorySegmentProxy proxy, IntellectPropertyAttribute attribute, ToBytesAnalyseResult analyseResult, object target, bool isArrayElement = false, bool isNullable = false)
        {
            float value;
            if (!isNullable) value = analyseResult.GetValue<float>(target);
            else
            {
                float? nullableValue = analyseResult.GetValue<float?>(target);
                if (nullableValue == null)
                {
                    if (!attribute.IsRequire) return;
                    throw new PropertyNullValueException(
                        string.Format(ExceptionMessage.EX_PROPERTY_VALUE, attribute.Id,
                                        analyseResult.Property.Name,
                                        analyseResult.Property.PropertyType));
                }
                value = (float)nullableValue;
            }
            if (attribute.AllowDefaultNull && value.Equals(DefaultValue.Float) && !isArrayElement) return;
            proxy.WriteByte((byte)attribute.Id);
            proxy.WriteFloat(value);
        }

        /// <summary>
        ///     从第三方客户数据转换为元数据
        /// </summary>
        /// <param name="attribute">当前字段标注的属性</param>
        /// <param name="value">第三方客户数据</param>
        /// <returns>返回转换后的元数据</returns>
        /// <exception cref="Exception">转换失败</exception>
        public override byte[] Process(IntellectPropertyAttribute attribute, object value)
        {
            if (attribute == null) throw new System.Exception("非法的智能属性标签。");
            if (attribute.IsRequire && value == null) throw new System.Exception("无法处理非法的类型值。");
            return BitConverter.GetBytes((float) value);
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="attribute">当前字段标注的属性</param>
        /// <param name="data">元数据</param>
        /// <returns>返回转换后的第三方客户数据</returns>
        /// <exception cref="Exception">转换失败</exception>
        public override object Process(IntellectPropertyAttribute attribute, byte[] data)
        {
            return Process(attribute, data, 0);
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="attribute">当前字段标注的属性</param>
        /// <param name="data">元数据</param>
        /// <param name="offset">元数据所在的偏移量</param>
        /// <param name="length">元数据长度</param>
        /// <returns>返回转换后的第三方客户数据</returns>
        /// <exception cref="Exception">转换失败</exception>
        public override object Process(IntellectPropertyAttribute attribute, byte[] data, int offset, int length = 0)
        {
            if (attribute == null) throw new System.Exception("非法的智能属性标签。");
            if (attribute.IsRequire && data == null) throw new System.Exception("无法处理非法的类型值。");
            return BitConverter.ToSingle(data, offset);
        }

        /// <summary>
        ///     从元数据转换为第三方客户数据
        /// </summary>
        /// <param name="instance">目标对象</param>
        /// <param name="result">分析结果</param>
        /// <param name="data">元数据</param>
        /// <param name="offset">元数据所在的偏移量</param>
        /// <param name="length">元数据长度</param>
        public override void Process(object instance, GetObjectAnalyseResult result, byte[] data, int offset, int length = 0)
        {
            if(result.Nullable) result.SetValue<float?>(instance, BitConverter.ToSingle(data, offset));
            else result.SetValue(instance, BitConverter.ToSingle(data, offset));
        }

        #endregion
    }
}