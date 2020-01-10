public class Math {
  public static int mod(int x, int m) {
    return (x%m + m)%m;
  }

  public static int abs(int x) {
    return x < 0 ? -x : x;
  }

  public static bool withinMod(int x, int v, int t, int m) {
    int difference0 = mod(x, m) - mod(v, m);
    int difference1 = mod(x, m) + m - mod(v, m);
    int difference2 = mod(x, m) - m - mod(v,m);
    int difference = min(abs(difference0), abs(difference1), abs(difference2));
    return difference < t;
  }

  public static int max(params int[] list) {
    int largest = list[0];
    for (int i = 1; i < list.Length; i++) {
      if (list[i] > largest) largest = list[i];
    }
    return largest;
  }
  public static int min(params int[] list) {
    int smallest = list[0];
    for (int i = 1; i < list.Length; i++) {
      if (list[i] < smallest) smallest = list[i];
    }
    return smallest;
  }
}
