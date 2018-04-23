
using NUnit.Framework;
using Unity;
using Unity.Exceptions;
using Unity.Lifetime;

namespace UnityDemo.Test
{
   [TestFixture]
   public class RegisterTypeRegisterInstance
   {
      /// <summary>
      /// ResolveType zonder registratie
      /// 
      /// Wanneer je een concrete class probeert te resolven zonder deze eerst te hebben geregistreerd
      /// dan maakt unity deze voor je aan met als lifetime Transient (bij elke resolve een nieuwe instantie,
      /// dispose wordt niet voor je aangeroepen)
      /// </summary>
      [Test]
      public void ResolveType_WithoutRegistration()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var implementatieA = container.Resolve<Implementatie>();
            var implementatieB = container.Resolve<Implementatie>();

            Assert.AreNotSame(implementatieA, implementatieB);
            Assert.AreEqual(2, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(0, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// RegisterInstance
      /// 
      /// In plaats van het registreren van een interface naar een type kun je ook een zelf 
      /// geinstantieerde instantie registreren. Deze wordt dan behandeld als singleton
      /// </summary>
      [Test]
      public void RegisterInstance()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var implementatie = new Implementatie();
            container.RegisterInstance<IInterface>(implementatie);

            var implementatieA = container.Resolve<IInterface>();
            var implementatieB = container.Resolve<IInterface>();

            Assert.AreSame(implementatieA, implementatieB);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// RegisterType met meerdere Interfaces
      /// 
      /// Wanneer je een registratie vergeet krijg je een exception. Ook wanneer je het concrete
      /// type hebt geregistreerd bij een andere interface die deze ook implementeerd.
      /// </summary>
      [Test]
      public void RegisterType_ForMultipleInterfaces()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var implementatieManager = new ContainerControlledLifetimeManager();
            container.RegisterType<IInterface, Implementatie>(implementatieManager);

            var implementatie = container.Resolve<IInterface>();

            Assert.Throws<ResolutionFailedException>(() => container.Resolve<IAnotherInterface>());

            Assert.NotNull(implementatie);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// RegisterInstance meerdere maken dezelfde instantie
      /// 
      /// Je kunt een instantie meerdere malen registreren bij meerdere interfaces. Instantie wordt
      /// dan meerdere malen ge-disposed
      /// </summary>
      [Test]
      public void RegisterInstanceMultipleTimes()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var implementatie = new Implementatie();
            container.RegisterInstance<IInterface>(implementatie);
            container.RegisterInstance<IAnotherInterface>(implementatie);

            var implementatieA = container.Resolve<IInterface>();
            var anotherimplementatieA = container.Resolve<IAnotherInterface>();

            Assert.AreSame(implementatieA, anotherimplementatieA);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(2, Implementatie.DisposeCounter);
      }

      /// <summary>
      /// RegisterInstance meerdere malen bij een andere interface met een andere lifetime
      /// 
      /// Wanneer we een instantie meerdere malen bij een andere interface met een andere lifetime
      /// registreren krijgen we hetzelfde object terug maar wordt deze maar eenmaal ge-disposed (vanwege
      /// de ExternallyControlledLifetimeManager bij de tweede registratie)
      /// </summary>
      [Test]
      public void RegisterInstance_MultipleTimes_WithExternallyControlledLifetimeManager()
      {
         Implementatie.ResetCounters();

         using (var container = new UnityContainer())
         {
            var implementatie = new Implementatie();

            var containerControlledLifetimeManager = new ContainerControlledLifetimeManager();
            container.RegisterInstance<IInterface>(implementatie, containerControlledLifetimeManager);

            var externallyControlledLifetimeManager = new ExternallyControlledLifetimeManager();
            container.RegisterInstance<IAnotherInterface>(implementatie, externallyControlledLifetimeManager);

            var implementatieA = container.Resolve<IInterface>();
            var anotherimplementatieA = container.Resolve<IAnotherInterface>();

            Assert.AreSame(implementatieA, anotherimplementatieA);
            Assert.AreEqual(1, Implementatie.ConstructorCounter);
         }

         Assert.AreEqual(1, Implementatie.DisposeCounter);
      }
   }
}