using Xunit;
using ChatBot.Classes;

namespace ChatBot.Tests
{
    public class BotEngineTests
    {
        [Fact]
        public void GetAnAnswer_WithValidKeyword_Hallo_ShouldReturnCorrectAnswer()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            string answer = botEngine.GetAnAnswer("Hallo");

            // Assert
            Assert.Equal("Hallo, wie geht es Ihnen?", answer);
        }

        [Fact]
        public void GetAnAnswer_WithValidKeyword_WieGehts_ShouldReturnCorrectAnswer()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            string answer = botEngine.GetAnAnswer("Wie gehts?");

            // Assert
            Assert.Equal("Mir geht es gut, danke! Und Ihnen?", answer);
        }

        [Fact]
        public void GetAnAnswer_WithValidKeyword_MirGehtEsGutUndWieGehtEsDir_ShouldReturnCorrectAnswer()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            string answer = botEngine.GetAnAnswer("Mir geht es gut und wie geht es dir?");

            // Assert
            Assert.Equal("Mir geht es blendend solange Sie mich nicht beenden :D", answer);
        }

        [Fact]
        public void GetAnAnswer_WithValidKeyword_GutUndDir_ShouldReturnCorrectAnswer()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            string answer = botEngine.GetAnAnswer("Gut und dir?");

            // Assert
            Assert.Equal("Mir geht es blendend solange Sie mich nicht beenden :D", answer);
        }

        [Fact]
        public void GetAnAnswer_WithInvalidKeyword_ShouldReturnDefaultAnswer()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            string answer = botEngine.GetAnAnswer("InvalidKeyword");

            // Assert
            Assert.Equal("Es tut mir leid, ich habe Sie nicht verstanden.", answer);
        }

        [Fact]
        public void ComputeLevenshteinDistance_WithEqualStrings_ShouldReturnZero()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            int distance = botEngine.ComputeLevenshteinDistance("test", "test");

            // Assert
            Assert.Equal(0, distance);
        }

        [Fact]
        public void ComputeLevenshteinDistance_WithDifferentStrings_ShouldReturnDistance()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            int distance = botEngine.ComputeLevenshteinDistance("kitten", "sitting");

            // Assert
            Assert.Equal(3, distance);
        }

        [Fact]
        public void ComputeLevenshteinDistance_WithEmptyStrings_ShouldReturnZero()
        {
            // Arrange
            BotEngine botEngine = new BotEngine();

            // Act
            int distance = botEngine.ComputeLevenshteinDistance("", "");

            // Assert
            Assert.Equal(0, distance);
        }
    }
}
