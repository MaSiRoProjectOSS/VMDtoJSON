﻿namespace MaSiRoProject
{
    namespace Common
    {
        /// <summary>
        /// X軸とY軸をまとめたクラス
        /// </summary>
        public class Rectangle<T>
        {
            /// <summary>
            /// 開始座標
            /// </summary>
            public Coordinate<T> Start = new Coordinate<T>();

            /// <summary>
            /// 終了座標
            /// </summary>
            public Coordinate<T> End = new Coordinate<T>();

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="startX">開始座標のX軸</param>
            /// <param name="startY">開始座標のY軸</param>
            /// <param name="endX">終了座標のX軸</param>
            /// <param name="endY">終了座標のY軸</param>
            public Rectangle(T startX, T startY, T endX, T endY)
            {
                this.Set(startX, startY, endX, endY);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Rectangle()
            {
            }

            /// <summary>
            /// 設定関数
            /// </summary>
            /// <param name="startX">開始座標のX軸</param>
            /// <param name="startY">開始座標のY軸</param>
            /// <param name="endX">終了座標のX軸</param>
            /// <param name="endY">終了座標のY軸</param>
            public void Set(T startX, T startY, T endX, T endY)
            {
                this.Start.X = startX;
                this.Start.Y = startY;
                this.End.X = endX;
                this.End.Y = endY;
            }
        }

        /// <summary>
        /// X軸とY軸をまとめたクラス
        /// </summary>
        public class Coordinate<T>
        {
            /// <summary>
            /// X軸
            /// </summary>
            public T X;

            /// <summary>
            /// Y軸
            /// </summary>
            public T Y;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            public Coordinate(T x, T y)
            {
                this.Set(x, y);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Coordinate()
            {
            }

            /// <summary>
            /// 設定関数
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            public void Set(T x, T y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        /// <summary>
        /// 回転軸をまとめたクラス
        /// </summary>
        public class AxisOfRotation<T>
        {
            /// <summary>
            /// [内部変数] Roll
            /// </summary>
            public T inner_roll;

            /// <summary>
            /// [内部変数]Pitch
            /// </summary>
            public T inner_pitch;

            /// <summary>
            /// [内部変数]Yaw
            /// </summary>
            public T inner_yaw;

            /// <summary>
            /// Roll
            /// </summary>
            public T RollDegree
            {
                get { return this.inner_roll; }
            }

            /// <summary>
            /// Pitch
            /// </summary>
            public T PitchDegree
            {
                get { return this.inner_pitch; }
            }

            /// <summary>
            /// Yaw
            /// </summary>
            public T YawDegree
            {
                get { return this.inner_yaw; }
            }

            /// <summary>
            /// Roll
            /// </summary>
            public T Roll
            {
                get { return this.inner_roll; }

                set
                {
                    this.inner_roll = value;
                }
            }

            /// <summary>
            /// Pitch
            /// </summary>
            public T Pitch
            {
                get { return this.inner_pitch; }

                set
                {
                    this.inner_pitch = value;
                }
            }

            /// <summary>
            /// Yaw
            /// </summary>
            public T Yaw
            {
                get { return this.inner_yaw; }

                set
                {
                    this.inner_yaw = value;
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="roll">Roll</param>
            /// <param name="pitch">Pitch</param>
            /// <param name="yaw">Yaw</param>
            public AxisOfRotation(T roll, T pitch, T yaw)
            {
                this.Set(roll, pitch, yaw);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public AxisOfRotation()
            {
            }

            /// <summary>
            /// 設定関数
            /// </summary>
            /// <param name="roll">Roll</param>
            /// <param name="pitch">Pitch</param>
            /// <param name="yaw">Yaw</param>
            public void Set(T roll, T pitch, T yaw)
            {
                this.inner_roll = roll;
                this.inner_pitch = pitch;
                this.inner_yaw = yaw;
            }
        }

        /// <summary>
        /// X軸とY軸とZ軸をまとめたクラス
        /// </summary>
        public class Position<T>
        {
            /// <summary>
            /// X軸
            /// </summary>
            public T X;

            /// <summary>
            /// Y軸
            /// </summary>
            public T Y;

            /// <summary>
            /// Z軸
            /// </summary>
            public T Z;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            /// <param name="z">Z軸</param>
            public Position(T x, T y, T z)
            {
                this.Set(x, y, z);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Position()
            {
            }

            /// <summary>
            /// 設定関数
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            /// <param name="z">Z軸</param>
            public void Set(T x, T y, T z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        /// <summary>
        /// X軸とY軸とZ軸とW軸をまとめたクラス
        /// </summary>
        public class Quaternion<T>
        {
            /// <summary>
            /// X軸
            /// </summary>
            public T X;

            /// <summary>
            /// Y軸
            /// </summary>
            public T Y;

            /// <summary>
            /// Z軸
            /// </summary>
            public T Z;

            /// <summary>
            /// W軸
            /// </summary>
            public T W;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            /// <param name="z">Z軸</param>
            /// <param name="w">W軸</param>
            public Quaternion(T x, T y, T z, T w)
            {
                this.Set(x, y, z, w);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public Quaternion()
            {
            }

            /// <summary>
            /// 設定関数
            /// </summary>
            /// <param name="x">X軸</param>
            /// <param name="y">Y軸</param>
            /// <param name="z">Z軸</param>
            /// <param name="w">W軸</param>
            public void Set(T x, T y, T z, T w)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.W = w;
            }
        }
    }
}