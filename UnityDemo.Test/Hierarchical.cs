using Microsoft.Practices.Unity;
using NUnit.Framework;
using Unity;
using Unity.Lifetime;

namespace UnityDemo.Test
{
   [TestFixture]
   public class Hierarchical
   {
      /// <summary>
      /// HierarchicalLifetimeManager
      /// 
      /// Gelijk aan ContainerControlled. Geeft een singleton en dus voor elke aanroep dezelfde instantie.
      /// De container roept netjes de dispose aan wanneer de container wordt ge-disposed
      /// </summary>
      [Test]
      public void HierarchicalLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new HierarchicalLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);
            
            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// HierarchicalLifetimeManager met meerdere interfaces naar hetzelfde type
      /// 
      /// Net als bij ContianerControlled krijg je ook bij de HierarchicalLifetimeManager
      /// dezelfde instantie terug (binnen de container) wanneer je een interface
      /// resolved die dezelfde implementatie heeft
      /// </summary>
      [Test]
      public void HierarchicalLifetimeManager_MultipleInterfaces_SameConcrete()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            container.RegisterType<Implementatie>(new HierarchicalLifetimeManager());

            container.RegisterType<IInterface, Implementatie>();
            container.RegisterType<IAnotherInterface, Implementatie>();

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IAnotherInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// HierarchicalLifetimeManager met childcontainer
      /// 
      /// Het verschil tussen HierarchicalLifetimeManager en ContainerControlledLifetimeManager zitten
      /// hem in het gebruik van een childcointainer. Waar je bij een ContainerControlledLifetimeManager
      /// bij een childcontainer dezelfde instance terug krijgt, krijg je bij de HierarchicalLifetimeManager
      /// een instantie per (child)container terug
      /// </summary>
      [Test]
      public void HierarchicalLifetimeManager_MultipleChildContainer()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new HierarchicalLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var childContianer = container.CreateChildContainer();
            var childContianer2 = container.CreateChildContainer();

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = childContianer.Resolve<IInterface>();
            var implementatieC = childContianer2.Resolve<IInterface>();

            Assert.AreNotSame(implementatieA, implementatieB);
            Assert.AreNotSame(implementatieA, implementatieC);
            Assert.AreEqual(3, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(3, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// HierarchicalLifetimeManager met childcontainer
      /// 
      /// Binnen de childcontainer is de instantie weer een singleton
      /// </summary>
      [Test]
      public void HierarchicalLifetimeManager_ChildContainer()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var manager = new HierarchicalLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(manager);

            var childContianer = container.CreateChildContainer();

            var implementatieA = childContianer.Resolve<IInterface>();
            var implementatieB = childContianer.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }
   }
}
