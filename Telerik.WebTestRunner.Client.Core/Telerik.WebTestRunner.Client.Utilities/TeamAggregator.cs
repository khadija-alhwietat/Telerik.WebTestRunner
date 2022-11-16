using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Telerik.WebTestRunner.Client.Configuration.Elements;
using Telerik.WebTestRunner.Client.Configuration.Sections;

namespace Telerik.WebTestRunner.Client.Utilities
{
	public class TeamAggregator
	{
		private static TeamAggregator current;

		private Dictionary<string, string> memberTeamLookup;

		private TeamAggregator()
		{
		}

		public static TeamAggregator GetCurrent()
		{
			if (current == null)
			{
				current = new TeamAggregator();
			}
			return current;
		}

		public string GetTeamNameByMemberAlias(string alias)
		{
			EnsureMemberLookupDictionary();
			if (memberTeamLookup.TryGetValue(alias, out string value))
			{
				return value;
			}
			return null;
		}

		private void EnsureMemberLookupDictionary()
		{
			if (memberTeamLookup == null)
			{
				memberTeamLookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
				foreach (TeamConfigElement team in ((TeamConfigSection)ConfigurationManager.GetSection("teamsConfiguration")).Teams)
				{
					foreach (TeamMemberConfigElement member in team.Members)
					{
						StringEnumerator enumerator3 = member.Aliases.GetEnumerator();
						try
						{
							while (enumerator3.MoveNext())
							{
								string key = enumerator3.Current;
								memberTeamLookup.Add(key, team.Name);
							}
						}
						finally
						{
							(enumerator3 as IDisposable)?.Dispose();
						}
					}
				}
			}
		}
	}
}
