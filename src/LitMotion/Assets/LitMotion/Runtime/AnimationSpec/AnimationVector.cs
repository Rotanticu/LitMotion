using System;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// 动画向量接口，所有向量化动画值的基类接口。
    /// </summary>
    public interface AnimationVector
    {
        public bool isInitialized
        {
            get;
            protected set;
        }
        float this[int index] { get; set; }
        AnimationVector NewVector();
        void Reset();
    }

    /// <summary>
    /// 一维动画向量（用float），适用于单通道动画（如透明度、单轴移动等）。
    /// </summary>
    public struct AnimationVector1D : AnimationVector
    {
        /// <summary>
        /// 动画值
        /// </summary>
        public float Value
        {
            get;
            internal set;
        }
        private bool _isInitialized;
        bool AnimationVector.isInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
            }
        }
        public float this[int index]
        {
            get => index == 0 ? Value : throw new IndexOutOfRangeException();
            set { if (index == 0) Value = value; else throw new IndexOutOfRangeException(); }
        }
        public AnimationVector NewVector() => new AnimationVector1D(0f);
        public void Reset() { Value = 0f; }

        public AnimationVector1D(float value = 0f)
        {
            Value = value;
            _isInitialized = true;
        }

        /// <summary>
        /// 创建全零新实例
        /// </summary>
        public static AnimationVector1D NewInstance() => new AnimationVector1D(0f);
        /// <summary>
        /// 深拷贝
        /// </summary>
        public static AnimationVector1D Copy(AnimationVector1D v) => new AnimationVector1D(v.Value);
        /// <summary>
        /// 从source拷贝到target
        /// </summary>
        public static void CopyFrom(ref AnimationVector1D target, in AnimationVector1D source) => target.Value = source.Value;
    }

    /// <summary>
    /// 二维动画向量（用Vector2），适用于2D平面动画（如位置、缩放等）。
    /// </summary>
    public struct AnimationVector2D : AnimationVector
    {
        private bool _isInitialized;
        bool AnimationVector.isInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
            }
        }
        /// <summary>
        /// 动画值（x, y）
        /// </summary>
        [SerializeField]
        private Vector2 _value;
        public AnimationVector2D(float x = 0f, float y = 0f)
        {
            _value = new Vector2(x, y);
            _isInitialized = true;
        }
        public AnimationVector2D(Vector2 v)
        {
            _value = v;
            _isInitialized = true;
        }
        public float this[int index]
        {
            get => index == 0 ? _value.x : index == 1 ? _value.y : throw new IndexOutOfRangeException();
            set { if (index == 0) _value.x = value; else if (index == 1) _value.y = value; else throw new IndexOutOfRangeException(); }
        }
        public AnimationVector NewVector() => new AnimationVector2D(0f, 0f);
        public void Reset() { _value.x = 0f; _value.y = 0f; }

        /// <summary>
        /// 创建全零新实例
        /// </summary>
        public static AnimationVector2D NewInstance() => new AnimationVector2D(0f, 0f);
        /// <summary>
        /// 深拷贝
        /// </summary>
        public static AnimationVector2D Copy(AnimationVector2D v) => new AnimationVector2D(v._value);
        /// <summary>
        /// 从source拷贝到target
        /// </summary>
        public static void CopyFrom(ref AnimationVector2D target, in AnimationVector2D source) { target._value.x = source._value.x; target._value.y = source._value.y; }
    }

    /// <summary>
    /// 三维动画向量（用Vector3），适用于3D空间动画（如位置、缩放、旋转等）。
    /// </summary>
    public struct AnimationVector3D : AnimationVector
    {
        private bool _isInitialized;
        bool AnimationVector.isInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
            }
        }
        /// <summary>
        /// 动画值（x, y, z）
        /// </summary>
        [SerializeField]
        private Vector3 _value;
        public AnimationVector3D(float x = 0f, float y = 0f, float z = 0f)
        {
            _value = new Vector3(x, y, z);
            _isInitialized = true;
        }
        public AnimationVector3D(Vector3 v)
        {
            _value = v;
            _isInitialized = true;
        }
        public float this[int index]
        {
            get => index == 0 ? _value.x : index == 1 ? _value.y : index == 2 ? _value.z : throw new IndexOutOfRangeException();
            set { if (index == 0) _value.x = value; else if (index == 1) _value.y = value; else if (index == 2) _value.z = value; else throw new IndexOutOfRangeException(); }
        }
        public AnimationVector NewVector() => new AnimationVector3D(0f, 0f, 0f);
        public void Reset() { _value.x = 0f; _value.y = 0f; _value.z = 0f; }

        /// <summary>
        /// 创建全零新实例
        /// </summary>
        public static AnimationVector3D NewInstance() => new AnimationVector3D(0f, 0f, 0f);
        /// <summary>
        /// 深拷贝
        /// </summary>
        public static AnimationVector3D Copy(AnimationVector3D v) => new AnimationVector3D(v._value);
        /// <summary>
        /// 从source拷贝到target
        /// </summary>
        public static void CopyFrom(ref AnimationVector3D target, in AnimationVector3D source) { target._value.x = source._value.x; target._value.y = source._value.y; target._value.z = source._value.z; }
    }

    /// <summary>
    /// 四维动画向量（用Vector4），适用于颜色、四元数等四通道动画。
    /// </summary>
    public struct AnimationVector4D : AnimationVector
    {
        private bool _isInitialized;
        bool AnimationVector.isInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
            }
        }
        /// <summary>
        /// 动画值（x, y, z, w）
        /// </summary>
        [SerializeField]
        private Vector4 _value;

        public AnimationVector4D(float x = 0f, float y = 0f, float z = 0f, float w = 0f)
        {
            _value = new Vector4(x, y, z, w);
            _isInitialized = true;
        }
        public AnimationVector4D(Vector4 v)
        {
            _value = v;
            _isInitialized = true;
        }
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return _value.x;
                    case 1: return _value.y;
                    case 2: return _value.z;
                    case 3: return _value.w;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: _value.x = value; break;
                    case 1: _value.y = value; break;
                    case 2: _value.z = value; break;
                    case 3: _value.w = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }
        public AnimationVector NewVector() => new AnimationVector4D(0f, 0f, 0f, 0f);
        public void Reset() { _value.x = 0f; _value.y = 0f; _value.z = 0f; _value.w = 0f; }

        /// <summary>
        /// 创建全零新实例
        /// </summary>
        public static AnimationVector4D NewInstance() => new AnimationVector4D(0f, 0f, 0f, 0f);
        /// <summary>
        /// 深拷贝
        /// </summary>
        public static AnimationVector4D Copy(AnimationVector4D v) => new AnimationVector4D(v._value);
        /// <summary>
        /// 从source拷贝到target
        /// </summary>
        public static void CopyFrom(ref AnimationVector4D target, in AnimationVector4D source) { target._value.x = source._value.x; target._value.y = source._value.y; target._value.z = source._value.z; target._value.w = source._value.w; }
    }
} 