using KorakPoKorak.Domain;

namespace KorakPoKorak.Domain.Tests
{
    public class WorkshopAwardRulesTests
    {
        [Fact]
        public void FirstWorkshop_AwardsMasterAndFirstStep()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(1, WorkshopComplexity.Beginner, 1);

            Assert.Contains(BadgeCodes.WorkshopMaster, codes);
            Assert.Contains(BadgeCodes.FirstWorkshop, codes);
            Assert.DoesNotContain(BadgeCodes.ThreeWorkshops, codes);
        }

        [Fact]
        public void ThirdWorkshop_AwardsMilestone()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(3, WorkshopComplexity.Beginner, 1);

            Assert.Contains(BadgeCodes.WorkshopMaster, codes);
            Assert.Contains(BadgeCodes.ThreeWorkshops, codes);
            Assert.DoesNotContain(BadgeCodes.FirstWorkshop, codes);
        }

        [Fact]
        public void FifthAndTenth_AwardMilestones()
        {
            Assert.Contains(BadgeCodes.FiveWorkshops,
                WorkshopAwardRules.GetBadgeCodes(5, WorkshopComplexity.Beginner, 1));
            Assert.Contains(BadgeCodes.TenWorkshops,
                WorkshopAwardRules.GetBadgeCodes(10, WorkshopComplexity.Beginner, 1));
        }

        [Fact]
        public void Intermediate_AwardsKnowledgeStar()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(1, WorkshopComplexity.Intermediate, 1);

            Assert.Contains(BadgeCodes.KnowledgeStar, codes);
            Assert.DoesNotContain(BadgeCodes.BraveStep, codes);
        }

        [Fact]
        public void Advanced_AwardsBraveStep()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(2, WorkshopComplexity.Advanced, 1);

            Assert.Contains(BadgeCodes.BraveStep, codes);
            Assert.Contains(BadgeCodes.Persistent, codes);
        }

        [Fact]
        public void MultipleActivityTypes_AwardsCreativeStar()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(1, WorkshopComplexity.Beginner, 2);

            Assert.Contains(BadgeCodes.CreativeStar, codes);
        }

        [Fact]
        public void SingleActivityType_DoesNotAwardCreativeStar()
        {
            var codes = WorkshopAwardRules.GetBadgeCodes(1, WorkshopComplexity.Beginner, 1);

            Assert.DoesNotContain(BadgeCodes.CreativeStar, codes);
        }
    }
}
