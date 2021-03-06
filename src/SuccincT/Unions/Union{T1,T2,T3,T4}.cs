﻿using System;
using System.Collections.Generic;
using SuccincT.Functional;
using SuccincT.Unions.PatternMatchers;
using static SuccincT.Functional.Unit;

namespace SuccincT.Unions
{
    public sealed class Union<T1, T2, T3, T4> : IUnion<T1, T2, T3, T4>
    {
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        private readonly T4 _value4;
        private readonly Dictionary<Variant, Func<int>> _hashCodes;
        private readonly Dictionary<Variant, Func<Union<T1, T2, T3, T4>, bool>> _unionsMatch;

        public Variant Case { get; }

        public Union(T1 value) : this(unit)
        {
            _value1 = value;
            Case = Variant.Case1;
        }

        public Union(T2 value) : this(unit)
        {
            _value2 = value;
            Case = Variant.Case2;
        }

        public Union(T3 value) : this(unit)
        {
            _value3 = value;
            Case = Variant.Case3;
        }

        public Union(T4 value) : this(unit)
        {
            _value4 = value;
            Case = Variant.Case4;
        }

        // ReSharper disable once UnusedParameter.Local - unit param used to
        // prevent JSON serializer from using this constructor to create an invalid union.
        private Union(Unit _)
        {
            _hashCodes = new Dictionary<Variant, Func<int>>
            {
                { Variant.Case1, () => _value1.GetHashCode() },
                { Variant.Case2, () => _value2.GetHashCode() },
                { Variant.Case3, () => _value3.GetHashCode() },
                { Variant.Case4, () => _value4.GetHashCode() }
            };
            _unionsMatch = new Dictionary<Variant, Func<Union<T1, T2, T3, T4>, bool>>
            {
                { Variant.Case1, other => EqualityComparer<T1>.Default.Equals(_value1, other._value1) },
                { Variant.Case2, other => EqualityComparer<T2>.Default.Equals(_value2, other._value2) },
                { Variant.Case3, other => EqualityComparer<T3>.Default.Equals(_value3, other._value3) },
                { Variant.Case4, other => EqualityComparer<T4>.Default.Equals(_value4, other._value4) }
            };
        }

        public T1 Case1 => Case == Variant.Case1 ? _value1 : throw new InvalidCaseException(Variant.Case1, Case);
        public T2 Case2 => Case == Variant.Case2 ? _value2 : throw new InvalidCaseException(Variant.Case2, Case);
        public T3 Case3 => Case == Variant.Case3 ? _value3 : throw new InvalidCaseException(Variant.Case3, Case);
        public T4 Case4 => Case == Variant.Case4 ? _value4 : throw new InvalidCaseException(Variant.Case4, Case);

        public TResult Value<TResult>()
        {
            switch (typeof(TResult))
            {
                case var t when t == typeof(T1): return (TResult)(object)Case1;
                case var t when t == typeof(T2): return (TResult)(object)Case2;
                case var t when t == typeof(T3): return (TResult)(object)Case3;
                case var t when t == typeof(T4): return (TResult)(object)Case4;
                default: throw new InvalidCaseOfTypeException(typeof(TResult));
            }
        }
        
        public bool HasValueOf<T>()
        {
            switch (Case)
            {
                case Variant.Case1:
                    return _value1.GetType() == typeof(T);
                case Variant.Case2:
                    return _value2.GetType() == typeof(T);
                case Variant.Case3:
                    return _value3.GetType() == typeof(T);
                case Variant.Case4:
                    return _value4.GetType() == typeof(T);
            }

            return false;
        }

        public IUnionFuncPatternMatcher<T1, T2, T3, T4, TResult> Match<TResult>() =>
            new UnionPatternMatcher<T1, T2, T3, T4, TResult>(this);

        public IUnionActionPatternMatcher<T1, T2, T3, T4> Match() =>
            new UnionPatternMatcher<T1, T2, T3, T4, Unit>(this);

        public override bool Equals(object obj) => obj is Union < T1, T2, T3, T4> union && UnionsEqual(union);

        public override int GetHashCode() => _hashCodes[Case]();

        public static bool operator ==(Union<T1, T2, T3, T4> a, Union<T1, T2, T3, T4> b)
        {
            var aObj = (object)a;
            var bObj = (object)b;
            return aObj == null && bObj == null || aObj != null && a.Equals(b);
        }

        public static bool operator !=(Union<T1, T2, T3, T4> a, Union<T1, T2, T3, T4> b) => !(a == b);

        public static implicit operator Union<T1, T2, T3, T4>(T1 value) => new Union<T1, T2, T3, T4>(value);
        public static implicit operator Union<T1, T2, T3, T4>(T2 value) => new Union<T1, T2, T3, T4>(value);
        public static implicit operator Union<T1, T2, T3, T4>(T3 value) => new Union<T1, T2, T3, T4>(value);
        public static implicit operator Union<T1, T2, T3, T4>(T4 value) => new Union<T1, T2, T3, T4>(value);

        private bool UnionsEqual(Union<T1, T2, T3, T4> testObject) =>
            Case == testObject.Case && _unionsMatch[Case](testObject);
    }
}
