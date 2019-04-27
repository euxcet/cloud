public class Matrix {
	double [,] data;
	int n, m;

	public Matrix(int _n, int _m) {
		n = _n; m = _m;
		data = new double[n, m];
		for(int i = 0; i < n; i++)
			for(int j = 0; j < m; j++)
				data[i, j] = 0;
	}

	public double this[int x, int y] {
		get {
			return m[x, y];
		}
		set {
			m[x, y] = value;
		}
	}

	public int N {
		get {
			return n;
		}
	}

	public int M {
		get {
			return m;
		}
	}

	public static Matrix operator +(Matrix a, Matrix b) {
		if (a.N != b.N || a.M != b.M) {
			return null;
		}
		Matrix c = new Matrix(a.N, a.M);
		for(int i = 0; i < a.N; i++)
			for(int j = 0; j < a.M; j++)
				c[i, j] = a[i, j] + b[i, j];
		return c;
	}

	public static Matrix operator -(Matrix a, Matrix b) {
		if (a.N != b.N || a.M != b.M) {
			return null;
		}
		Matrix c = new Matrix(a.N, a.M);
		for(int i = 0; i < a.N; i++)
			for(int j = 0; j < a.M; j++)
				c[i, j] = a[i, j] + b[i, j];

	}

	public static Matrix operator *(Matrix a, Matrix b) {
		if (a.M != b.N) {
			return null;
		}
		Matrix c = new Matrix(a.N, b.M);
		for(int i = 0; i < a.N; i++)
			for(int j = 0; j < b.M; j++)
				for(int k = 0; k < a.M; k++)
					c[i, j] += a[i, k] * b[k, j];
	}
	
	public static Matrix Solve(Matrix a, Matrix b) {
		if (a.N != b.N || a.N < a.M || b.M != 1) {
			return null;
		}
		Matrix x = new Matrix(a.M, 1);
		Matrix ta = a.T() * a;
		Matrix tb = a.T() * b;

		// A^T * A * x = A^T * b
		// A^T * A (a.M * a.M)
		// A^T * b (a.M * 1)


		for(int row = 0; row < ta.N - 1; row++) {
			float vmax = -1;
			int pos = 0;
			for(int i = row; i < ta.N; i++) {
				if (Math.Abs(ta[i, row]) > vmax) {
					vmax = Math.Abs(ta[i, row]);
					pos = i;
				}
			}
			if (vmax < 0.00001f) {
				return null;
			}
			if (pos != row) {
				for(int i = 0; i < ta.M; i++)
					swap(ta[row, i], ta[pos, i]);
				swap(tb[row, 0], tb[pos, 0]);
			}
			for(int i = row + 1; i < ta.N; i++) {
				for(int j = row; j < ta.M; j++)
					ta[i, j] -= ta[i, row] / ta[row, row] * ta[row, j];
				tb[i, 0] -= ta[i, row] / ta[row, row] * tb[row, 0];
			}
		}
		for(int row = ta.N - 1; row >= 0; row--) {
			x[row, 0] = tb[row, 0];
			for(int i = row + 1; i < ta.M; i++) {
				x[row, 0] -= ta[row, i] * x[i, 0];
			}
			x[row, 0] /= ta[row, row];
		}
		return x;
	}
}
