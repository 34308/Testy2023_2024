using JJ_API.Service.Buisneess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JJ_API.Tests
{
    [TestClass]
    public class CensorshipServiceTest
    {
        
        [TestMethod]
        public void CheckIfMouseIsCurse()
        {
            // Arrange
            string[] words=new string[] {"it","is","a","sentence","were","mouse","is","a","curse"} ;

            // Act
            var result = CensorshipService.CheckForCurses(words);

            // Assert
            Assert.IsTrue(!result.isClean, $"mouse is a course");
        }

        [TestMethod]
        [DataRow(new string[] { })]
        [DataRow(null)]
        public void CheckWhatHappensForEmptyAndNull(string[] words)
        {
            // Arrange
           
            // Act
            var result = CensorshipService.CheckForCurses(words);

            // Assert
            Assert.IsTrue(result.isClean, $"no words no curse");
        }
    }
}
