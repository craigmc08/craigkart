namespace Items {
  public interface Item {
    // Return false if the item is used up completely
    bool ExecuteEffect(PDriverController driver);
  }
}
