﻿namespace Zone.UmbracoPersonalisationGroups.Criteria.PagesViewed
{
    using System;
    using Newtonsoft.Json;
    using Umbraco.Core;

    /// <summary>
    /// Implements a personalisation group criteria based on the whether certain pages (node Ids) have been viewed
    /// </summary>
    public class PagesViewedPersonalisationGroupCriteria : IPersonalisationGroupCriteria
    {
        internal static string CriteriaAlias = "pagesViewed";

        private readonly IPagesViewedProvider _pagesViewedProvider;

        public PagesViewedPersonalisationGroupCriteria()
        {
            _pagesViewedProvider = new CookiePagesViewedProvider();
        }

        public PagesViewedPersonalisationGroupCriteria(IPagesViewedProvider pagesViewedProvider)
        {
            _pagesViewedProvider = pagesViewedProvider;
        }

        public string Name => "Pages viewed";

        public string Alias => CriteriaAlias;

        public string Description => "Matches visitor session with whether certain pages have been viewed";

        public bool MatchesVisitor(string definition)
        {
            Mandate.ParameterNotNullOrEmpty(definition, "definition");

            PagesViewedSetting pagesViewedSetting;
            try
            {
                pagesViewedSetting = JsonConvert.DeserializeObject<PagesViewedSetting>(definition);
            }
            catch (JsonReaderException)
            {
                throw new ArgumentException($"Provided definition is not valid JSON: {definition}");
            }

            var nodeIdsViewed = _pagesViewedProvider.GetNodeIdsViewed();

            switch (pagesViewedSetting.Match)
            {
                case PagesViewedSettingMatch.ViewedAny:
                    return pagesViewedSetting.NodeIds
                        .ContainsAny(nodeIdsViewed);
                case PagesViewedSettingMatch.ViewedAll:
                    return pagesViewedSetting.NodeIds
                        .ContainsAll(nodeIdsViewed);
                case PagesViewedSettingMatch.NotViewedAny:
                    return !pagesViewedSetting.NodeIds
                        .ContainsAny(nodeIdsViewed);
                case PagesViewedSettingMatch.NotViewedAll:
                    return !pagesViewedSetting.NodeIds
                        .ContainsAll(nodeIdsViewed);
                default:
                    return false;
            }
        }
    }
}
