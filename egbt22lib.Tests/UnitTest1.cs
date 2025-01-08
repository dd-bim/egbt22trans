using System;
using Xunit;
using egbt22lib;

namespace egbt22lib.Tests;

public class UnitTest1
{
    public class ConvertTests
    {
        [Fact]
        public void DBRef_GK5_to_EGBT22_Local_ValidInput_ReturnsTrue()
        {
            // Arrange
            double[] gk5Rechts = { 1000, 2000, 3000 };
            double[] gk5Hoch = { 4000, 5000, 6000 };
            double[] h = { 10, 20, 30 };

            // Act
            bool result = Convert.DBRef_GK5_to_EGBT22_Local(gk5Rechts, gk5Hoch, h, out double[] localRechts, out double[] localHoch);

            // Assert
            Assert.True(result);
            Assert.NotEmpty(localRechts);
            Assert.NotEmpty(localHoch);
        }

        [Fact]
        public void DBRef_GK5_to_EGBT22_Local_InvalidInput_ReturnsFalse()
        {
            // Arrange
            double[] gk5Rechts = { 1000, 2000 };
            double[] gk5Hoch = { 4000, 5000, 6000 };
            double[] h = { 10, 20, 30 };

            // Act
            bool result = Convert.DBRef_GK5_to_EGBT22_Local(gk5Rechts, gk5Hoch, h, out double[] localRechts, out double[] localHoch);

            // Assert
            Assert.False(result);
            Assert.Empty(localRechts);
            Assert.Empty(localHoch);
        }

        [Fact]
        public void EGBT22_Local_to_DBRef_GK5_ValidInput_ReturnsTrue()
        {
            // Arrange
            double[] localRechts = { 1000, 2000, 3000 };
            double[] localHoch = { 4000, 5000, 6000 };
            double[] h = { 10, 20, 30 };

            // Act
            bool result = Convert.EGBT22_Local_to_DBRef_GK5(localRechts, localHoch, h, out double[] gk5Rechts, out double[] gk5Hoch);

            // Assert
            Assert.True(result);
            Assert.NotEmpty(gk5Rechts);
            Assert.NotEmpty(gk5Hoch);
        }

        [Fact]
        public void EGBT22_Local_to_DBRef_GK5_InvalidInput_ReturnsFalse()
        {
            // Arrange
            double[] localRechts = { 1000, 2000 };
            double[] localHoch = { 4000, 5000, 6000 };
            double[] h = { 10, 20, 30 };

            // Act
            bool result = Convert.EGBT22_Local_to_DBRef_GK5(localRechts, localHoch, h, out double[] gk5Rechts, out double[] gk5Hoch);

            // Assert
            Assert.False(result);
            Assert.Empty(gk5Rechts);
            Assert.Empty(gk5Hoch);
        }

        [Fact]
        public void DBRef_GK5_to_EGBT22_Local_Calculation()
        {
            // Arrange
            double[] gk5Rechts = { 5421156.142, 5422904.161, 5422365.311, 5422424.357, 5422771.097, 5423363.809, 5423977.187, 5424750.005 };
            double[] gk5Hoch = { 5649020.237, 5644705.306, 5639810.849, 5634811.344, 5629830.214, 5624865.469, 5619903.725, 5617815.93 };
            double[] h = { 121.043, 145.543, 165.543, 185.543, 205.543, 217.795, 197.795, 188.866 };

            double[] expectedLocalRechts = { 8094.7348, 9906.6333, 9440.5767, 9573.8410, 9994.4368, 10660.6788, 11347.4931, 12151.1235 };
            double[] expectedLocalHoch = { 66280.7435, 61992.4437, 57090.7089, 52092.8253, 47117.5796, 42162.3552, 37210.4251, 35134.3789 };

            // Act
            bool result = Convert.DBRef_GK5_to_EGBT22_Local(gk5Rechts, gk5Hoch, h, out double[] localRechts, out double[] localHoch);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedLocalRechts.Length, localRechts.Length);
            Assert.Equal(expectedLocalHoch.Length, localHoch.Length);

            for (int i = 0; i < expectedLocalRechts.Length; i++)
            {
                Assert.Equal(expectedLocalRechts[i], localRechts[i], 4);
                Assert.Equal(expectedLocalHoch[i], localHoch[i], 4);
            }
        }

        [Fact]
        public void EGBT22_Local_to_DBRef_GK5_Calculation()
        {
            // Arrange
            double[] localRechts = { 8094.7348, 9906.6333, 9440.5767, 9573.8410, 9994.4368, 10660.6788, 11347.4931, 12151.1235 };
            double[] localHoch = { 66280.7435, 61992.4437, 57090.7089, 52092.8253, 47117.5796, 42162.3552, 37210.4251, 35134.3789 };
            double[] h = { 121.043, 145.543, 165.543, 185.543, 205.543, 217.795, 197.795, 188.866 };

            double[] expectedGk5Rechts = { 5421156.142, 5422904.161, 5422365.311, 5422424.357, 5422771.097, 5423363.809, 5423977.187, 5424750.005 };
            double[] expectedGk5Hoch = { 5649020.237, 5644705.306, 5639810.849, 5634811.344, 5629830.214, 5624865.469, 5619903.725, 5617815.93 };

            // Act
            bool result = Convert.EGBT22_Local_to_DBRef_GK5(localRechts, localHoch, h, out double[] gk5Rechts, out double[] gk5Hoch);

            // Assert
            Assert.True(result);
            Assert.Equal(expectedGk5Rechts.Length, gk5Rechts.Length);
            Assert.Equal(expectedGk5Hoch.Length, gk5Hoch.Length);

            for (int i = 0; i < expectedGk5Rechts.Length; i++)
            {
                Assert.Equal(expectedGk5Rechts[i], gk5Rechts[i], 3);
                Assert.Equal(expectedGk5Hoch[i], gk5Hoch[i], 3);
            }
        }

    }
}