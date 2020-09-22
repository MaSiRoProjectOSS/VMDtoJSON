namespace MaSiRoProject.Common
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
        /// <param name="start_x">開始座標のX軸</param>
        /// <param name="start_y">開始座標のY軸</param>
        /// <param name="end_x">終了座標のX軸</param>
        /// <param name="end_y">終了座標のY軸</param>
        public Rectangle(T start_x, T start_y, T end_x, T end_y)
        {
            this.Set(start_x, start_y, end_x, end_y);
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
        /// <param name="start_x">開始座標のX軸</param>
        /// <param name="start_y">開始座標のY軸</param>
        /// <param name="end_x">終了座標のX軸</param>
        /// <param name="end_y">終了座標のY軸</param>
        public void Set(T start_x, T start_y, T end_x, T end_y)
        {
            this.Start.X = start_x;
            this.Start.Y = start_y;
            this.End.X = end_x;
            this.End.Y = end_y;
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
        /// <param name="z">Z軸（デフォルト:0）</param>
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