using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityDemo.Test
{
   [TestFixture]
   public class Transient
   {
      // De defaultLifetimeManager
      // Wanneer een interface wordt geregistreerd zonder interface is Transient de default
      [Test]
      public void DefaultLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            container.RegisterType<IInterface, Implementatie>();

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreNotSame(implementatieA, implementatieB);
            Assert.AreEqual(2, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(0, Implementatie.DisposeCounter);
      }

      // TransientLifetimeManager
      // Zelfde demo als hierboven maar dan met TransientLifetimeManager. Nieuwe instantie wordt gemaakt
      // voor elke resolve. Deze worden niet ge-disposed wanneer de container wordt gedisposed
      [Test]
      public void TransientLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new TransientLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreNotSame(implementatieA, implementatieB);
            Assert.AreEqual(2, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(0, Implementatie.DisposeCounter);
      }
   }
}
