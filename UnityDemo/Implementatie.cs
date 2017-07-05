namespace UnityDemo
{
   public class Implementatie : IInterface, IAnotherInterface
   {
      public static int ConstructorCounter { get; private set; }
      public static int DisposeCounter { get; private set; }

      public static void ResetCounters()
      {
         ConstructorCounter = 0;
         DisposeCounter = 0;
      }

      public Implementatie()
      {
         ConstructorCounter++;
      }

      public void Dispose()
      {
         DisposeCounter++;
      }
   }
}