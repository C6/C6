using NUnit.Framework;
using static C6.Speed;



namespace C6.Tests.Enums
{
    [TestFixture]
    public class SpeedTests
    {
        [Test]
        public void Speed_ConstantIsLessThanLogarithmic_IsTrue() => 
            Assert.That(Constant, Is.LessThan(Log));


        [Test]
        public void Speed_LogarithmicIsLessThanLinear_IsTrue() => 
            Assert.That(Log, Is.LessThan(Linear));


        [Test]
        public void Speed_LinearIsLessThanPotentiallyInfinite_IsTrue() => 
            Assert.That(Linear, Is.LessThan(PotentiallyInfinite));
    }
}
