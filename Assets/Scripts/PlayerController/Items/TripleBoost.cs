namespace Items {
  public class TripleBoost : Item {
    public int remaining = 3;

    public bool ExecuteEffect(PDriverController driver) {
      if (remaining == 0) return false;

      driver.StartBoost();

      remaining--;
      if (remaining < 1) return false;
      return true;
    }
  }
}
