using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Temp
{
    public class PointInt
    {
        public long X;

        public long Y;

        public PointInt(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }

        public PointInt(PointInt head)
            : this(head.X, head.Y)
        {
        }

        public static PointInt operator +(PointInt a, PointInt b)
        {
            return new PointInt(a.X + b.X, a.Y + b.Y);
        }

        public static PointInt operator -(PointInt a, PointInt b)
        {
            return new PointInt(a.X - b.X, a.Y - b.Y);
        }

        public static PointInt operator *(PointInt a, long k)
        {
            return new PointInt(k * a.X, k * a.Y);
        }

        public static PointInt operator *(long k, PointInt a)
        {
            return new PointInt(k * a.X, k * a.Y);
        }

        public bool IsInsideRectangle(long l, long b, long r, long t)
        {
            return (l <= X) && (X <= r) && (b <= Y) && (Y <= t);
        }

        public bool Equals(PointInt other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.X == this.X && other.Y == this.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PointInt))
            {
                return false;
            }
            return Equals((PointInt)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.X.GetHashCode() * 397) ^ this.Y.GetHashCode();
            }
        }
    }

    public class Point2DReal
    {
        public double X;

        public double Y;

        public Point2DReal(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point2DReal(Point2DReal head)
            : this(head.X, head.Y)
        {
        }

        public static Point2DReal operator +(Point2DReal a, Point2DReal b)
        {
            return new Point2DReal(a.X + b.X, a.Y + b.Y);
        }

        public static Point2DReal operator -(Point2DReal a, Point2DReal b)
        {
            return new Point2DReal(a.X - b.X, a.Y - b.Y);
        }

        public static Point2DReal operator *(Point2DReal a, double k)
        {
            return new Point2DReal(k * a.X, k * a.Y);
        }

        public static Point2DReal operator *(double k, Point2DReal a)
        {
            return new Point2DReal(k * a.X, k * a.Y);
        }

        public double Dist(Point2DReal p)
        {
            return Math.Sqrt((p.X - X)*(p.X - X) + (p.Y - Y)*(p.Y - Y));
        }

        public bool IsInsideRectangle(double l, double b, double r, double t)
        {
            return (l <= X) && (X <= r) && (b <= Y) && (Y <= t);
        }
    }

    internal class LineInt
    {
        public LineInt(PointInt a, PointInt b)
        {
            A = a.Y - b.Y;
            B = b.X - a.X;
            C = a.X * b.Y - a.Y * b.X;
        }

        public long A, B, C;

        public bool ContainsPoint(PointInt p)
        {
            return A * p.X + B * p.Y + C == 0;
        }

        public int Sign(PointInt p)
        {
            return Math.Sign(A * p.X + B * p.Y + C);
        }
    }

    internal static class Geometry
    {
        public static long VectInt(PointInt a, PointInt b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static long VectInt(PointInt a, PointInt b, PointInt c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }
    }

    internal class MatrixInt
    {
        private readonly long[,] m_Matrix;

        public int Size
        {
            get
            {
                return m_Matrix.GetLength(0);
            }
        }

        public long Mod { get; private set; }

        public MatrixInt(int size, long mod = 0)
        {
            m_Matrix = new long[size,size];
            Mod = mod;
        }

        public MatrixInt(long[,] matrix, long mod = 0)
        {
            var size = matrix.GetLength(0);
            m_Matrix = new long[size,size];
            Array.Copy(matrix, m_Matrix, size * size);
            Mod = mod;

            if (mod != 0)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        m_Matrix[i, j] %= mod;
                    }
                }
            }
        }

        public static MatrixInt IdentityMatrix(int size, long mod = 0)
        {
            long[,] matrix = new long[size,size];

            for (int i = 0; i < size; i++)
            {
                matrix[i, i] = 1;
            }

            return new MatrixInt(matrix, mod);
        }

        public long this[int i, int j]
        {
            get
            {
                return m_Matrix[i, j];
            }

            set
            {
                m_Matrix[i, j] = value;
            }
        }

        public static MatrixInt operator +(MatrixInt a, MatrixInt b)
        {
            int n = a.Size;
            long mod = Math.Max(a.Mod, b.Mod);
            long[,] c = new long[n,n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    c[i, j] = a[i, j] + b[i, j];
                }
            }

            if (mod > 0)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        c[i, j] %= mod;
                    }
                }
            }

            return new MatrixInt(c, mod);
        }

        public static MatrixInt operator *(MatrixInt a, MatrixInt b)
        {
            int n = a.Size;
            long mod = Math.Max(a.Mod, b.Mod);

            long[,] c = new long[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        c[i, j] += a[i, k] * b[k, j];
                        if (mod > 0)
                        {
                            c[i, j] %= mod;
                        }
                    }
                }
            }

            return new MatrixInt(c, mod);
        }

        public void Print()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write("{0} ", m_Matrix[i, j]);
                }
                Console.WriteLine();
            }
        }
    }

    public static class Permutations
    {
        private static readonly Random m_Random;

        static Permutations()
        {
            m_Random = new Random();
        }

        public static int[] GetRandomPermutation(int n)
        {
            int[] p = new int[n];
            for (int i = 0; i < n; i++)
            {
                p[i] = i;
            }

            for (int i = n - 1; i > 0; i--)
            {
                int j = m_Random.Next(i + 1);
                int tmp = p[i];
                p[i] = p[j];
                p[j] = tmp;
            }

            return p;
        }

        /*public static T[] Shuffle<T>(this T[] array)
        {
            int length = array.Length;
            int[] p = GetRandomPermutation(length);
            T[] result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = array[p[i]];
            }

            return result;
        }*/

        /*public static T[] ShuffleSort<T>(this T[] array)
        {
            var result = array.Shuffle();
            Array.Sort(result);
            return result;
        }*/

        public static void Shuffle<T>(T[] array)
        {
            var n = array.Count();
            for (int i = n - 1; i > 0; i--)
            {
                int j = m_Random.Next(i + 1);
                T tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }            
        }

        public static void ShuffleSort<T>(T[] array)
        {
            Shuffle(array);
            Array.Sort(array);
        }

        public static int[] Next(int[] p)
        {
            int n = p.Length;
            var next = new int[n];
            Array.Copy(p, next, n);

            int k = -1;
            for (int i = n - 1; i > 0; i--)
            {
                if (next[i - 1] < next[i])
                {
                    k = i - 1;
                    break;
                }
            }
            if (k == -1)
            {
                return null;
            }
            for (int i = n - 1; i >= 0; i--)
            {
                if (next[i] > next[k])
                {
                    var tmp = next[i];
                    next[i] = next[k];
                    next[k] = tmp;
                    break;
                }
            }
            for (int i = 1; i <= (n - k - 1) / 2; i++)
            {
                var tmp = next[k + i];
                next[k + i] = next[n - i];
                next[n - i] = tmp;
            }

            return next;
        }
    }

    internal static class Algebra
    {
        public static long Phi(long n)
        {
            long result = n;
            for (long i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    while (n % i == 0)
                    {
                        n /= i;
                    }

                    result -= result / i;
                }
            }

            if (n > 1)
            {
                result -= result / n;
            }

            return result;
        }

        public static long BinPower(long a, long n, long mod)
        {
            long result = 1;

            while (n > 0)
            {
                if ((n & 1) != 0)
                {
                    result = (result * a) % mod;
                }

                a = (a * a) % mod;
                n >>= 1;
            }

            return result;
        }

        public static MatrixInt MatrixBinPower(MatrixInt a, long n)
        {
            MatrixInt result = MatrixInt.IdentityMatrix(a.Size, a.Mod);

            while (n > 0)
            {
                if ((n & 1) != 0)
                {
                    result *= a;
                }

                a *= a;
                n >>= 1;
            }

            return result;
        }

        public static long Gcd(long a, long b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }

        public static long ExtendedGcd(long a, long b, out long x, out long y)
        {
            if (b == 0)
            {
                x = 1;
                y = 0;
                return a;
            }

            long x1;
            long y1;
            long d = ExtendedGcd(b, a % b, out x1, out y1);
            x = y1;
            y = x1 - (a / b) * y1;
            return d;
        }

        public static long Lcm(long a, long b)
        {
            return (a / Gcd(a, b)) * b;
        }

        public static bool[] GetPrimes(int n)
        {
            n = Math.Max(n, 2);
            bool[] prime = new bool[n + 1];
            for (int i = 2; i <= n; i++)
            {
                prime[i] = true;
            }

            for (int i = 2; i * i <= n; i++)
            {
                if (prime[i])
                {
                    if ((long)i * i <= n)
                    {
                        for (int j = i * i; j <= n; j += i)
                        {
                            prime[j] = false;
                        }
                    }
                }
            }

            return prime;
        }

        public static long GetFibonacciNumber(long n, long mod = 0)
        {
            long[,] matrix = new long[,] { { 0, 1 }, { 1, 1 } };

            MatrixInt result = MatrixBinPower(new MatrixInt(matrix, mod), n);

            return result[1, 1];
        }

        public static long[] GetFibonacciSequence(int n)
        {
            long[] result = new long[n];
            result[0] = result[1] = 1;

            for (int i = 2; i < n; i++)
            {
                result[i] = result[i - 1] + result[i - 2];
            }

            return result;
        }

        public static long GetInverseElement(long a, long mod)
        {
            long x, y;
            long g = ExtendedGcd(a, mod, out x, out y);

            if (g != 1)
            {
                return -1;
            }

            return ((x % mod) + mod) % mod;
        }

        public static long[] GetAllInverseElements(long n, long mod)
        {
            long[] result = new long[n];
            result[1] = 1;
            for (int i = 2; i < n; i++)
            {
                result[i] = (mod - (((mod / i) * result[mod % i]) % mod)) % mod;
            }

            return result;
        }

        public static int GetMinimalPrimeDivisor(int n)
        {
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0)
                {
                    return i;
                }
            }

            return n;
        }

        public static long Sqr(long x)
        {
            return x * x;
        }

        public static int SumOfDigits(long x, long baseMod = 10)
        {
            int res = 0;
            while (x > 0)
            {
                res += (int)(x % baseMod);
                x = x / baseMod;
            }
            return res;
        }
    }

    internal static class Reader
    {
        public static void Read<T>(out T v1)
        {
            var values = new T[1];
            Read(values);
            v1 = values[0];
        }

        public static void Read<T>(out T v1, out T v2)
        {
            var values = new T[2];
            Read(values);
            v1 = values[0];
            v2 = values[1];
        }

        public static void Read<T>(out T v1, out T v2, out T v3)
        {
            var values = new T[3];
            Read(values);
            v1 = values[0];
            v2 = values[1];
            v3 = values[2];
        }

        public static void Read<T>(out T v1, out T v2, out T v3, out T v4)
        {
            var values = new T[4];
            Read(values);
            v1 = values[0];
            v2 = values[1];
            v3 = values[2];
            v4 = values[3];
        }

        public static void Read<T>(out T v1, out T v2, out T v3, out T v4, out T v5)
        {
            var values = new T[5];
            Read(values);
            v1 = values[0];
            v2 = values[1];
            v3 = values[2];
            v4 = values[3];
            v5 = values[4];
        }

        public static void Read<T>(T[] values)
        {
            Read(values, values.Length);
        }

        public static void Read<T>(T[] values, int count)
        {
// ReSharper disable PossibleNullReferenceException
            var list = Console.ReadLine().Split();
// ReSharper restore PossibleNullReferenceException

            count = Math.Min(count, list.Length);

            var converter = TypeDescriptor.GetConverter(typeof(T));

            for (int i = 0; i < count; i++)
            {
                values[i] = (T)converter.ConvertFromString(list[i]);
            }  
        }

        public static int[] ReadDigits()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return Console.ReadLine().Select(x => int.Parse(x.ToString(CultureInfo.InvariantCulture))).ToArray();
            // ReSharper restore AssignNullToNotNullAttribute
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }
    }

    public interface IGraph
    {
        bool IsOriented { get; set; }

        int Vertices { get; set; }

        IList<int> this[int i] { get; }

        void AddEdge(int u, int v);

        void AddOrientedEdge(int u, int v);

        void AddNotOrientedEdge(int u, int v);
    }

    public class ListGraph : IGraph
    {
        private readonly List<int>[] m_Edges;

        public int Vertices { get; set; }

        public bool IsOriented { get; set; }

        public IList<int> this[int i]
        {
            get
            {
                return this.m_Edges[i];
            }
        }

        public ListGraph(int vertices, bool isOriented = false)
        {
            this.Vertices = vertices;
            this.IsOriented = isOriented;

            this.m_Edges = new List<int>[vertices];

            for (int i = 0; i < vertices; i++)
            {
                this.m_Edges[i] = new List<int>();
            }
        }

        public void AddEdge(int u, int v)
        {
            this.AddOrientedEdge(u, v);
            if (!IsOriented)
            {
                this.AddOrientedEdge(v, u);
            }
        }

        public void AddNotOrientedEdge(int u, int v)
        {
            this.AddOrientedEdge(u, v);
            this.AddOrientedEdge(v, u);
        }

        public void AddOrientedEdge(int first, int second)
        {
            this.m_Edges[first].Add(second);
        }

        public int[] Bfs(int start)
        {
            int[] d = new int[Vertices];
            for (int i = 0; i < Vertices; i++)
            {
                d[i] = -1;
            }

            Queue<int> queue = new Queue<int>();
            queue.Enqueue(start);
            d[start] = 0;

            while (queue.Count > 0)
            {
                int v = queue.Dequeue();
                foreach (int t in this.m_Edges[v].Where(t => d[t] == -1))
                {
                    queue.Enqueue(t);
                    d[t] = d[v] + 1;
                }
            }

            return d;
        }
    }

    internal class SimpleSumTable
    {
        private readonly int[,] m_Sum;

        public SimpleSumTable(int n, int m, int[,] table)
        {
            m_Sum = new int[n + 1,m + 1];

            for (int i = 1; i < n + 1; i++)
            {
                for (int j = 1; j < m + 1; j++)
                {
                    m_Sum[i, j] = m_Sum[i, j - 1] + m_Sum[i - 1, j] - m_Sum[i - 1, j - 1] + table[i - 1, j - 1];
                }
            }
        }

        public int GetSum(int l, int b, int r, int t)
        {
            return m_Sum[r + 1, t + 1] - m_Sum[r + 1, b] - m_Sum[l, t + 1] + m_Sum[l, b];
        }
    }

    internal class SegmentTreeSimpleInt<T>
    {
        public int Size { get; private set; }

        private readonly T[] m_Tree;

        private readonly Func<T, T, T> m_Operation;

        private readonly T m_Null;

        public SegmentTreeSimpleInt(int size, Func<T, T, T> operation, T nullElement, IList<T> array = null)
        {
            this.Size = size;
            this.m_Operation = operation;
            this.m_Null = nullElement;

            m_Tree = new T[4 * size];
            if (array != null)
            {
                this.Build(array, 1, 0, size - 1);
            }
        }

        private void Build(IList<T> array, int v, int tl, int tr)
        {
            if (tl == tr)
            {
                m_Tree[v] = array[tl];
            }
            else
            {
                int tm = (tl + tr) / 2;
                this.Build(array, 2 * v, tl, tm);
                this.Build(array, 2 * v + 1, tm + 1, tr);
                this.CalculateNode(v);
            }
        }

        public T GetSum(int l, int r)
        {
            return GetSum(1, 0, Size - 1, l, r);
        }

        private T GetSum(int v, int tl, int tr, int l, int r)
        {
            if (l > r)
            {
                return m_Null;
            }

            if (l == tl && r == tr)
            {
                return m_Tree[v];
            }

            int tm = (tl + tr) / 2;

            return this.m_Operation(
                GetSum(2 * v, tl, tm, l, Math.Min(r, tm)), GetSum(2 * v + 1, tm + 1, tr, Math.Max(l, tm + 1), r));
        }

        public void Update(int pos, T newValue)
        {
            Update(1, 0, Size - 1, pos, newValue);
        }

        private void Update(int v, int tl, int tr, int pos, T newValue)
        {
            if (tl == tr)
            {
                m_Tree[v] = newValue;
            }
            else
            {
                int tm = (tl + tr) / 2;
                if (pos <= tm)
                {
                    Update(2 * v, tl, tm, pos, newValue);
                }
                else
                {
                    Update(2 * v + 1, tm + 1, tr, pos, newValue);
                }
                this.CalculateNode(v);
            }
        }

        private void CalculateNode(int v)
        {
            m_Tree[v] = this.m_Operation(m_Tree[2 * v], m_Tree[2 * v + 1]);
        }
    }

    internal class Pair<TFirst, TSecond>
    {
        public Pair(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        public TFirst First { set; get; }

        public TSecond Second { set; get; }

        protected bool Equals(Pair<TFirst, TSecond> other)
        {
            return EqualityComparer<TFirst>.Default.Equals(this.First, other.First) && EqualityComparer<TSecond>.Default.Equals(this.Second, other.Second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Pair<TFirst, TSecond>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TFirst>.Default.GetHashCode(this.First) * 397) ^ EqualityComparer<TSecond>.Default.GetHashCode(this.Second);
            }
        }
    }

    internal class FenwickTreeInt64
    {
        public FenwickTreeInt64(int size)
        {
            this.m_Size = size;
            m_Tree = new long[size];
        }

        public FenwickTreeInt64(int size, IList<long> tree)
            : this(size)
        {
            for (int i = 0; i < size; i++)
            {
                Inc(i, tree[i]);
            }
        }

        public long Sum(int r)
        {
            long res = 0;
            for (; r >= 0; r = (r & (r + 1)) - 1)
            {
                res += m_Tree[r];
            }
            return res;
        }

        public long Sum(int l, int r)
        {
            return Sum(r) - Sum(l - 1);
        }

        public void Inc(int i, long x)
        {
            for (; i < m_Size; i = i | (i + 1))
            {
                m_Tree[i] += x;
            }
        }

        public void Set(int i, long x)
        {
            Inc(i, x - Sum(i, i));
        }

        private readonly int m_Size;

        private readonly long[] m_Tree;
    }

    internal class AccumulativeDictionary<TKey> : Dictionary<TKey, int>
    {
        public new void Add(TKey key, int value = 1)
        {
            if (this.ContainsKey(key))
            {
                base[key] += value;
            }
            else
            {
                base.Add(key, value);
            }
        }

        public new int this[TKey key]
        {
            get
            {
                return this.ContainsKey(key) ? base[key] : 0;
            }
            set
            {
                this.Add(key, value);
            }
        }
    }

    public class PriorityQueue<T>
    {
        public PriorityQueue(Comparison<T> comparison = null)
        {
            if (comparison == null)
            {
                if (typeof(T).GetInterfaces().Any(i => i == typeof(IComparable<T>)))
                {
                    m_Comparison = (a, b) => ((IComparable<T>)a).CompareTo(b);
                }
                else
                {
                    throw new ApplicationException("Add comparer");
                }
            }
            else
            {
                m_Comparison = comparison;
            }
        }

        public int Count { get; private set; }

        public void Enqueue(T item)
        {
            m_List.Add(item);
            m_Indexes.Add(item, this.Count);
            this.Count++;
            Up(this.Count);
        }

        public T Peek()
        {
            return m_List[0];
        }

        public T Dequeue()
        {
            if (this.Count > 0)
            {
                var result = m_List[0];

                Swap(0, this.Count - 1);
                m_Indexes.Remove(m_List[this.Count - 1]);
                m_List.RemoveAt(this.Count - 1);
                this.Count--;
                this.Down(1);

                return result;
            }
            throw new ApplicationException("Couldn't get element from empty queue");
        }

        public void Update(T item)
        {
            int index = m_Indexes[item];
            this.Up(index + 1);
        }

        private readonly List<T> m_List = new List<T>();

        private readonly Dictionary<T, int> m_Indexes = new Dictionary<T, int>();

        private readonly Comparison<T> m_Comparison;

        private void Up(int index)
        {
            while (index > 1 && m_Comparison.Invoke(m_List[index - 1], m_List[index / 2 - 1]) > 0)
            {
                this.Swap(index - 1, index / 2 - 1);

                index = index / 2;
            }
        }

        private void Down(int index)
        {
            while (2 * index <= this.Count && m_Comparison.Invoke(m_List[index - 1], m_List[2 * index - 1]) < 0
                   || 2 * index + 1 <= this.Count && m_Comparison.Invoke(m_List[index - 1], m_List[2 * index]) < 0)
            {
                if (2 * index + 1 > this.Count || m_Comparison.Invoke(m_List[2 * index - 1], m_List[2 * index]) > 0)
                {
                    this.Swap(index - 1, 2 * index - 1);
                    index = 2 * index;
                }
                else
                {
                    this.Swap(index - 1, 2 * index);
                    index = 2 * index + 1;
                }
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = m_List[i];
            m_List[i] = m_List[j];
            m_List[j] = tmp;

            m_Indexes[m_List[i]] = i;
            m_Indexes[m_List[j]] = j;
        }
    }

    public class DisjointSetUnion<T>
    {
        public DisjointSetUnion()
        {
            m_Parent = new Dictionary<T, T>();
            m_Rank = new Dictionary<T, int>();
        }

        public DisjointSetUnion(DisjointSetUnion<T> set)
        {
            m_Parent = new Dictionary<T, T>(set.m_Parent);
            m_Rank = new Dictionary<T, int>(set.m_Rank);
        }

        private readonly Dictionary<T, T> m_Parent;
        private readonly Dictionary<T, int> m_Rank;

        public int GetRank(T x)
        {
            return m_Rank[x];
        }

        public void MakeSet(T x)
        {
            m_Parent[x] = x;
            this.m_Rank[x] = 0;
        }

        public void UnionSets(T x, T y)
        {
            x = this.FindSet(x);
            y = this.FindSet(y);
            if (!x.Equals(y))
            {
                if (m_Rank[x] < m_Rank[y])
                {
                    T t = x;
                    x = y;
                    y = t;
                }
                m_Parent[y] = x;
                if (m_Rank[x] == m_Rank[y])
                {
                    m_Rank[x]++;
                }
            }
        }

        public T FindSet(T x)
        {
            if (x.Equals(m_Parent[x]))
            {
                return x;
            }
            return m_Parent[x] = this.FindSet(m_Parent[x]);
        }
    }

    internal class HamiltonianPathFinder
    {
        public static void Run()
        {
            int n, m;
            Reader.Read(out n, out m);
            List<int>[] a = new List<int>[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = new List<int>();
            }
            for (int i = 0; i < m; i++)
            {
                int x, y;
                Reader.Read(out x, out y);
                a[x].Add(y);
                a[y].Add(x);
            }

            var rnd = new Random();

            bool[] v = new bool[n];
            int[] p = new int[n];
            for (int i = 0; i < n; i++)
            {
                p[i] = -1;
            }
            int first = rnd.Next(n);
            int cur = first;
            v[cur] = true;

            int count = 1;

            while (true)
            {
                var unb = a[cur].Where(u => !v[u]).ToList();
                int d = unb.Count;
                if (d > 0)
                {
                    int next = unb[rnd.Next(d)];
                    v[next] = true;
                    p[cur] = next;
                    cur = next;
                    count++;
                }
                else
                {
                    if (count  == n && a[cur].Contains(first))
                    {
                        p[cur] = first;
                        break;
                    }

                    d = a[cur].Count;
                    int pivot;
                    do
                    {
                        pivot = a[cur][rnd.Next(d)];
                    }
                    while (p[pivot] == cur);

                    int next = p[pivot];

                    int x = next;
                    int y = -1;
                    while (true)
                    {
                        int tmp = p[x];
                        p[x] = y;
                        y = x;
                        x = tmp;
                        if (y == cur)
                        {
                            break;
                        }
                    }
                    p[pivot] = cur;
                    cur = next;
                }
            }

            cur = first;
            do
            {
                Console.Write("{0} ", cur);
                cur = p[cur];
            }
            while (cur != first);
        }

        public static void WriteTest(int n)
        {
            Console.WriteLine("{0} {1}", 2 * n, 2 * (n - 1) + n);
            for (int i = 0; i < n - 1; i++)
            {
                Console.WriteLine("{0} {1}", 2 * i, 2 * i + 2);
                Console.WriteLine("{0} {1}", 2 * i + 1, 2 * i + 3);
                //Console.WriteLine("{0} {1}", 2 * i + 1, 2 * i + 2);
                //Console.WriteLine("{0} {1}", 2 * i, 2 * i + 3);
            }
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("{0} {1}", 2 * i, 2 * i + 1);
            }
        }
    }

    internal class Backtrack<T> where T : class 
    {
        public Backtrack(Func<T, IEnumerable<T>> generator)
        {
            this.m_Generator = generator;
        }

        public Dictionary<T, T> Generate(T startState)
        {
            var result = new Dictionary<T, T>();
            result.Add(startState, null);

            var queue = new Queue<T>();
            queue.Enqueue(startState);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var next = m_Generator(current);
                foreach (var state in next)
                {
                    if (!result.ContainsKey(state))
                    {
                        result[state] = current;
                        queue.Enqueue(state);
                    }
                }
            }

            return result;
        }

        private Func<T, IEnumerable<T>> m_Generator;
    }

    public static class Utility
    {
        public static readonly int[] sx = new[] { 1, 0, -1, 0 };
        public static readonly int[] sy = new[] { 0, 1, 0, -1 };

        public static PointInt[] GenerateNeighbors(long x, long y)
        {
            var result = new PointInt[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = new PointInt(x + sx[i], y + sy[i]);
            }
            return result;
        }

        public static PointInt[] GenerateNeighbors(this PointInt p)
        {
            return GenerateNeighbors(p.X, p.Y);
        }

        public static List<PointInt> GenerateNeighborsWithBounds(long x, long y, int n, int m)
        {
            var result = new List<PointInt>(4);
            for (int i = 0; i < 4; i++)
            {
                var nx = x + sx[i];
                var ny = y + sy[i];
                if (0 <= nx && nx < n && 0 <= ny && ny < m)
                {
                    result.Add(new PointInt(nx, ny));
                }
            }
            return result;
        }

        public static List<PointInt> GenerateNeighborsWithBounds(this PointInt p, int n, int m)
        {
            return GenerateNeighborsWithBounds(p.X, p.Y, n, m);
        }
    }

    internal class Program
    {
        private static StreamReader m_InputStream;

        private static StreamWriter m_OutStream;

        private static void OpenFiles()
        {
            m_InputStream = new StreamReader("input.txt"); //File.OpenText("input.txt");
            Console.SetIn(m_InputStream);

            m_OutStream = new StreamWriter("output.txt"); //File.CreateText("output.txt");
            Console.SetOut(m_OutStream);
        }

        private static void CloseFiles()
        {
            m_OutStream.Flush();

            m_InputStream.Dispose();
            m_OutStream.Dispose();
        }

        private static void Main()
        {
            //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            OpenFiles();

            var mainThread = new Thread(() => new Solution().Solve(), 50 * 1024 * 1024);
            mainThread.Start();
            mainThread.Join();

            CloseFiles();
        }
    }

    internal class Solution
    {
        public void Solve()
        {

        }
    }
}
