namespace KorakPoKorak.Domain
{
    /// <summary>
    /// Candidate badge codes for a completed workshop. Duplicate prevention
    /// (once per child vs once per child+workshop) is enforced by the award service/DB.
    /// </summary>
    public static class WorkshopAwardRules
    {
        public static IReadOnlyList<string> GetBadgeCodes(
            int completedWorkshopCount,
            WorkshopComplexity complexity,
            int activityTypeCount)
        {
            var codes = new List<string> { BadgeCodes.WorkshopMaster };

            switch (completedWorkshopCount)
            {
                case 1:
                    codes.Add(BadgeCodes.FirstWorkshop);
                    break;
                case 2:
                    codes.Add(BadgeCodes.Persistent);
                    break;
                case 3:
                    codes.Add(BadgeCodes.ThreeWorkshops);
                    break;
                case 4:
                    codes.Add(BadgeCodes.CuriousMind);
                    break;
                case 5:
                    codes.Add(BadgeCodes.FiveWorkshops);
                    break;
                case 6:
                    codes.Add(BadgeCodes.GreatProgress);
                    break;
                case 7:
                    codes.Add(BadgeCodes.Dedicated);
                    break;
                case 8:
                    codes.Add(BadgeCodes.Explorer);
                    break;
                case 10:
                    codes.Add(BadgeCodes.TenWorkshops);
                    break;
                case 15:
                    codes.Add(BadgeCodes.SuperLearner);
                    break;
                case 20:
                    codes.Add(BadgeCodes.Champion);
                    break;
            }

            if (complexity == WorkshopComplexity.Intermediate)
                codes.Add(BadgeCodes.KnowledgeStar);

            if (complexity == WorkshopComplexity.Advanced)
                codes.Add(BadgeCodes.BraveStep);

            if (activityTypeCount >= 2)
                codes.Add(BadgeCodes.CreativeStar);

            return codes;
        }
    }
}
