using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityDemo
{
   public class TextEditor
   {
      private readonly ISpellChecker _spellChecker;

      public TextEditor(ISpellChecker spellChecker)
      {
         _spellChecker = spellChecker;
      }
      private void CheckSpelling()
      {
         _spellChecker.CheckSpelling();
      }
   }

   public class TextEditorNoDi
   {
      private readonly SpellChecker _spellChecker;

      public TextEditorNoDi()
      {
         _spellChecker = new SpellChecker();
      }
      private void CheckSpelling()
      {
         _spellChecker.CheckSpelling();
      }
   }

   public interface ISpellChecker
   {
      bool CheckSpelling();
   }

   public class SpellChecker : ISpellChecker
   {
      public bool CheckSpelling()
      {
         throw new NotImplementedException();
      }
   }
}
