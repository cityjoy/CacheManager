﻿using System;
using CacheManager.Core.Internal;
using static CacheManager.Core.Utility.Guard;
using StackRedis = StackExchange.Redis;

namespace CacheManager.Redis
{
    //// this looks strange but yes it has a reason. I could use a serializer to get and set values
    //// from redis but using the "native" supported types from Stackexchange.Redis, instead of using
    //// a serializer is up to 10x faster... so its more than worth the effort...
    //// basically I have to cast the values to and from RedisValue which has implicit conversions
    //// to/from those types defined internally... I cannot simply cast to my TCacheValue, because its
    //// generic, and not defined as class or struct or anything... so there is basically no other way

    internal interface IRedisValueConverter
    {
        StackRedis.RedisValue ToRedisValue<T>(T value);

        T FromRedisValue<T>(StackRedis.RedisValue value, string valueType);
    }

    internal interface IRedisValueConverter<T>
    {
        StackRedis.RedisValue ToRedisValue(T value);

        T FromRedisValue(StackRedis.RedisValue value, string valueType);
    }

    internal class RedisValueConverter :
        IRedisValueConverter,
        IRedisValueConverter<byte[]>,
        IRedisValueConverter<string>,
        IRedisValueConverter<int>,
        IRedisValueConverter<double>,
        IRedisValueConverter<bool>,
        IRedisValueConverter<long>,
        IRedisValueConverter<object>
    {
        private static readonly Type ByteArrayType = typeof(byte[]);
        private static readonly Type StringType = typeof(string);
        private static readonly Type IntType = typeof(int);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type BoolType = typeof(bool);
        private static readonly Type LongType = typeof(long);
        private readonly ICacheSerializer serializer;

        public RedisValueConverter(ICacheSerializer serializer)
        {
            NotNull(serializer, nameof(serializer));

            this.serializer = serializer;
        }

        StackRedis.RedisValue IRedisValueConverter<byte[]>.ToRedisValue(byte[] value) => value;

        byte[] IRedisValueConverter<byte[]>.FromRedisValue(StackRedis.RedisValue value, string valueType) => value;

        StackRedis.RedisValue IRedisValueConverter<string>.ToRedisValue(string value) => value;

        string IRedisValueConverter<string>.FromRedisValue(StackRedis.RedisValue value, string valueType) => value;

        StackRedis.RedisValue IRedisValueConverter<int>.ToRedisValue(int value) => value;

        int IRedisValueConverter<int>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (int)value;

        StackRedis.RedisValue IRedisValueConverter<double>.ToRedisValue(double value) => value;

        double IRedisValueConverter<double>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (double)value;

        StackRedis.RedisValue IRedisValueConverter<bool>.ToRedisValue(bool value) => value;

        bool IRedisValueConverter<bool>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (bool)value;

        StackRedis.RedisValue IRedisValueConverter<long>.ToRedisValue(long value) => value;

        long IRedisValueConverter<long>.FromRedisValue(StackRedis.RedisValue value, string valueType) => (long)value;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Scope = "member", Target = "CacheManager.Redis.RedisValueConverter.#CacheManager.Redis.IRedisValueConverter`1<System.Object>.ToRedisValue(System.Object)", Justification = "For performance reasons we don't do checks at this point. Also, its internally used only.")]
        StackRedis.RedisValue IRedisValueConverter<object>.ToRedisValue(object value)
        {
            var valueType = value.GetType();
            if (valueType == ByteArrayType)
            {
                var converter = (IRedisValueConverter<byte[]>)this;
                return converter.ToRedisValue((byte[])value);
            }
            else if (valueType == StringType)
            {
                var converter = (IRedisValueConverter<string>)this;
                return converter.ToRedisValue((string)value);
            }
            else if (valueType == IntType)
            {
                var converter = (IRedisValueConverter<int>)this;
                return converter.ToRedisValue((int)value);
            }
            else if (valueType == DoubleType)
            {
                var converter = (IRedisValueConverter<double>)this;
                return converter.ToRedisValue((double)value);
            }
            else if (valueType == BoolType)
            {
                var converter = (IRedisValueConverter<bool>)this;
                return converter.ToRedisValue((bool)value);
            }
            else if (valueType == LongType)
            {
                var converter = (IRedisValueConverter<long>)this;
                return converter.ToRedisValue((long)value);
            }

            return this.serializer.Serialize(value);
        }

        object IRedisValueConverter<object>.FromRedisValue(StackRedis.RedisValue value, string valueType)
        {
            if (valueType == ByteArrayType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<byte[]>)this;
                return converter.FromRedisValue(value, valueType);
            }
            else if (valueType == StringType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<string>)this;
                return converter.FromRedisValue(value, valueType);
            }
            else if (valueType == IntType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<int>)this;
                return converter.FromRedisValue(value, valueType);
            }
            else if (valueType == DoubleType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<double>)this;
                return converter.FromRedisValue(value, valueType);
            }
            else if (valueType == BoolType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<bool>)this;
                return converter.FromRedisValue(value, valueType);
            }
            else if (valueType == LongType.AssemblyQualifiedName)
            {
                var converter = (IRedisValueConverter<long>)this;
                return converter.FromRedisValue(value, valueType);
            }

            return this.Deserialize(value, valueType);
        }

        public StackRedis.RedisValue ToRedisValue<T>(T value) => this.serializer.Serialize(value);

        public T FromRedisValue<T>(StackRedis.RedisValue value, string valueType) => (T)this.Deserialize(value, valueType);

        private object Deserialize(StackRedis.RedisValue value, string valueType)
        {
            var type = Type.GetType(valueType, false);
            EnsureNotNull(type, "Type could not be loaded, {0}.", valueType);

            return this.serializer.Deserialize(value, type);
        }
    }
}