using Microsoft.Practices.Unity;
using NUnit.Framework;
using Unity;
using Unity.Lifetime;

namespace UnityDemo.Test
{
   [TestFixture]
   public class ExternallyControlled
   {
      /// <summary>
      /// ExternallyControlledLifetimeManager
      /// 
      /// Geeft een singleton die niet voor je wordt opgeruimd. 
      /// </summary>
      [Test]
      public void ExternallyControlledLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new ExternallyControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(0, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// ExternallyControlledLifetimeManager
      /// 
      /// Ook binnen de childcontainer krijg je dezelfde instantie als in de rootcontainer.
      /// </summary>
      [Test]
      public void ExternallyControlledLifetimeManager_ChildContainer()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new ExternallyControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var childContainer = container.CreateChildContainer();

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = childContainer.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(0, Implementatie.DisposeCounter);
      }
   }
}
