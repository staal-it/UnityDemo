
using NUnit.Framework;
using Unity;
using Unity.Exceptions;
using Unity.Lifetime;

namespace UnityDemo.Test
{
   [TestFixture]
   public class ContainerControlled
   {
      /// <summary>
      /// ContainerControlledLifetimeManager
      /// 
      /// Met ContainerControlledLifetimeManager maakt je een singleton aan welke door de container
      /// wordt ge-disposed
      /// </summary>
      [Test]
      public void ContainerControlledLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new ContainerControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// ContainerControlledLifetimeManager met childcontroller
      /// 
      /// De singleton werkt bij ContainerControlledLifetimeManager over container heen.
      /// Ook vanuit een childcontainer krijgen we dus dezelfde instantie terug
      /// als uit de rootcontainer
      /// </summary>
      [Test]
      public void ContainerControlledLifetimeManager_ChildContainer()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new ContainerControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var childContianer = container.CreateChildContainer();

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = childContianer.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// ContainerControlledLifetimeManager met meerdere namen
      /// 
      /// Je kunt dezelfde interface meerdere malen registreren bij behulp van een naam.
      /// Bij een resolve krijg je dan een singleton die uniek is per naam.
      /// </summary>
      [Test]
      public void ContainerControlledLifetimeManagerWithKey()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager1 = new ContainerControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>("1", manager1);

            var manager2 = new ContainerControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>("2", manager2);

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<IInterface>());

            var implementatie1A = container.Resolve<IInterface>("1");
            var implementatie1B = container.Resolve<IInterface>("1");
            var implementatie2A = container.Resolve<IInterface>("2");
            var implementatie2B = container.Resolve<IInterface>("2");

            Assert.AreSame(implementatie1A, implementatie1B);
            Assert.AreSame(implementatie2A, implementatie2B);
            Assert.AreNotSame(implementatie1A, implementatie2A);
            Assert.AreEqual(2, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(2, Implementatie.DisposeCounter);
      }
   }
}
